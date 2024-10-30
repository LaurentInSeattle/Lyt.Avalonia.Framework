namespace Lyt.Avalonia.Mvvm.Dialogs;

public sealed record class ModalMessage(ModalMessage.Modal State)
{
    public enum Modal 
    {
        Enter,
        Leave,
    }
}
