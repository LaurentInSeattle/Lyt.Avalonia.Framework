namespace Lyt.Avalonia.Mvvm.Core;

public class DialogBindable<TControl, TParameters> : Bindable<TControl>
    where TControl : Control, new()
    where TParameters : class
{
    protected readonly IDialogService dialogService;

    protected Action<DialogBindable<TControl, TParameters>, bool>? onClose;

    protected TParameters? parameters;

    public DialogBindable() : base()
        => this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();

    public DialogBindable(bool disablePropertyChangedLogging = false, bool disableAutomaticBindingsLogging = false)
        : base(disablePropertyChangedLogging, disableAutomaticBindingsLogging)
        => this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();

    public DialogBindable(TControl control) : base()
    {
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();
        this.Bind(control);
    }

    public virtual void Initialize(
        Action<DialogBindable<TControl, TParameters>, bool> onClose,
        TParameters? parameters)
    {
        this.onClose = onClose;
        this.parameters = parameters;
    }

    public override bool Validate() => true;

    public override bool TrySaveAndClose()
    {
        bool isValid = this.Validate();
        if (isValid)
        {
            this.onClose?.Invoke(this, true);
            this.dialogService.Dismiss();
        }

        return isValid;
    }

    public override void Cancel()
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }
}
