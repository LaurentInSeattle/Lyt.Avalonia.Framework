namespace Lyt.Avalonia.Interfaces.Dispatch;

public interface IDispatch
{
    void OnUiThread(Action action);

    void OnUiThread<TArgs>(Action<TArgs> action, TArgs args); 
}
