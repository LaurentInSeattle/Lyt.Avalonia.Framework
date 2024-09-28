namespace Lyt.Avalonia.Orchestrator;

public class WorkflowPage<TState, TTrigger> : Bindable
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public virtual TState State { get; set; } = default;

    public string? Title { get => this.Get<string>(); set => this.Set(value); }

    public virtual Task OnInitialize() => Task.CompletedTask;

    public virtual Task OnShutdown() => this.OnDeactivateAsync(default);

    public virtual Task OnActivateAsync(TState fromState) => Task.CompletedTask;

    public virtual Task OnDeactivateAsync(TState toState) => Task.CompletedTask;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
    public WorkflowManager<TState, TTrigger> WorkflowManager { get; set; }
#pragma warning restore CS8618 
}
