namespace Lyt.Avalonia.Framework.TestApp.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly TimingModel timingModel;

    public ShellViewModel()
    {
        this.timingModel = ApplicationBase.GetModel<TimingModel>();
        this.timingModel.SubscribeToUpdates(this.OnTimingModelUpdated, withUiDispatch: true);
        this.TickCount = "Hello Avalonia!";
        this.IsTicking = string.Empty;
        this.OnStartStopCommand = new Command(this.OnStartStop);
        this.OnSvgCommand = new Command(this.OnSvg);
    }

    public WorkflowManager<WorkflowState, WorkflowTrigger>? Workflow { get; private set; }

    private void OnStartStop(object? _)
    {
        if (this.timingModel.IsTicking)
        {
            this.timingModel.Stop();
        }
        else
        {
            this.timingModel.Start();
        }

        // Not good 
        // This can only be caught in the main top level try catch 
        //
        // throw new NotImplementedException("wtf");
    }

    private void OnSvg(object? _)
    {
        string source = this.View!.callIcon.Source;
        this.View.callIcon.Source = source == "call" ? "call_end" : "call";
        //var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        //var folder = "assets";
        //var svgFolderPath = Path.Combine(rootFolder, folder);
        //var targetFolder = "ExtractedSvg";
        //var targetFolderPath = Path.Combine(rootFolder, targetFolder);
        //if (Directory.Exists(svgFolderPath))
        //{
        //    // Enumerates files 
        //    var enumerationOptions = new EnumerationOptions()
        //    {
        //        IgnoreInaccessible = true,
        //        RecurseSubdirectories = true,
        //        MatchType = MatchType.Simple,
        //        MaxRecursionDepth = 2,
        //    };
        //    var files = Directory.EnumerateFiles(svgFolderPath, "*.svg", enumerationOptions);
        //    foreach (string file in files)
        //    {
        //        if ( !file.Contains("regular"))
        //        {
        //            continue;
        //        }

        //        if (!file.Contains("24"))
        //        {
        //            continue;
        //        }

        //        FileInfo fi = new FileInfo(file);
        //        var target = Path.Combine(targetFolderPath, fi.Name);
        //        target = target.Replace("ic_fluent_", "");
        //        target = target.Replace("_24", "");
        //        target = target.Replace("_regular", "");

        //        File.Copy(file, target, true);
        //    }
        //}
    }

    protected override async void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.timingModel.Start();
        this.SetupWorkflow();
        if (this.Workflow is not null)
        {
            await this.Workflow.Initialize();
            _ = this.Workflow.Start();
        }
    }

    private void OnTimingModelUpdated(ModelUpdateMessage _)
    {
        int ticks = this.timingModel.TickCount;
        this.TickCount = string.Format("Ticks: {0}", ticks);
        bool modelIsTicking = this.timingModel.IsTicking;
        this.IsTicking = modelIsTicking ? "Ticking" : "Stopped";
        this.ButtonText = modelIsTicking ? "Stop" : "Start";
        var profiler = App.GetRequiredService<IProfiler>();
        profiler.MemorySnapshot();
        if (this.Workflow is not null)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var fireAndForget = this.Workflow.Next();
#pragma warning restore IDE0059 
        }
    }

    private void SetupWorkflow()
    {
        StateDefinition<WorkflowState, WorkflowTrigger, Bindable> Create<TViewModel, TView>(
            WorkflowState state, WorkflowTrigger trigger, WorkflowState target)
            where TViewModel : Bindable, new()
            where TView : Control, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.Bind(new TView());
            if (vm is WorkflowPage<WorkflowState, WorkflowTrigger> page)
            {
                page.State = state;
                page.Title = state.ToString();
                return
                    new StateDefinition<WorkflowState, WorkflowTrigger, Bindable>(
                        state, page, null, null, null, null,
                        [
                            new TriggerDefinition<WorkflowState, WorkflowTrigger> ( trigger, target , null )
                        ]);
            }
            else
            {
                string msg = "View is not a Workflow Page";
                this.Logger.Error(msg);
                throw new Exception(msg);
            }
        }

        var startup = Create<StartupViewModel, StartupView>(WorkflowState.Startup, WorkflowTrigger.Ready, WorkflowState.Login);
        var login = Create<LoginViewModel, LoginView>(WorkflowState.Login, WorkflowTrigger.LoggedIn, WorkflowState.Select);
        var select = Create<SelectViewModel, SelectView>(WorkflowState.Select, WorkflowTrigger.Selected, WorkflowState.Process);
        var process = Create<ProcessViewModel, ProcessView>(WorkflowState.Process, WorkflowTrigger.Complete, WorkflowState.Login);

        var stateMachineDefinition =
            new StateMachineDefinition<WorkflowState, WorkflowTrigger, Bindable>(
                WorkflowState.Startup, // Initial state
                [ 
                    // List of state definitions
                    startup, login , select, process,
                ]);

        this.Workflow =
            new WorkflowManager<WorkflowState, WorkflowTrigger>(
                this.Logger, this.Messenger, this.View!.WorkflowContent!, stateMachineDefinition);
    }

    public ICommand? OnStartStopCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public ICommand? OnSvgCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public string? ButtonText { get => this.Get<string>(); set => this.Set(value); }

    public string? TickCount { get => this.Get<string>(); set => this.Set(value); }

    public string? IsTicking { get => this.Get<string>(); set => this.Set(value); }
}
