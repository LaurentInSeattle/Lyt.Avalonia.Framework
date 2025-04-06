using Lyt.Avalonia.Mvvm.Extensions;
using System.Runtime.InteropServices;

namespace Lyt.Avalonia.Mvvm;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ApplicationBase(
    string organizationKey,
    string applicationKey,
    string uriString,
    Type mainWindowType,
    Type applicationModelType,
    List<Type> modelTypes,
    List<Type> singletonTypes,
    List<Tuple<Type, Type>> servicesInterfaceAndType,
    bool singleInstanceRequested = false,
    Uri? splashImageUri = null, 
    Window? appSplashWindow =null) : Application, IApplicationBase
{
    public static Window MainWindow { get; private set; }

    // The host cannot be null or else there is no app...
    public static IHost AppHost { get; private set; }

    // Logger will never be null or else the app did not take off
    public ILogger Logger { get; private set; }

    // Can be null ! 
    private Window? splashWindow;

    // LATER, maybe, using Fluent theme for now
    // public StyleManager StyleManager { get; private set; }

#pragma warning restore CS8618

    // To enforce single instance 
    private static FileStream? LockFile;

    private readonly string organizationKey = organizationKey;
    private readonly string applicationKey = applicationKey;
#pragma warning disable IDE0052 // Remove unread private members
    // We may need this one later 
    private readonly string uriString = uriString;
#pragma warning restore IDE0052 
    private readonly Type mainWindowType = mainWindowType;
    private readonly Type applicationModelType = applicationModelType;
    private readonly List<Type> modelTypes = modelTypes;
    private readonly List<Type> singletonTypes = singletonTypes;
    private readonly List<Tuple<Type, Type>> servicesInterfaceAndType = servicesInterfaceAndType;
    private readonly List<Type> validatedModelTypes = [];
    private readonly bool isSingleInstanceRequested = singleInstanceRequested;
    private readonly Uri? splashImageUri = splashImageUri;
    private readonly Window? appSplashWindow = appSplashWindow; 

    private IClassicDesktopStyleApplicationLifetime? desktop;

    public static T GetRequiredService<T>() where T : notnull
        => ApplicationBase.AppHost!.Services.GetRequiredService<T>();

    public static object GetRequiredService(Type type)
        => ApplicationBase.AppHost!.Services.GetRequiredService(type);

    public static T? GetOptionalService<T>() where T : notnull
        => ApplicationBase.AppHost!.Services.GetService<T>();

    public static object? GetOptionalService(Type type)
        => ApplicationBase.AppHost!.Services.GetService(type);

    public static TModel GetModel<TModel>() where TModel : notnull
    {
        TModel? model = ApplicationBase.GetRequiredService<TModel>() ??
            throw new ApplicationException("No model of type " + typeof(TModel).FullName);
        bool isModel = typeof(IModel).IsAssignableFrom(typeof(TModel));
        if (!isModel)
        {
            throw new ApplicationException(typeof(TModel).FullName + "  is not a IModel");
        }

        return model;
    }

    public IEnumerable<IModel> GetModels()
    {
        List<IModel> models = [];
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ApplicationBase.AppHost!.Services.GetRequiredService(type);
            bool isModel = typeof(IModel).IsAssignableFrom(model.GetType());
            if (isModel)
            {
                models.Add((model as IModel)!);
            }
        }

        return models;
    }

    public async Task Shutdown()
    {
        this.Logger.Info("***   Shutdown   ***");
        await this.OnShutdownBegin();

        //startupWindow.Closing += (_, _) => { this.logViewer?.Close(); };
        IApplicationModel applicationModel = ApplicationBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Shutdown();
        await ApplicationBase.AppHost!.StopAsync();
        await this.OnShutdownComplete();

        this.ForceShutdown();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Try to catch all exceptions, missing the ones on the main thread at this time 
        TaskScheduler.UnobservedTaskException += this.OnTaskSchedulerUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;
        Dispatcher.UIThread.ShutdownStarted += this.OnDispatcherShutdownStarted;

        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            this.desktop = lifetime;

            if (this.desktop is null)
            {
                throw new InvalidOperationException("Desktop should not be null.");
            }

            // Enforce single instance if requested 
            if (this.isSingleInstanceRequested && this.IsAlreadyRunning())
            {
                this.ForceShutdown();
                return;
            }

            if ((this.splashImageUri is not null)&& (this.appSplashWindow is not null))
            {
                throw new InvalidOperationException("Cannot have two splash windows.");
            }
            
            if (this.splashImageUri is not null)
            {
                // Show default splash window
                this.splashWindow = new ImageSplashWindow(this.splashImageUri);
                this.desktop.MainWindow = this.splashWindow;
            }

            if (this.appSplashWindow is not null)
            {
                // Show app provided splash screen window
                this.splashWindow = this.appSplashWindow;
                this.desktop.MainWindow = this.splashWindow;
            }
        }

        // Let Avalonia complete its own startup and show us the splash.
        // Note: Base class doing nothing, but keep: may change in the future 
        base.OnFrameworkInitializationCompleted();

        // Launch the actual init of the app, delay just a bit to ensure the splash shows up
        Schedule.OnUiThread(50, this.InitializeApplication, DispatcherPriority.ApplicationIdle);
    }

    protected virtual Task OnStartupBegin() => Task.CompletedTask;

    protected virtual Task OnStartupComplete() => Task.CompletedTask;

    protected virtual Task OnShutdownBegin() => Task.CompletedTask;

    protected virtual Task OnShutdownComplete() => Task.CompletedTask;

    private async void InitializeApplication()
    {
        this.InitializeHosting();

        if (Design.IsDesignMode)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (this.desktop is not null)
        {
            var startupWindow = ApplicationBase.GetRequiredService<Window>();
            if (startupWindow is Window window)
            {
                // Create the main window without showing it 
                ApplicationBase.MainWindow = window;

                // LATER, maybe, using Fluent theme for now
                // this.StyleManager = new StyleManager(window);

                // Start and wait for startup to complete
                await this.Startup();

                // Show the main window once init is fully complete 
                this.desktop.MainWindow = ApplicationBase.MainWindow;
                ApplicationBase.MainWindow.Show();

                // Close the splash screen if any was created 
                this.splashWindow?.Close();
            }
            else
            {
                throw new NotImplementedException("Failed to create MainWindow");
            }
        }
        else
        {
            // Still in designer mode ? 
            throw new InvalidOperationException("Desktop should not be null.");
        }
    }

    private void InitializeHosting()
    {
        ApplicationBase.AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_0, services) =>
                {
                    // Register the app
                    _ = services.AddSingleton<IApplicationBase>(this);

                    // Always Main Window 
                    _ = services.AddSingleton(typeof(Window), this.mainWindowType);

                    // The Application Model, also  a singleton, no need here to also add it without the inferface  
                    _ = services.AddSingleton(typeof(IApplicationModel), this.applicationModelType);

                    // Models 
                    foreach (Type modelType in this.modelTypes)
                    {
                        bool isModel = typeof(IModel).IsAssignableFrom(modelType);
                        if (isModel)
                        {
                            // Models can be retrieved all via the interface or retrieved only one by providing specific type,
                            // just like singletons below
                            _ = services.AddSingleton(modelType);
                            this.validatedModelTypes.Add(modelType);
                        }
                        else
                        {
                            Debug.WriteLine(modelType.FullName!.ToString() + " is not a IModel");
                        }
                    }

                    // Singletons, they do not need an interface. 
                    foreach (var singletonType in this.singletonTypes)
                    {
                        _ = services.AddSingleton(singletonType);
                    }

                    // Services, all must comply to a specific interface 
                    foreach (var serviceType in this.servicesInterfaceAndType)
                    {
                        var interfaceType = serviceType.Item1;
                        var implementationType = serviceType.Item2;
                        _ = services.AddSingleton(interfaceType, implementationType);
                    }

                }).Build();
    }

    protected static Tuple<Type, Type> OsSpecificService<TInterface>(string implementationName)
    {
        // Only Windows and MacOS for now 
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                // OSPlatform.Linux is NOT supported, at least for now, no way to test it here
                throw new ArgumentException("Unsupported platform: " + RuntimeInformation.OSDescription);
            }

            var maybeAssembly = Assembly.GetEntryAssembly();
            if (maybeAssembly is Assembly assembly)
            {
                // Type? maybeType = assembly.GetType("WallpaperService");
                var typeInfos = assembly.DefinedTypes;
                TypeInfo? maybeTypeInfo =
                    (from typeInfo in typeInfos 
                     where typeInfo.Name == implementationName 
                     select typeInfo)
                    .FirstOrDefault();
                if (maybeTypeInfo is not null && maybeTypeInfo.AsType() is Type type)
                {
                    if (type.Implements<TInterface>())
                    {
                        object? instance = Activator.CreateInstance(type);
                        if (instance is TInterface service)
                        {
                            return new Tuple<Type, Type>(typeof(TInterface), instance.GetType());
                        }
                    }
                }
            }

            throw new ArgumentException(
                "Failed to create instance of service " + implementationName + " for " + RuntimeInformation.OSDescription);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            throw;
        }
    }


    private async Task Startup()
    {
        await ApplicationBase.AppHost.StartAsync();
        await this.OnStartupBegin();

        var logger = ApplicationBase.GetRequiredService<ILogger>();
        this.Logger = logger;

        if (Debugger.IsAttached && this.Logger is LogViewerWindow logViewer)
        {
            try
            {
                logViewer.Show();
            }
            catch (Exception) { /* swallow */ }
        }

        this.Logger.Info("***   Startup   ***");

        // Warming up the models: 
        // This ensures that the Application Model and all listed models are constructed.
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ApplicationBase.AppHost!.Services.GetRequiredService(type);
            if (model is not IModel)
            {
                throw new ApplicationException("Failed to warmup model: " + type.FullName);
            }
        }

        IApplicationModel applicationModel = ApplicationBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Initialize();
        await this.OnStartupComplete();
    }

    private void ForceShutdown()
    {
        if (this.desktop is not null)
        {
            this.desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.desktop.Shutdown();
        }
    }

    private bool IsAlreadyRunning()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            // No multiple instances on Mac 
            return false;
        }
        else
        {
            // Windows or Unix
            try
            {
                string directory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.organizationKey);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = Path.Combine(directory, string.Concat(this.applicationKey, ".lock"));
                ApplicationBase.LockFile = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                if (ApplicationBase.LockFile is not null)
                {
                    ApplicationBase.LockFile.Lock(0, 0);
                    return false;
                }
            }
            catch
            {
                // Swallow and assume we are permitted to run 
                return false;
            }

            return true;
        }
    }

    private void OnDispatcherShutdownStarted(object? sender, EventArgs e)
    {
        if (Debugger.IsAttached)
        {
            // Use this break to debug issues at startup, if needed 
            // Debugger.Break();
        }

        this.Logger.Info("***   Shutdown Started   ***");
    }

    private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        => this.GlobalExceptionHandler(e.ExceptionObject as Exception);

    private void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        => this.GlobalExceptionHandler(e.Exception);

    private void GlobalExceptionHandler(Exception? exception)
    {
        if (Debugger.IsAttached) { Debugger.Break(); }

        if ((this.Logger is not null) && (exception is not null))
        {
            this.Logger.Error(exception.ToString());
        }

        // ??? 
        // What can we do here ? 
    }
}
