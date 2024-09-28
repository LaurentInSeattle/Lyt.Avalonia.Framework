namespace Lyt.Avalonia.Model;

public class ApplicationModelBase(IProfiler profiler, ILogger logger, IApplicationBase application) : IApplicationModel
{
    protected readonly IProfiler profiler = profiler;
    protected readonly ILogger logger = logger;
    protected readonly IApplicationBase application = application;

    public async Task Initialize()
    {
        try
        {
            foreach (var model in this.application.GetModels())
            {
                await model.Initialize();
            }

            this.logger.Info("Software initialization complete");
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw new ApplicationException("Failed to initialize models.", ex);
        }

        _ = Task.Run(async() =>
        {
            try
            {
                // Delay until the app has fully started up before looking up the perf counters 
                // Do not collect, again to avoid a slow startup.
                await Task.Delay(250);
                this.profiler.MemorySnapshot("All Models Initialized", withGCCollect: false);
            }
            catch (Exception ex)
            {
                // Should never fail here
                if (Debugger.IsAttached) { Debugger.Break(); }
                this.logger.Error(ex.ToString());
                throw new ApplicationException("Failed to cleanup on startup.", ex);
            }
        }); 
    }

    public async Task Shutdown()
    {
        try
        {
            foreach (var model in this.application.GetModels())
            {
                await model.Shutdown();
            }
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw;
        }
    }
}
