namespace Lyt.Avalonia.Orchestrator;

public sealed class NavigationMessage(Bindable activated, Bindable? deactivated)
{
    public Bindable Activated { get; private set; } = activated;

    public Bindable? Deactivated { get; private set; } = deactivated;
}
