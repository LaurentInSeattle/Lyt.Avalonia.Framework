namespace Lyt.StateMachine;

public sealed class TriggerDefinition<TState, TTrigger>(TTrigger trigger, TState toState, Func<bool>? validator)
        where TState : struct, Enum
        where TTrigger : struct, Enum
{
    public TTrigger Trigger { get; set; } = trigger;

    public TState ToState { get; set; } = toState;

    public Func<bool>? Validator { get; private set; } = validator;
}
