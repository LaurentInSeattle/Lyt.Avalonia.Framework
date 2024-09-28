namespace Lyt.Avalonia.Mvvm.Behaviors;

public abstract class Behavior : AvaloniaObject, IBehavior
{
    /// <summary> Gets the AvaloniaObject to which this behavior is attached. </summary>
    public AvaloniaObject? AssociatedObject { get; private set; }

    /// <summary> Attaches this behavior to the specified AvaloniaObject. </summary>
    /// <param name="associatedObject">The <see cref="AvaloniaObject"/> to which to attach.</param>
    public void Attach(AvaloniaObject associatedObject)
    {
        if (object.Equals(associatedObject, this.AssociatedObject))
        {
            return;
        }

        if (this.AssociatedObject is not null)
        {
            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "An instance of a behavior cannot be attached to more than one object at a time."));
        }

        this.AssociatedObject = associatedObject;
        this.OnAttached();
    }

    /// <summary> Detaches the behaviors from the <see cref="AssociatedObject"/>. </summary>
    public void Detach()
    {
        this.OnDetaching();
        this.AssociatedObject = null;
    }

    /// <summary> Called after the behavior is attached to the <see cref="AssociatedObject"/> </summary>
    /// <remarks> Override this to hook up functionality to the <see cref="AssociatedObject"/> </remarks>
    protected abstract void OnAttached();

    /// <summary> Called when the behavior is being detached from its <see cref="AssociatedObject"/> </summary>
    /// <remarks> Override this to unhook functionality from the <see cref="AssociatedObject"/> </remarks>
    protected abstract void OnDetaching();
}
