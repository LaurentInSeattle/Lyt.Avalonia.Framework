using System.Diagnostics;

namespace Lyt.Avalonia.Interfaces.Logger; 

public sealed class BasicLogger : ILogger
{
    private static DateTime last = DateTime.Now;

    public bool BreakOnError { get; set; } = true;

    public void Debug(string message) => System.Diagnostics.Debug.WriteLine(ShortTimeString() + message);

    public void Info(string message) => Trace.TraceInformation(ShortTimeString() + message);

    public void Warning(string message) => Trace.TraceWarning(ShortTimeString() + message);

    public void Error(string message)
    {
        if (this.BreakOnError && Debugger.IsAttached)
        {
            Debugger.Break();
        }

        Trace.TraceError(ShortTimeString() + message);
    }

    public void Fatal(string message)
    {
        Trace.TraceError(message);
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(message);
    }

    public static string ShortTimeString()
    {
        DateTime now = DateTime.Now;
        int deltaMs = (int)(now - last).TotalMilliseconds;
        string result = string.Format("{0}::{1} ({2}ms) - ", now.Second, now.Millisecond, deltaMs);
        last = now;
        return result;
    }
}
