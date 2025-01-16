namespace Lyt.Avalonia.Controls;

public delegate void RoutedEventDelegate(object sender, RoutedEventArgs rea);

public static class Utilities
{
    /// <summary> Gets a control </summary>
    /// <typeparam name="T">Type of the Control to get</typeparam>
    /// <param name="e">The TemplateAppliedEventArgs</param>
    /// <param name="name">The Name of the Control to return</param>
    /// <returns>a control with the indicated params</returns>
    public static T? GetControl<T>(TemplateAppliedEventArgs e, string name)
        where T : AvaloniaObject
        => e.NameScope.Find<T>(name);

    /// <summary> Gets a control </summary>
    /// <typeparam name="T">Type of the Control to get</typeparam>
    /// <param name="e">The TemplateAppliedEventArgs</param>
    /// <param name="name">The Name of the Control to return</param>
    /// <param name="avaloniaObj">a control with the indicated params</param>
    public static void GetControl<T>(TemplateAppliedEventArgs e, string name, out T? avaloniaObj)
            where T : AvaloniaObject
                => avaloniaObj = GetControl<T>(e, name);

    public static bool TryFindResource<T>(string resourceName, out T? resource)
    {
        resource = default;
        try
        {
            if (Application.Current is null)
            {
                return false;
            }

            bool found = Application.Current.TryFindResource(resourceName, out object? resourceObject);
            if (found && resourceObject is T resourceTypeT)
            {
                resource = resourceTypeT;
                return true;
            }
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }

        return false;
    }

    public static bool IsPointerInside(this Control control, PointerEventArgs args)
    {
        PointerPoint pp = args.GetCurrentPoint(control);
        var rectangle = new Rect ( control.Bounds.Size) ;
        Rect inflated = rectangle.Inflate(0.5);
        //Debug.WriteLine( inflated.ToString() );
        //Debug.WriteLine(pp.Position.ToString());
        bool inside = inflated.Contains(pp.Position);
        // Debug.WriteLine(inside ? "Inside": "Outside");
        return inside;
    }

    public static void ApplyControlTheme(this Control control, ControlTheme theme)
    {
        if (theme.Setters is null)
        {
            return;
        }

        foreach (var item in theme.Setters)
        {
            var setter = item as Setter;
            if ((setter is not null) && (setter.Property is not null))
            {
                if(setter.Value is ControlTheme nestedTheme)
                {
                    control.ApplyControlTheme(nestedTheme); 
                }
                else
                {
                    control.SetCurrentValue(setter.Property, setter.Value);
                }
            }
        }
    }

    public static readonly Action Nop = delegate { };

    public static readonly Action<Exception> Throw = delegate (Exception ex) { throw (ex); };

    // Summary: Subscribes an element handler to an observable sequence.
    // Parameters:
    //   source: Observable sequence to subscribe to.
    //   onNext: Action to invoke for each element in the observable sequence.
    // Type parameters:
    //      T: The type of the elements in the source sequence.
    // Returns: System.IDisposable object used to unsubscribe from the observable sequence.
    // Exceptions: T:System.ArgumentNullException: source or onNext is null.
    public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(onNext);

        return source.Subscribe(new AnonymousObserver<T>(onNext, Throw, Nop));
    }

    // Summary:  Subscribes an element handler and an exception handler to an observable sequence.
    // Parameters:
    //   source: Observable sequence to subscribe to.
    //   onNext: Action to invoke for each element in the observable sequence.
    //   onError: Action to invoke upon exceptional termination of the observable sequence.
    // Type parameters:
    //   T: The type of the elements in the source sequence.
    // Returns: System.IDisposable object used to unsubscribe from the observable sequence.
    // Exceptions: T:System.ArgumentNullException: source or onNext or onError is null.
    public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(onNext);
        ArgumentNullException.ThrowIfNull(onError);

        return source.Subscribe(new AnonymousObserver<T>(onNext, onError, Nop));
    }
}
