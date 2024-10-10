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
    bool singleInstanceRequested = false) : Application, IApplicationBase
{
    public static Window MainWindow; 

    // The host cannot be null or else there is no app...
    public static IHost AppHost { get; private set; }

    // Logger will never be null or else the app did not take off
    public ILogger Logger { get; private set; }

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

    private IClassicDesktopStyleApplicationLifetime? desktop;

    public override async void OnFrameworkInitializationCompleted()
    {
        // Try to catch all exceptions, missing the ones on the main thread at this time 
        TaskScheduler.UnobservedTaskException += this.OnTaskSchedulerUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;

        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            this.desktop = lifetime;
        }

        // Enforce single instance if requested 
        if (this.isSingleInstanceRequested && this.IsAlreadyRunning() && (this.desktop is not null))
        {
            this.ForceShutdown();
            return;
        }

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
                ApplicationBase.MainWindow = window;
                this.desktop.MainWindow = window;
                // LATER, maybe, using Fluent theme for now
                // this.StyleManager = new StyleManager(window);
            }
            else
            {
                throw new NotImplementedException("Failed to create MainWindow");
            }
        }
        else
        {
            // Should not be in designer mode
            throw new NotImplementedException("Unsupported Application Lifetime");
        }

        base.OnFrameworkInitializationCompleted();
        await this.Startup();
    }

    private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        this.GlobalExceptionHandler(exception);
    }

    private void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var exception = e.Exception;
        this.GlobalExceptionHandler(exception);
    }

    private void InitializeHosting()
    {
        ApplicationBase.AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_0, services) =>
                {
                    // Register the app
                    _ = services.AddSingleton(typeof(IApplicationBase), this);

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

    public static T GetRequiredService<T>() where T : notnull
        => ApplicationBase.AppHost!.Services.GetRequiredService<T>();

    public static object GetRequiredService(Type type)
        => ApplicationBase.AppHost!.Services.GetRequiredService(type);

    public static TModel GetModel<TModel>() where TModel : notnull
    {
        TModel? model = ApplicationBase.GetRequiredService<TModel>() ??
            throw new ApplicationException("No model of type " + typeof(TModel).FullName);
        bool isModel = typeof(IModel).IsAssignableFrom(typeof(TModel));
        if (!isModel)
        {
            throw new ApplicationException(typeof(TModel).FullName + "  is not a IMOdel");
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

    protected virtual Task OnStartupBegin() => Task.CompletedTask;

    protected virtual Task OnStartupComplete() => Task.CompletedTask;

    protected virtual Task OnShutdownBegin() => Task.CompletedTask;

    protected virtual Task OnShutdownComplete() => Task.CompletedTask;

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
        this.WarmupModels();
        IApplicationModel applicationModel = ApplicationBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Initialize();
        await this.OnStartupComplete();
    }

    private void WarmupModels()
    {
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ApplicationBase.AppHost!.Services.GetRequiredService(type);
            if (model is not IModel)
            {
                throw new ApplicationException("Failed to warmup model: " + type.FullName);
            }
        }
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
            catch { /* Swallow */  }

            return true;
        }
    }

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