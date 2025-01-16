namespace Lyt.Avalonia.Mvvm.Toasting;

public sealed class ToastMessage
{
    public sealed record class Show(
         string Title = "", string Message = "", 
         int Delay = 10_000, InformationLevel Level = InformationLevel.Info) { }

    public sealed class Dismiss { }

    public sealed class OnDismiss { }
}