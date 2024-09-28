namespace Lyt.Avalonia.Mvvm.Behaviors;

public sealed class BehaviorCollection : AvaloniaList<IBehavior>
{
    /// <summary> Gets the <see cref="AvaloniaObject"/> to which the <see cref="BehaviorCollection"/> is attached. </summary>
    public AvaloniaObject? AssociatedObject { get; private set; }

    /// <summary> Attaches the collection of behaviors to the specified <see cref="AvaloniaObject"/>. </summary>
    /// <param name="associatedObject">The <see cref="AvaloniaObject"/> to which to attach.</param>
    /// <exception cref="InvalidOperationException">The <see cref="BehaviorCollection"/> is already attached to a different <see cref="AvaloniaObject"/>.</exception>
    public void Attach(AvaloniaObject associatedObject)
    {
        if (object.Equals(associatedObject, this.AssociatedObject))
        {
            return;
        }

        if (this.AssociatedObject is not null)
        {
            throw new InvalidOperationException(
                "An instance of a behavior cannot be attached to more than one object at a time.");
        }

        this.AssociatedObject = associatedObject;

        foreach (IBehavior behavior in this)
        {
            behavior.Attach(this.AssociatedObject);
        }
    }

    /// <summary> Detaches the collection of behaviors from the <see cref="BehaviorCollection.AssociatedObject"/>. </summary>
    public void Detach()
    {
        foreach (IBehavior behavior in this)
        {
            behavior.Detach();
        }

        this.AssociatedObject = null;
    }
}
