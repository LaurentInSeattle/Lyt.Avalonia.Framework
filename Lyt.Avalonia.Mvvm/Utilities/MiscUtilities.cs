namespace Lyt.Avalonia.Mvvm.Utilities;

using static Lyt.Avalonia.Controls.Utilities;

public static class MiscUtilities
{
    /// <summary> Find first parent of type T in VisualTree. </summary>
    public static TControl? FindParentControl<TControl>(this StyledElement control) where TControl : StyledElement
    {
        StyledElement? parent = control.Parent;
        while ((parent is not null) && (parent is not TControl))
        {
            parent = parent.Parent;
        }

        if (parent is not null && (parent is TControl parentAsT))
        {
            return parentAsT;
        }

        return null;
    }

    public static string ToIconName(this InformationLevel informationLevel)
    {
        switch (informationLevel)
        {
            case InformationLevel.Info:
                return "info";

            case InformationLevel.Warning:
                return "warning";

            case InformationLevel.Error:
                return "error_circle";
        }

        throw new Exception("Missing resource for InformationLevel");
    }

    public static SolidColorBrush ToBrush(this InformationLevel informationLevel)
    {
        SolidColorBrush? brush= null;
        switch (informationLevel)
        {
            case InformationLevel.Info:
                TryFindResource<SolidColorBrush>("LightAqua_0_120", out brush);
                break;

            case InformationLevel.Warning:
                TryFindResource<SolidColorBrush>("OrangePeel_0_100", out brush);
                break;

            case InformationLevel.Error:
                TryFindResource<SolidColorBrush>("PastelOrchid_1_100", out brush);
                break;
        }

        if (brush is not null)
        {
            return brush;
        }

        throw new Exception("Missing resource for InformationLevel");
    }
}
