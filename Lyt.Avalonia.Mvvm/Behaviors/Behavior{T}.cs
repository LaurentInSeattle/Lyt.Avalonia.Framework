namespace Lyt.Avalonia.Mvvm.Behaviors;

public abstract class Behavior<T> : Behavior where T : AvaloniaObject
{
    /// <summary> Gets the T Object to which this behavior is attached. </summary>
    public new T? AssociatedObject => base.AssociatedObject as T; 

    public void Attach(T associatedObject) => base.Attach(associatedObject);

    public new void Attach(AvaloniaObject associatedObject)
    {
        if (associatedObject is not T)
        {
            throw new ArgumentException("Not type T ");
        }

        base.Attach(associatedObject);
    }
}
