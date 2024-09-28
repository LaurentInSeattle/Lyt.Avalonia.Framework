namespace Lyt.StateMachine;

public sealed class StateDefinition<TState, TTrigger, TTag>(
    TState state,
    TTag? tag, 
    Action<TState>? onEnter,
    Action<TState>? onLeave,
    Action<TState>? onTimeout,
    TimeoutDefinition<TState>? timeoutDefinition,
    List<TriggerDefinition<TState, TTrigger>>? triggerDefinitions)
        where TState : struct, Enum
        where TTrigger : struct, Enum
{
    public TState State { get; private set; } = state;

    public TTag? Tag { get; private set; } = tag;

    public Action<TState>? OnEnter { get; private set; } = onEnter;

    public Action<TState>? OnLeave { get; private set; } = onLeave;

    public Action<TState>? OnTimeout { get; private set; } = onTimeout;

    public List<TriggerDefinition<TState, TTrigger>>? TriggerDefinitions { get; private set; } = triggerDefinitions;

    public TimeoutDefinition<TState>? TimeoutDefinition { get; private set; } = timeoutDefinition;
}
