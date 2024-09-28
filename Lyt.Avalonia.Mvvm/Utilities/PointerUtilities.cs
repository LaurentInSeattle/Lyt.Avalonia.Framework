namespace Lyt.Avalonia.Mvvm.Utilities;

public static class PointerUtilities
{
    public static MouseButton MouseButtonFromPointerPoint(this PointerPoint pointerPoint)
    {
        var properties = pointerPoint.Properties;
        if (properties.IsLeftButtonPressed)
        {
            return MouseButton.Left;
        }
        else if (properties.IsRightButtonPressed)
        {
            return MouseButton.Right;
        }
        else if (properties.IsMiddleButtonPressed)
        {
            return MouseButton.Middle;
        }

        return MouseButton.None;
    }

}
