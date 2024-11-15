namespace Lyt.Avalonia.Orchestrator;

public sealed record class NavigationMessage(Bindable Activated, Bindable? Deactivated = null); 