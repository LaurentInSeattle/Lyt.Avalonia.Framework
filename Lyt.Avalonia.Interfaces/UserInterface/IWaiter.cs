namespace Lyt.Avalonia.Interfaces.UserInterface;

public interface IWaiter
{
    object? Host { get; set; }

    void Show(string message);

    void Dismiss();
}
