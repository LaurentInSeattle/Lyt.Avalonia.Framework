namespace Lyt.Avalonia.Mvvm.Behaviors;

/// <summary> 
/// Tricky ! Seems to be doing nothing but...
/// Behaviors attached to non UI object (typically Bindable{T}) need to store them or 
/// else they could be garbage collected and only provide weak reference when messaging.
/// (Behaviors for controls use a dependency property, also see Interaction class.)
/// </summary>
public interface ISupportBehaviors
{
    List<object> Behaviors { get; }
}