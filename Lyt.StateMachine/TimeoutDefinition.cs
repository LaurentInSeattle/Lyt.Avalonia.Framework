namespace Lyt.StateMachine;

public sealed class TimeoutDefinition<TState>(TState toState, int valueMillisecs)
        where TState : struct, Enum
{
    public TState ToState { get; set; } = toState;

    public int ValueMillisecs { get; private set; } = valueMillisecs;
}
