namespace Lyt.Avalonia.Mvvm.Behaviors;

public class BehaviorEnabledUserControl : UserControl, ISupportBehaviors
{
    public List<object> Behaviors { get; private set; } = [];
}
