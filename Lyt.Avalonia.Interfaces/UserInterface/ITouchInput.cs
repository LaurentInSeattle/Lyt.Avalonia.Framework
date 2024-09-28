namespace Lyt.Avalonia.Interfaces.UserInterface;

public interface ITouchInput
{
    object? Host { get; set; }

    void ShowNumericKeypad (object target, string title);

    // void ShowKeyboard(object target, string title);

    void Dismiss();
}
