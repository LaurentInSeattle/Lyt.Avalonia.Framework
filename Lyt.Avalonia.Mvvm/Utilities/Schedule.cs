namespace Lyt.Avalonia.Mvvm.Utilities;

public static class Schedule
{
    public static void OnUiThread(int delay, Action action, DispatcherPriority priority)
        => Task.Run(async () =>
            {
                await Task.Delay(delay);
                Dispatch.OnUiThread(action, priority);
            });

    public static void OnUiThread<TArgs>(
        int delay, Action<TArgs> action, TArgs args, DispatcherPriority priority)
        => Task.Run(async () =>
            {
                await Task.Delay(delay);
                Dispatch.OnUiThread(action, args, priority);
            });
}
