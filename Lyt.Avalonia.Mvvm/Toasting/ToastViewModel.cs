namespace Lyt.Avalonia.Mvvm.Toasting;

public sealed class ToastViewModel : Bindable<ToastView> 
{
    private const int NoDelay = 0;
    private const int MinDelay = 1_000;
    private const int MaxDelay = 60_000;

    private readonly IToaster toaster;

    public ToastViewModel(IToaster toaster)
    {
        this.toaster = toaster;
        this.IconGeometry = InformationLevel.Info.ToIconGeometry();
        this.ColorLevel = InformationLevel.Info.ToBrush();
    }

    private DispatcherTimer? dismissTimer;

    public void Show(string title, string message, int dismissDelay, InformationLevel toastLevel)
    {
        this.Title = title;
        this.Message = message;
        this.IconGeometry = toastLevel.ToIconGeometry();
        this.ColorLevel = toastLevel.ToBrush();
        this.DismissCommand = new Command(this.Dismiss);
        string loggedMessage =
            "Toast: " + toastLevel.ToString() + " - " + title + " - " + message;
        if (toastLevel == InformationLevel.Error)
        {
            this.Logger.Error(loggedMessage);
        }
        else if (toastLevel == InformationLevel.Warning)
        {
            this.Logger.Warning(loggedMessage);
        }
        else

        {
            this.Logger.Info(loggedMessage);
        }

        if (dismissDelay == ToastViewModel.NoDelay)
        {
            // dismiss on click or explicit request 
        }
        else
        {
            // auto dismiss after delay
            if (dismissDelay < ToastViewModel.MinDelay)
            {
                dismissDelay = ToastViewModel.MinDelay;
            }
            else if (dismissDelay > ToastViewModel.MaxDelay)
            {
                dismissDelay = ToastViewModel.MaxDelay;
            }

            this.StopTimer();
            this.dismissTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(dismissDelay),
                IsEnabled = true,
            };
            this.dismissTimer.Tick += this.DismissTimerTick;
        }
    }

    private void DismissTimerTick(object? _, EventArgs e) => this.Dismiss();

    private void Dismiss(object? _) => this.Dismiss();

    public void Dismiss()
    {
        this.StopTimer();
        this.toaster.Dismiss();
    }

    private void StopTimer()
    {
        if (this.dismissTimer != null)
        {
            this.dismissTimer.Stop();
            this.dismissTimer = null;
        }
    }

    public ICommand DismissCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    /// <summary> Gets or sets the ColorLevel bound property.</summary>
    public SolidColorBrush ColorLevel { get => this.Get<SolidColorBrush>()!; [DoNotLog] set => _ = this.Set(value); }

    /// <summary> Gets or sets the IconName bound property.</summary>
    public StreamGeometry IconGeometry { get => this.Get<StreamGeometry>()!; [DoNotLog] set => _ = this.Set(value); }

    /// <summary> Gets or sets the Title bound property.</summary>
    public string? Title { get => this.Get<string>(); [DoNotLog] set => _ = this.Set(value); }

    /// <summary> Gets or sets the Message bound property.</summary>
    public string? Message { get => this.Get<string>(); [DoNotLog] set => _ = this.Set(value); }
}
