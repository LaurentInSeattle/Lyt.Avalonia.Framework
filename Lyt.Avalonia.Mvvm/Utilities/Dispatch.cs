namespace Lyt.Avalonia.Mvvm.Utilities;

public static class Dispatch
{
    // Sadly Action cannot be used as an extension method type...
    public static void OnUiThread(Action action, DispatcherPriority priority = default)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            // We are already on the UI thread, no need to invoke.
            action();
        }
        else
        {
            Dispatcher.UIThread.Post(action, priority);
        }
    }

    public static void OnUiThread<TArgs>(Action<TArgs> action, TArgs args, DispatcherPriority priority = default)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            // We are already on the UI thread, no need to invoke.
            action(args);
        }
        else
        {
            Dispatcher.UIThread.Post((Action)delegate { action(args); }, priority);
        }
    }
}
