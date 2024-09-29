namespace Lyt.Avalonia.Framework.TestApp.Models; 

public sealed class TimingModel(IMessenger messenger, ILogger logger) 
    : ModelBase(messenger, logger), IModel
{
    private DispatcherTimer? dispatcherTimer;

    public int TickCount { get => this.Get<int>(); set => this.Set(value); }

    public bool IsTicking { get => this.Get<bool>(); set => this.Set(value); }

    public override Task Initialize()
    {
        this.Start();
        return Task.CompletedTask;
    }

    public override Task Shutdown()
    {
        this.Stop();
        return Task.CompletedTask;
    }

    public void Start ()
    {
        if ( this.dispatcherTimer is null)
        {
            ++this.TickCount;
            this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4), IsEnabled = false };
            this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;
            this.dispatcherTimer.Start();
            this.IsTicking = true;
        }
    }

    public void Stop()
    {
        if (this.dispatcherTimer is not null)
        {
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.Tick -= this.OnDispatcherTimerTick;
            this.dispatcherTimer = null;
            this.IsTicking = false;
        }
    }

    private void OnDispatcherTimerTick(object? sender, EventArgs e) => ++this.TickCount;
}
