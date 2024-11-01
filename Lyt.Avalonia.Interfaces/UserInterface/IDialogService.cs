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
}
