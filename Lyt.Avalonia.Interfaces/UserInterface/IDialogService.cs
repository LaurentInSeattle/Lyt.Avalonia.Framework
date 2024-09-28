namespace Lyt.Avalonia.Interfaces.UserInterface;

public sealed class ConfirmActionParameters
{
    public string Title { get; set; } = "Untitled";

    public string Message { get; set; } = "No message provided.";

    public string ActionVerb { get; set; } = "Go!";

    public Action<bool>? OnConfirm { get; set; }

    public InformationLevel InformationLevel { get; set; } = InformationLevel.Info;
}

public interface IDialogService
{
    bool IsModal { get; }

    void Confirm(object maybePanel, ConfirmActionParameters parameters); 

    void Show<TDialog>(object panel, TDialog dialog);

    void Dismiss(); 

    void Run<TDialog, TDialogParameters>(object panel, Action<bool> onClose, TDialogParameters dialogParameters)
        where TDialog : IDialog<TDialogParameters>, new()
        where TDialogParameters : class; 
}

public interface IDialog<TDialogParameters, TData> : IDialog<TDialogParameters>
{
    TData DialogData { get; }
}

public interface IDialog<TDialogParameters>
{
    // Meant to be a ModalHostControl control but...
    // is left as an object to avoid taking a dependency on the UI framework
    object? Host { get; set; }

    bool DialogResult { get; }

    void Initialize(TDialogParameters dialogParameters);
}
