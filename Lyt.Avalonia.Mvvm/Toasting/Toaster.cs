namespace Lyt.Avalonia.Controls.Toasting;

public sealed class Toaster : IToaster
{
    private readonly IMessenger messenger;
    private ToastViewModel? current;
    public Panel? hostPanel;

    public Toaster(IMessenger messenger)
    {
        this.messenger = messenger;
        this.messenger.Subscribe<ToastMessage.Show>(this.OnShow, withUiDispatch: true);
        this.messenger.Subscribe<ToastMessage.Dismiss>(this.OnDismiss, withUiDispatch: true);
    }

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
        => this.messenger.Publish(
            new ToastMessage.Show { Title = title, Message = message, Delay = dismissDelay, Level = toastLevel });

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

        ToastView? view = this.current.View;
        if (view is not null)
        {
            panel.Children.Remove(view);
        }

        this.current.Unbind();
        this.current = null;

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

        this.current?.Dismiss();
        this.current = new ToastViewModel(this);
        ToastView? view = this.current.CreateViewAndBind();
        if (view is not null)
        {
            panel.Children.Add(view);
        }

        this.current.Show(show.Title, show.Message, show.Delay, show.Level);
    }
}
