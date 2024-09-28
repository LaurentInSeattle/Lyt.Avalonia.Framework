namespace Lyt.Avalonia.Controls;

public delegate void RoutedEventDelegate(object sender, RoutedEventArgs rea);

public static class Utilities
{
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
        Rect rectangle = new Rect ( control.Bounds.Size) ;
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
}
