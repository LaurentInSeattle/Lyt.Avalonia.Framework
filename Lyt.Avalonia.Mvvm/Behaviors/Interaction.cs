namespace Lyt.Avalonia.Mvvm.Behaviors;

/// <summary> Defines a <see cref="BehaviorCollection"/> attached property . </summary>
public sealed class Interaction
{
    static Interaction()
    {
        BehaviorsProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<BehaviorCollection?>>(BehaviorsChanged));
    }

    /// <summary> Gets or sets the <see cref="BehaviorCollection"/> associated with a specified object. </summary>
    public static readonly AttachedProperty<BehaviorCollection?> BehaviorsProperty =
        AvaloniaProperty.RegisterAttached<Interaction, AvaloniaObject, BehaviorCollection?>("Behaviors");

    /// <summary> Gets the <see cref="BehaviorCollection"/> associated with a specified object. </summary>
    /// <param name="obj">The <see cref="AvaloniaObject"/> from which to retrieve the <see cref="BehaviorCollection"/>.</param>
    /// <returns>A <see cref="BehaviorCollection"/> containing the behaviors associated with the specified object.</returns>
    public static BehaviorCollection GetBehaviors(AvaloniaObject obj)
    {
        var behaviorCollection = obj.GetValue(BehaviorsProperty);
        if (behaviorCollection is null)
        {
            behaviorCollection = [];
            obj.SetValue(BehaviorsProperty, behaviorCollection);
        }

        return behaviorCollection;
    }

    /// <summary> Sets the <see cref="BehaviorCollection"/> associated with a specified object. </summary>
    /// <param name="obj">The <see cref="AvaloniaObject"/> on which to set the <see cref="BehaviorCollection"/>.</param>
    /// <param name="value">The <see cref="BehaviorCollection"/> associated with the object.</param>
    public static void SetBehaviors(AvaloniaObject obj, BehaviorCollection? value) 
        => obj.SetValue(BehaviorsProperty, value);

    private static void BehaviorsChanged(AvaloniaPropertyChangedEventArgs<BehaviorCollection?> e)
    {
        var oldCollection = e.OldValue.GetValueOrDefault();
        var newCollection = e.NewValue.GetValueOrDefault();
        if (oldCollection == newCollection)
        {
            return;
        }

        if (oldCollection is { AssociatedObject: not null })
        {
            oldCollection.Detach();
        }

        if (newCollection is not null)
        {
            newCollection.Attach(e.Sender);
        }
    }
}