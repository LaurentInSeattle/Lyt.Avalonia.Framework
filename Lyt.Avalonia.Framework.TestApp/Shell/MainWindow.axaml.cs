namespace Lyt.Avalonia.Framework.TestApp.Shell;

public partial class MainWindow : Window
{
    private bool isShutdownRequested;

    private bool isShutdownComplete;

    public MainWindow()
    {
        this.InitializeComponent();

        this.Closing += this.OnMainWindowClosing;
        this.Loaded +=
            (s, e) =>
            {
                var vm = App.GetRequiredService<ShellViewModel>();
                vm.CreateViewAndBind();
                this.Content = vm.View;
            }; 
    }

    private void OnMainWindowClosing(object? sender, CancelEventArgs e)
    {
        if (!this.isShutdownComplete)
        {
            e.Cancel = true;
        }

        if (!this.isShutdownRequested)
        {
            this.isShutdownRequested = true;
            Schedule.OnUiThread(50,
                async () =>
                {
                    var app = App.GetRequiredService<IApplicationBase>();
                    await app.Shutdown();
                    this.isShutdownComplete = true;
                    this.Close();
                }, DispatcherPriority.Normal);
        }
    }
}
