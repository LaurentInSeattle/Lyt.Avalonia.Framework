﻿namespace Lyt.Avalonia.Mvvm.Behaviors;

/// <summary> Allows extending functionality to any object, including Bindable. </summary>
/// <typeparam name="TObject">The type of the associated object.</typeparam>
public abstract class BehaviorBase<TObject> where TObject : class, ISupportBehaviors
{
    /// <summary> Gets the TObject to which this behavior is attached. </summary>
    public TObject? AssociatedObject { get; private set; }

    /// <summary> Attaches this behavior to the specified TObject. </summary>
    /// <param name="associatedObject">The TObject to which to attach.</param>
    public void Attach(TObject associatedObject)
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
        this.AssociatedObject.Behaviors.Add(this);
        this.OnAttached();
    }

    /// <summary> Detaches the behaviors from its AssociatedObject. </summary>
    public void Detach()
    {
        this.OnDetaching();
        if (this.AssociatedObject is null)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture, "No attached object."));
        }

        this.AssociatedObject.Behaviors.Remove(this);
        this.AssociatedObject = null;
    }

    /// <summary> Called after the behavior is attached to the AssociatedObject. </summary>
    /// <remarks> Override this to hook up functionality to the AssociatedObject. </remarks>
    protected abstract void OnAttached();

    /// <summary> Called when the behavior is being detached from its AssociatedObject. </summary>
    /// <remarks> Override this to unhook functionality from the AssociatedObject. </remarks>
    protected abstract void OnDetaching();
}