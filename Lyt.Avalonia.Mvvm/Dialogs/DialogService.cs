namespace Lyt.Avalonia.Mvvm.Dialogs;

public sealed class DialogService(IMessenger messenger, ILogger logger) : IDialogService
{
    private readonly IMessenger messenger = messenger;
    private readonly ILogger logger = logger;

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
        Action<object, bool> onClose,
        TParameters? parameters=null)
        where TDialog : UserControl, new()
        where TParameters : class
    {
        try
        {
            Panel panel = this.Guard(maybePanel);
            viewModel.CreateViewAndBind();
            viewModel.Initialize(onClose, parameters);
            this.ShowInternal(panel, viewModel.View);
        }
        catch (Exception ex)
        {
            this.logger.Error("Failed to launch dialog, exception thrown: \n" + ex.ToString());
            throw;
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
            if (this.modalUserControl is not null && this.modalHostControl is not null)
            {
                this.modalHostControl.ContentGrid.Children.Remove(this.modalUserControl);
            }

            if (this.modalHostPanel is not null && this.modalHostControl is not null)
            {
                this.modalHostPanel.IsHitTestVisible = false;
                this.modalHostPanel.Children.Remove(this.modalHostControl);
            }

            this.modalHostControl = null;
            this.modalHostPanel = null;
            this.modalUserControl = null;
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
        dialog.ZIndex = 999_999; 
        var host = new ModalHostControl(panel, (bool _) => { });
        panel.Children.Add(host);
        host.ContentGrid.Children.Add(dialog);

        this.messenger.Publish(new ModalMessage(ModalMessage.Modal.Enter));
        panel.IsHitTestVisible = true;
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

    #region LATER: Run a Modal Window 

    //public void Run<TDialog, TDialogParameters>(
    //    object maybePanel, Action<bool> onClose, TDialogParameters dialogParameters)
    //    where TDialog : IDialog<TDialogParameters>, new()
    //    where TDialogParameters : class
    //{
    //    try
    //    {
    //        if (maybePanel is not Panel panel)
    //        {
    //            this.logger.Error("Must provide a host panel");
    //            throw new InvalidOperationException("Must provide a host panel");
    //        }

    //        if (!typeof(TDialog).IsSubclassOf(typeof(UserControl)))
    //        {
    //            this.logger.Error("TDialog Must provide a type deriving from UserControl");
    //            throw new InvalidOperationException("TDialog Must provide a type deriving from UserControl");
    //        }

    //        this.RunInternal<TDialog, TDialogParameters>(panel, onClose, dialogParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        this.logger.Error("Failed to launch dialog, exception thrown: \n" + ex.ToString());
    //        throw;
    //    }
    //}

    //private void RunInternal<TDialog, TDialogParameters>(
    //    Panel panel, Action<bool> onClose, TDialogParameters? dialogParameters = null)
    //    where TDialog : IDialog<TDialogParameters>, new()
    //    where TDialogParameters : class
    //{
    //    var host = new ModalHostControl(panel, onClose);
    //    var modal = new TDialog { Host = host };
    //    if (modal is not UserControl userControl)
    //    {
    //        this.logger.Error("Failed to cast to UserControl");
    //        throw new InvalidOperationException("Failed to cast to UserControl");
    //    }

    //    if (dialogParameters is not null)
    //    {
    //        modal.Initialize(dialogParameters);
    //    }

    //    panel.Children.Add(host);
    //    host.ContentGrid.Children.Add(userControl);
    //    this.messenger.Publish(new ModalMessage(ModalMessage.Modal.Enter));
    //    panel.IsHitTestVisible = true;

    //}

    //public static bool Run<TDialog, TDialogParameters>(Window window, TDialogParameters? dialogParameters = null)
    //    where TDialog : UserControl, IDialog<TDialogParameters>, new()
    //    where TDialogParameters : class
    //{
    //    var host = new ModalHostWindow()
    //    {
    //        Owner = window,
    //        Width = window.ActualWidth,
    //        Height = window.ActualHeight,
    //    };

    //    TDialog modal = new TDialog { Host = host };
    //    if (dialogParameters is not null)
    //    {
    //        modal.Initialize(dialogParameters);
    //        PropertyInfo? imageSourceProperty = dialogParameters.GetType().GetProperty("ImageSource");
    //        if (imageSourceProperty is not null)
    //        {
    //            object? imageSourcePropertyValue = imageSourceProperty.GetValue(dialogParameters, null);
    //            if (imageSourcePropertyValue is string imageSourceUri)
    //            {
    //                if (!string.IsNullOrEmpty(imageSourceUri))
    //                {
    //                    try
    //                    {
    //                        BitmapImage source = new BitmapImage();
    //                        source.BeginInit();
    //                        source.UriSource = new Uri("pack://application:,,," + imageSourceUri);
    //                        source.EndInit();
    //                        host.BackgroundImage.Source = source;
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        // Silently swallow the exception if the image fails to load
    //                        Debug.WriteLine(ex);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    host.ContentGrid.Children.Add(modal);
    //    bool? _ = host.ShowDialog();
    //    return modal.DialogResult;
    //}

    #endregion Run a Window 
}
