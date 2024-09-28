namespace Lyt.Avalonia.Mvvm.Utilities;

public sealed class Logger : ILogger
{
    private static DateTime last = DateTime.Now;

    public void Debug(string message) => System.Diagnostics.Debug.WriteLine(ShortTimeString() + message);

    public void Info(string message) => Trace.TraceInformation(ShortTimeString() + message);

    public void Warning(string message) => Trace.TraceWarning(ShortTimeString() + message);

    public void Error(string message) => Trace.TraceError(ShortTimeString() + message);

    public void Fatal(string message)
    {
        Trace.TraceError(message);
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(message);
    }

    public static string ShortTimeString()
    {
        DateTime now = DateTime.Now;
        int deltaMs = (int) (now - last).TotalMilliseconds;
        string result = string.Format("{0}::{1} ({2}ms) - ", now.Second, now.Millisecond, deltaMs);
        last = now;
        return result;
    }
}
