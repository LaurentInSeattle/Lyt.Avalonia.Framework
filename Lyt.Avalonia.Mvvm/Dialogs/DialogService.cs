using Avalonia.Controls;

namespace Lyt.Avalonia.Mvvm.Dialogs;

public sealed class DialogService(IMessenger messenger, ILogger logger) : IDialogService
{
    private readonly IMessenger messenger = messenger;
    private readonly ILogger logger = logger;

    private bool isClassHandlerRegistered;
    private bool modalHostPanelHitTestVisible; 
    private Panel? modalHostPanel;
    private ModalHostControl? modalHostControl;
    private UserControl? modalUserControl;

    public bool IsModal
        => this.modalHostPanel is not null || this.modalUserControl is not null || this.modalHostControl is not null;

    public void Confirm(object maybePanel, ConfirmActionParameters parameters)
    {
        try
        {
            Panel panel = this.Guard(maybePanel);
            var viewModel = new ConfirmActionViewModel(parameters);
            viewModel.CreateViewAndBind();
            this.ShowInternal(panel, viewModel.View);
        }
        catch (Exception ex)
        {
            this.logger.Error("Failed to launch dialog, exception thrown: \n" + ex.ToString());
            throw;
        }
    }

    public void Show<TDialog>(object maybePanel, TDialog dialog)
    {
        try
        {
            if (this.IsModal)
            {
                this.logger.Error("Already showing a modal");
                throw new InvalidOperationException("Already showing a modal");
            }

            if (maybePanel is not Panel panel)
            {
                this.logger.Error("Must provide a host panel");
                throw new InvalidOperationException("Must provide a host panel");
            }

            if (!typeof(TDialog).IsSubclassOf(typeof(UserControl)))
            {
                this.logger.Error("TDialog Must provide a type deriving from UserControl");
                throw new InvalidOperationException("TDialog Must provide a type deriving from UserControl");
            }

            if (dialog is UserControl userControl)
            {
                this.ShowInternal(panel, userControl);
            }
            else
            {
                this.logger.Error("TDialog Must provide a type deriving from UserControl");
                throw new InvalidOperationException("TDialog Must provide a type deriving from UserControl");
            }
        }
        catch (Exception ex)
        {
            this.logger.Error("Failed to launch dialog, exception thrown: \n" + ex.ToString());
            throw;
        }
    }

    public void RunModal<TDialog, TParameters>(
        object maybePanel,
        DialogBindable<TDialog, TParameters> viewModel,
        Action<object, bool>? onClose = null,
        TParameters? parameters = null)
        where TDialog : UserControl, new()
        where TParameters : class
    {
        try
        {
            Panel panel = this.Guard(maybePanel);
            viewModel.CreateViewAndBind();
            viewModel.Initialize(onClose, parameters);
            this.ShowInternal(panel, viewModel.View);
            if (!this.isClassHandlerRegistered)
            {
                ApplicationBase.MainWindow.AddHandler(
                    InputElement.KeyDownEvent, this.OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
                this.isClassHandlerRegistered = true;
            }
        }
        catch (Exception ex)
        {
            this.logger.Error("Failed to launch dialog, exception thrown: \n" + ex.ToString());
            throw;
        }
    }

    private void OnKeyDown(object? _, KeyEventArgs args)
    {
        if ((this.IsModal) &&
            (this.modalUserControl is not null) &&
            (this.modalUserControl.DataContext is Bindable bindable))
        {
            if ((args.Key == Key.Escape) || (args.Key == Key.Enter))
            {
                if (bindable.CanEscape && (args.Key == Key.Escape))
                {
                    args.Handled = true;
                    bindable.Cancel();
                }
                else if (bindable.CanEnter && (args.Key == Key.Enter))
                {
                    args.Handled = true;
                    bindable.TrySaveAndClose();
                }

                // Here; We could have Key == Enter and CanEnter == false 
                // So dont move here: args.Handled = true; 
                // or else Enters are ignored 
            }
            // else : NOT handled or else cant type anything :( 
        }
    }

    public void Dismiss()
    {
        if (this.modalHostPanel is null && this.modalUserControl is null && this.modalHostControl is null)
        {
            this.logger.Warning("DialogService: Nothing to dismiss.");
            return;
        }

        if (this.modalHostPanel is null || this.modalUserControl is null || this.modalHostControl is null)
        {
            this.logger.Warning("DialogService: Inconsistent state, trying to recover and dismiss.");
        }

        try
        {
            // #1 - Remove user control from the modal host control 
            if (this.modalUserControl is not null && this.modalHostControl is not null)
            {
                bool removedDialog = this.modalHostControl.ContentGrid.Children.Remove(this.modalUserControl);
                if (!removedDialog)
                {
                    if (Debugger.IsAttached) { Debugger.Break(); }
                    this.logger.Warning("Failed to remove user dialog from modal host ");
                }
            }

            // #2 - Remove host control from the host panel 
            if (this.modalHostPanel is not null && this.modalHostControl is not null)
            {
                bool removedHost = this.modalHostPanel.Children.Remove(this.modalHostControl);
                if (!removedHost)
                {
                    if (Debugger.IsAttached) { Debugger.Break(); }
                    this.logger.Warning("Failed to remove modal host from host panel");
                }
            }

            // #3 - Restore host panel visibility state 
            if (this.modalHostPanel is not null)
            {
                this.modalHostPanel.IsHitTestVisible = this.modalHostPanelHitTestVisible;
            }

            // #4 - Unhook keyboard events 
            if (this.isClassHandlerRegistered)
            {
                ApplicationBase.MainWindow.RemoveHandler(InputElement.KeyDownEvent, this.OnKeyDown);
                this.isClassHandlerRegistered = false;
            }

            this.modalHostControl = null;
            this.modalUserControl = null;
            this.modalHostPanel = null;
        }
        catch (Exception ex)
        {
            this.logger.Error("Failed to dismiss dialog, exception thrown: \n" + ex.ToString());
            throw;
        }
        finally
        {
            this.messenger.Publish(new ModalMessage(ModalMessage.Modal.Leave));
        }
    }

    private void ShowInternal(Panel panel, UserControl dialog)
    {
        this.modalHostPanelHitTestVisible = panel.IsHitTestVisible;
        panel.IsHitTestVisible = true; 
        var host = new ModalHostControl();
        panel.Children.Add(host);
        host.ContentGrid.Children.Add(dialog);
        this.messenger.Publish(new ModalMessage(ModalMessage.Modal.Enter));
        this.modalHostPanel = panel;
        this.modalHostControl = host;
        this.modalUserControl = dialog;
    }

    private Panel Guard(object maybePanel)
    {
        if (this.IsModal)
        {
            // You should have checked if the service was modal, and if so, first
            // invoke 'Dismiss' before launching a new dialog 
            this.logger.Error("Already showing a modal");
            throw new InvalidOperationException("Already showing a modal");
        }

        if (maybePanel is not Panel panel)
        {
            this.logger.Error("Must provide a host panel");
            throw new InvalidOperationException("Must provide a host panel");
        }

        return panel;
    }
}
