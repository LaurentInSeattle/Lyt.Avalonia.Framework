namespace Lyt.Avalonia.Interfaces.UserInterface;

public interface IToaster
{
    object? Host { get; set; }

    object? View { get; }

    void Show(
        string title, string message, int dismissDelay = 10, 
        InformationLevel toastLevel = InformationLevel.Info);

    void Dismiss();
}
