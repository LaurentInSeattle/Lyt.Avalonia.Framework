namespace Lyt.StateMachine;

public sealed record class TriggerDefinition<TState, TTrigger>(
    TTrigger Trigger, TState ToState, Func<bool>? Validator = null)
        where TState : struct, Enum
        where TTrigger : struct, Enum;