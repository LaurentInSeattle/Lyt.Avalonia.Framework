namespace Lyt.StateMachine;

public sealed record class StateMachineDefinition<TState, TTrigger, TTag>(
    TState InitialState,
    List<StateDefinition<TState, TTrigger, TTag>> StateDefinitions,
    Action<TState, TState>? OnStateChanged = null)
        where TState : struct, Enum
        where TTrigger : struct, Enum; 
