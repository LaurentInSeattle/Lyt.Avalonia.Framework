namespace Lyt.Console;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ConsoleBase(
    string organizationKey,
    string applicationKey,
    string uriString,
    Type applicationModelType,
    List<Type> modelTypes,
    List<Type> singletonTypes,
    List<Tuple<Type, Type>> servicesInterfaceAndType) : IApplicationBase
{
    // The host cannot be null or else there is no app...
    public static IHost AppHost { get; private set; }

    // Logger will never be null or else the app did not take off
    public ILogger Logger { get; private set; }

#pragma warning restore CS8618

    private readonly string organizationKey = organizationKey;
    private readonly string applicationKey = applicationKey;
#pragma warning disable IDE0052 // Remove unread private members
    // We may need this one later 
    private readonly string uriString = uriString;
#pragma warning restore IDE0052 
    private readonly Type applicationModelType = applicationModelType;
    private readonly List<Type> modelTypes = modelTypes;
    private readonly List<Type> singletonTypes = singletonTypes;
    private readonly List<Tuple<Type, Type>> servicesInterfaceAndType = servicesInterfaceAndType;
    private readonly List<Type> validatedModelTypes = [];

    public static T GetRequiredService<T>() where T : notnull
        => ConsoleBase.AppHost!.Services.GetRequiredService<T>();

    public static object GetRequiredService(Type type)
        => ConsoleBase.AppHost!.Services.GetRequiredService(type);

    public static T? GetOptionalService<T>() where T : notnull
        => ConsoleBase.AppHost!.Services.GetService<T>();

    public static object? GetOptionalService(Type type)
        => ConsoleBase.AppHost!.Services.GetService(type);

    public static TModel GetModel<TModel>() where TModel : notnull
    {
        TModel? model = ConsoleBase.GetRequiredService<TModel>() ??
            throw new ApplicationException("No model of type " + typeof(TModel).FullName);
        bool isModel = typeof(IModel).IsAssignableFrom(typeof(TModel));
        if (!isModel)
        {
            throw new ApplicationException(typeof(TModel).FullName + "  is not a IModel");
        }

        return model;
    }

    public static void Print(string text)
    {
        if ( string.IsNullOrWhiteSpace(text) )
        {
            Debug.WriteLine("Tried to output empty text..."); 
            return; 
        }

        Debug.WriteLine(text);
        System.Console.WriteLine(text);
    }

    public IEnumerable<IModel> GetModels()
    {
        List<IModel> models = [];
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ConsoleBase.AppHost!.Services.GetRequiredService(type);
            bool isModel = typeof(IModel).IsAssignableFrom(model.GetType());
            if (isModel)
            {
                models.Add((model as IModel)!);
            }
        }

        return models;
    }

    public void Initialize()
    {
        // Try to catch all exceptions, missing the ones on the main thread at this time 
        TaskScheduler.UnobservedTaskException += this.OnTaskSchedulerUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;

        this.InitializeHosting();

        this.OnStartupBegin();
        _ = this.Startup(); 
        this.OnStartupComplete();
    }

    public async Task Shutdown()
    {
        this.Logger.Info("***   Shutdown   ***");
        this.OnShutdownBegin();

        IApplicationModel applicationModel = ConsoleBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Shutdown();
        await ConsoleBase.AppHost!.StopAsync();
        this.OnShutdownComplete();

        ForceShutdown();
    }

    protected virtual void OnStartupBegin() { }

    protected virtual void OnStartupComplete() { }

    protected virtual void OnShutdownBegin() { }

    protected virtual void OnShutdownComplete() { }

    private void InitializeHosting()
    {
        ConsoleBase.AppHost = 
            Host.CreateDefaultBuilder()
                .ConfigureServices((_0, services) =>
                {
                    // Register the app
                    _ = services.AddSingleton<IApplicationBase>(this);

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
                        try
                        {
                            var interfaceType = serviceType.Item1;
                            var implementationType = serviceType.Item2;
                            _ = services.AddSingleton(interfaceType, implementationType);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex); 
                            throw;
                        }
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
                var typeInfos = assembly.DefinedTypes;
                TypeInfo? maybeTypeInfo =
                    (from typeInfo in typeInfos
                     where typeInfo.Name == implementationName
                     select typeInfo)
                    .FirstOrDefault();
                if (maybeTypeInfo is not null && maybeTypeInfo.AsType() is Type type)
                {
                    if (typeof(TInterface).IsAssignableFrom(type))
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
        await ConsoleBase.AppHost.StartAsync();
        this.OnStartupBegin();

        var logger = ConsoleBase.GetRequiredService<ILogger>();
        this.Logger = logger;
        this.Logger.Info("***   Startup   ***");

        // Warming up the models: 
        // This ensures that the Application Model and all listed models are constructed.
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ConsoleBase.AppHost!.Services.GetRequiredService(type);
            if (model is not IModel)
            {
                throw new ApplicationException("Failed to warmup model: " + type.FullName);
            }
        }

        IApplicationModel applicationModel = ConsoleBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Initialize();
        this.OnStartupComplete();
    }

    private static void ForceShutdown() => AppHost.StopAsync();

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
