using Lyt.Avalonia.Interfaces.Profiler;

namespace Lyt.Avalonia.Framework.TestApp;

public partial class App : ApplicationBase
{
    public const string Organization = "Lyt";
    public const string Application = "LytAvaloniaFrameworkTestApp";
    public const string RootNamespace = "Lyt.Avalonia.Framework.TestApp";
    public const string AssemblyName = "Lyt.Avalonia.Framework.TestApp";
    public const string AssetsFolder = "Assets";

    public App() : base(
        App.Organization,
        App.Application,
        App.RootNamespace,
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(TimingModel),
            typeof(UserAdministrationModel),
            typeof(FileManagerModel),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           typeof(StartupViewModel),
           typeof(LoginViewModel),
           typeof(SelectViewModel),
           typeof(ProcessViewModel),
        ],
        [
            // Services 
#if DEBUG
            new Tuple<Type, Type>(typeof(ILogger), typeof(LogViewerWindow)),
#else
            new Tuple<Type, Type>(typeof(ILogger), typeof(Logger)),
#endif
            new Tuple<Type, Type>(typeof(IProfiler), typeof(Profiler)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
        ],
        singleInstanceRequested: true)
    {
        // This should be empty, use the OnStartup override
    }

    protected override async Task OnStartupBegin()
    {
        var logger = App.GetRequiredService<ILogger>();
        logger.Debug("OnStartupBegin begins");

        // This needs to complete before all models are initialized.
        var fileManager = App.GetRequiredService<FileManagerModel>();
        await fileManager.Configure(
            new FileManagerConfiguration(
                App.Organization, App.Application, App.RootNamespace, App.AssemblyName, App.AssetsFolder));

        // Not used for now 
        //// The localizer needs the File Manager, do not change the order.
        //var localizer = App.GetRequiredService<LocalizerModel>();
        //await localizer.Configure(
        //    new LocalizerConfiguration
        //    {
        //        AssemblyName = App.AssemblyName,
        //        Languages = ["en-US", "fr-FR", "it-IT"],
        //        // Use default for all other config parameters 
        //    });

        logger.Debug("OnStartupBegin complete");
    }

    // Why does it needs to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
