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
        => informationLevel switch
        {
            InformationLevel.Success => "emoji_happy",
            InformationLevel.Info => "path_info",
            InformationLevel.Warning => "path_warning",
            InformationLevel.Error => "path_error",
            _ => throw new Exception("Missing resource for InformationLevel"),
        };

    public static StreamGeometry ToIconGeometry( this InformationLevel informationLevel)
    {
        string resourceName = informationLevel.ToIconName();
        if( TryFindResource<StreamGeometry>(resourceName, out StreamGeometry? streamGeometry))
        {
            if ( streamGeometry is not null )
            {
                return streamGeometry;
            }
        }

        throw new Exception("Missing resource for InformationLevel");
    }

    public static SolidColorBrush ToBrush(this InformationLevel informationLevel)
    {
        SolidColorBrush? brush= null;
        switch (informationLevel)
        {
            case InformationLevel.Success:
                TryFindResource<SolidColorBrush>("FreshGreen_1_100", out brush);
                break;

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
