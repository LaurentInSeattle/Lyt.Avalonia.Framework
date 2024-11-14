namespace Lyt.Avalonia.Orchestrator;

public class WorkflowPage<TState, TTrigger, TControl> : WorkflowPage<TState, TTrigger>
    where TState : struct, Enum
    where TTrigger : struct, Enum
    where TControl : Control, new()
{
    public TControl View
        => this.Control as TControl ?? throw new InvalidOperationException("View is null");
}
