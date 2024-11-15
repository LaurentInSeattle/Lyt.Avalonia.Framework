namespace Lyt.StateMachine;

public sealed record class StateDefinition<TState, TTrigger, TTag>(
    TState State,
    TTag? Tag, 
    Action<TState>? OnEnter,
    Action<TState>? OnLeave,
    Action<TState>? OnTimeout,
    TimeoutDefinition<TState>? TimeoutDefinition,
    List<TriggerDefinition<TState, TTrigger>>? TriggerDefinitions)
        where TState : struct, Enum
        where TTrigger : struct, Enum; 
