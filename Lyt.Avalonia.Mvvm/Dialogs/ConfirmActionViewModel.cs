namespace Lyt.Avalonia.Mvvm.Dialogs; 

public class ConfirmActionViewModel : Bindable<ConfirmActionView>
{
    private readonly IDialogService dialogService;
    private readonly Action<bool> onConfirm; 

    public ConfirmActionViewModel(ConfirmActionParameters parameters)
    {
        this.dialogService = ApplicationBase.GetRequiredService<IDialogService>();
        this.Title = parameters.Title;
        this.Message = parameters.Message;
        this.ActionVerb = parameters.ActionVerb;
        this.ColorLevel = parameters.InformationLevel.ToBrush();
        if (parameters.OnConfirm is not null)
        {
            this.onConfirm = parameters.OnConfirm;
        }
        else
        {
            throw new ArgumentException("No callback delegate for confirming action"); 
        } 

        this.ActionCommand = new Command(this.OnAction);
        this.DismissCommand = new Command(this.OnDismiss);
    }

    protected override void OnViewLoaded ( )
        // Need to figure out why we need to do this !!!
        => this.View.Icon.Foreground = this.ColorLevel;

    private void OnAction(object? _)
    {
        this.onConfirm(true);
        this.dialogService.Dismiss();   
    }

    private void OnDismiss(object? _)
    {
        this.onConfirm(false);
        this.dialogService.Dismiss();   
    }

    public string Title { get => this.Get<string>()!; set => this.Set(value); }

    public string Message { get => this.Get<string>()!; set => this.Set(value); }

    public string ActionVerb { get => this.Get<string>()!; set => this.Set(value); }

    public SolidColorBrush ColorLevel { get => this.Get<SolidColorBrush>()!; set => _ = this.Set(value); }

    public ICommand DismissCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ActionCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

}
