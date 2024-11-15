namespace Lyt.StateMachine;

public sealed record class TimeoutDefinition<TState>(TState ToState, int ValueMillisecs)
        where TState : struct, Enum;
