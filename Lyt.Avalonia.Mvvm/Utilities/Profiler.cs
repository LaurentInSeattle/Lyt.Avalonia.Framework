namespace Lyt.Avalonia.Mvvm.Utilities;

public sealed class Profiler(ILogger logger) : IProfiler
{
    private readonly ILogger logger = logger; 
    private bool isTimingStarted;
    private Stopwatch? stopwatch;

    public async Task FullGcCollect(int delay = 0)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GCNotificationStatus status = GC.WaitForFullGCComplete();
        if (Debugger.IsAttached && status != GCNotificationStatus.NotApplicable)
        {
            this.logger.Info("GC Status: " + status.ToString());
        }

        if (delay > 0)
        {
            await Task.Delay(delay);
        }
    }

    public int[] CollectionCounts()
        => [GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2)];

    // Implemented only on Windows 
    public void MemorySnapshot(string comment = "", bool withGCCollect = true)
    {
        if (OperatingSystem.IsWindows())
        {
            this.WindowsMemorySnapshot(comment, withGCCollect);
        }
    }

    [Conditional("DEBUG")]
    public void Track(
        string message,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var frame = new StackFrame(1);
        var method = frame.GetMethod();
        if (method is null)
        {
            return;
        }

        string typeName = method.DeclaringType!.Name;
        string memberName = method.Name;
        this.logger.Info(
            string.Format(
                "***** {0} -- From {1}.{2} in {3}, line {4}",
                message, typeName, memberName, sourceFilePath, sourceLineNumber));
    }

    [Conditional("DEBUG")]
    public void StartTiming()
    {
        if (this.isTimingStarted)
        {
            this.logger.Warning("Timing already started");
            return;
        }

        this.isTimingStarted = true;
        this.stopwatch = Stopwatch.StartNew();
    }

    [Conditional("DEBUG")]
    public void EndTiming(string comment)
    {
        if (!this.isTimingStarted || (this.stopwatch == null))
        {
            this.logger.Warning("Timing not started");
            return;
        }

        this.isTimingStarted = false;
        this.stopwatch.Stop();
        float millisecs = (float)Math.Round(this.stopwatch.Elapsed.TotalMilliseconds, 1);
        this.stopwatch = null;
        string rightNow = DateTime.Now.ToLocalTime().ToLongTimeString();
        this.logger.Info("***** " + comment + " - Timing: " + millisecs.ToString("F1") + " ms.  - at: " + rightNow);
    }

    [SupportedOSPlatform("windows")]
    private async void WindowsMemorySnapshot(string comment, bool withGCCollect)
    {
        if (withGCCollect)
        {
            await this.FullGcCollect(0);
        }

        var currentProcess = Process.GetCurrentProcess();
        string processName = currentProcess.ProcessName;
        var ctr1 = new PerformanceCounter("Process", "Private Bytes", processName);
        float privateBytes = ctr1.NextValue();
        ctr1.Dispose();
        int[] collections = this.CollectionCounts();

        string rightNow = DateTime.Now.ToLocalTime().ToLongTimeString();
        string withCollect = withGCCollect ? ", with GC Collect " : " ";
        int megaPrivateBytes = (int)((privateBytes + 512 * 1024) / (1024 * 1024));
        string part1 = "***** Memory Snapshot: " + comment + "  at: " + rightNow + withCollect;
        string part2 = "Private Bytes:  " + megaPrivateBytes.ToString() + " MB.";
        string part3 = string.Format(" Gen. 0: {0} - 1: {1} - 2: {2}", collections[0], collections[1], collections[2]);
        this.logger.Info(part1 + "  -  " + part2 + "  -  " + part3);
    }
}
