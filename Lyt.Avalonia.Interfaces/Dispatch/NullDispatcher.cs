namespace Lyt.Avalonia.Interfaces.Dispatch;

public class NullDispatcher : IDispatch
{
    public void OnUiThread(Action action) => action();

    public void OnUiThread<TArgs>(Action<TArgs> action, TArgs args) => action(args);
}
