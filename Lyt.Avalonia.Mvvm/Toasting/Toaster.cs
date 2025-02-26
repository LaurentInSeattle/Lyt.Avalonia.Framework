namespace Lyt.Avalonia.Mvvm.Toasting;

public sealed class Toaster : IToaster
{
    private readonly IMessenger messenger;
    private ToastViewModel? current;
    private bool hostPanelHitTestVisibility;

    public Panel? hostPanel;

    public Toaster(IMessenger messenger)
    {
        this.messenger = messenger;
        this.messenger.Subscribe<ToastMessage.Show>(this.OnShow, withUiDispatch: true);
        this.messenger.Subscribe<ToastMessage.Dismiss>(this.OnDismiss, withUiDispatch: true);
    }

    public bool BreakOnError { get; set; } = true;

    // The host panel that will show the toasts 
    public object? Host
    {
        get => this.hostPanel;
        set
        {
            if (value is not Panel panel)
            {
                throw new Exception("The Toaster Host must be a panel.");
            }

            this.hostPanel = panel;
        }
    }

    // Provide access to the View so that it can eventually moved around, re-aligned, etc
    public object? View
    {
        get 
        {
            if (this.current is ToastViewModel viewModel)
            {
                return viewModel.View;
            }

            return null;
        }
    }

    public void Show(string title, string message, int dismissDelay = 10, InformationLevel toastLevel = InformationLevel.Info)
    {
        if (this.BreakOnError && toastLevel == InformationLevel.Error && Debugger.IsAttached)
        {
            Debugger.Break();
        }

        this.messenger.Publish(
            new ToastMessage.Show { Title = title, Message = message, Delay = dismissDelay, Level = toastLevel });
    } 

    public void Dismiss()
    {
        if (this.Host == null)
        {
            // No content control to host the toast, that could be problematic...
            if (Debugger.IsAttached) { Debugger.Break(); }
            return;
        }

        if (this.current == null)
        {
            // Nothing to dismiss, usually not really an issue
            // if (Debugger.IsAttached) { Debugger.Break(); }
            return;
        }

        if (this.Host is not Panel panel)
        {
            throw new Exception("The Toaster Host Panel is not initialized.");
        }

        // Restore the hit test status of the host panel 
        panel.IsHitTestVisible = this.hostPanelHitTestVisibility;

        ToastView? view = this.current.View;
        if (view is not null)
        {
            panel.Children.Remove(view);
        }

        this.messenger.Publish(new ToastMessage.OnDismiss());
    }

    private void OnDismiss(ToastMessage.Dismiss _) => this.Dismiss();

    private void OnShow(ToastMessage.Show show)
    {
        if (this.Host is not Panel panel)
        {
            // No content control to host the toast
            if (Debugger.IsAttached) { Debugger.Break(); }
            throw new Exception("The Toaster Host Panel is not initialized.");
        }

        if (this.current is null)
        {
            this.current = new ToastViewModel(this);
            _ = this.current.CreateViewAndBind();
        }

        ToastView? view = this.current.View;
        if (view is not null)
        {
            this.current.Show(show.Title, show.Message, show.Delay, show.Level);
            if (!panel.Children.Contains(view))
            {
                // Save hit test status and make the panel clickable so that we can 
                // dismiss toasts if needed.
                this.hostPanelHitTestVisibility = panel.IsHitTestVisible;
                panel.IsHitTestVisible = true ;
                panel.Children.Add(view);
            } 
        }
    }
}
