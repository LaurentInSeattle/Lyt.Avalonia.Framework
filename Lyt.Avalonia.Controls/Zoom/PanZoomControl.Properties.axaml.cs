namespace Lyt.Avalonia.Controls.PanZoom;

public partial class PanZoomControl : UserControl
{
    public enum ActionRequest
    {
        None, 
        Fit, 
        One,
    }

    /// <summary> ShiftToDrag Styled Property </summary>
    public static readonly StyledProperty<bool> ShiftToDragProperty =
        AvaloniaProperty.Register<PanZoomControl, bool>(
            nameof(ShiftToDrag),
            defaultValue: false,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceIsDisabled,
            enableDataValidation: false);

    /// <summary> Gets or sets the ShiftToDrag property.</summary>
    public bool ShiftToDrag
    {
        get => this.GetValue(ShiftToDragProperty);
        set => this.SetValue(ShiftToDragProperty, value);
    }

    private static bool CoerceIsDisabled(AvaloniaObject sender, bool newShiftToDrag)
    {
        if (sender is PanZoomControl panZoomControl)
        {
            panZoomControl.shiftToDrag = newShiftToDrag;
        }

        return newShiftToDrag;
    }

    /// <summary> Zoomable content  </summary>
    public static readonly StyledProperty<Control?> ZoomableContentProperty =
        AvaloniaProperty.Register<PanZoomControl, Control?>(
            nameof(ZoomableContent),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceContent,
            enableDataValidation: false);

    /// <summary> Gets or sets the ZoomableContent property.</summary>
    public Control? ZoomableContent    {
        get => this.GetValue(ZoomableContentProperty);
        set => this.SetValue(ZoomableContentProperty, value);
    }

    private static Control? CoerceContent(AvaloniaObject sender, Control? newContent)
    {
        if (sender is PanZoomControl zoomControl)
        {
            if (newContent is null)
            {
                Debug.WriteLine("Null Content!");
                return zoomControl.ZoomableContent;
            }
            else
            {
                // Debug.WriteLine("Zoom: " + newContent.ToString());
                zoomControl.UpdateContent(newContent);
                return newContent;
            }
        }

        return null;
    }

    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(
            nameof(Zoom),
            defaultValue: 0.5,
            inherits: false,
            defaultBindingMode: BindingMode.TwoWay,
            validate: null,
            coerce: CoerceZoom,
            enableDataValidation: false);

    public double Zoom { get => this.GetValue(ZoomProperty); set => this.SetValue(ZoomProperty, value); }

    private static double CoerceZoom(AvaloniaObject sender, double newZoom)
    {
        if (sender is PanZoomControl zoomControl)
        {
            double minZoom = zoomControl.GetFitZoomFactor();
            double maxZoom = PanZoomControl.MaxZoom;
            double oldValue = zoomControl.GetValue(ZoomProperty);
            // Debug.WriteLine("Zoom: " + oldValue.ToString("F3") + " - > " + newZoom.ToString("F3"));

            if ( newZoom < 0.000_1)
            {
                return oldValue;
            }

            if (newZoom < minZoom) 
            {
                return minZoom;
            }

            if (newZoom > maxZoom)
            {
                return maxZoom;
            }

            zoomControl.UpdateZoom(newZoom);
        }

        return newZoom;
    }

    public static readonly StyledProperty<double> ZoomFactorProperty =
        AvaloniaProperty.Register<PanZoomControl, double>(
            nameof(ZoomFactor),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.TwoWay,
            validate: null,
            coerce: CoerceZoomFactor,
            enableDataValidation: false);

    public double ZoomFactor { get => this.GetValue(ZoomFactorProperty); set => this.SetValue(ZoomFactorProperty, value); }

    private static double CoerceZoomFactor(AvaloniaObject sender, double newZoomFactor)
    {
        if (sender is PanZoomControl zoomControl)
        {
            double minZoom = zoomControl.GetFitZoomFactor();
            double maxZoom = PanZoomControl.MaxZoom;
            double newZoom = minZoom * newZoomFactor;
            double oldValue = zoomControl.GetValue(ZoomFactorProperty);
            // Debug.WriteLine("Zoom: " + oldValue.ToString("F3") + " - > " + newZoomFactor.ToString("F3"));

            if (newZoomFactor < 1.000)
            {
                return oldValue;
            }

            if (newZoom < minZoom)
            {
                return minZoom;
            }

            if (newZoom > maxZoom)
            {
                return maxZoom;
            }

            zoomControl.UpdateZoom(newZoom);
        }

        return newZoomFactor;
    }

    public static readonly StyledProperty<ActionRequest> RequestProperty =
        AvaloniaProperty.Register<PanZoomControl, ActionRequest>(
            nameof(Request),
            defaultValue: ActionRequest.None,
            inherits: false,
            defaultBindingMode: BindingMode.TwoWay,
            validate: null,
            coerce: CoerceRequest,
            enableDataValidation: false);

    public ActionRequest Request { get => this.GetValue(RequestProperty); set => this.SetValue(RequestProperty, value); }

    private static ActionRequest CoerceRequest(AvaloniaObject sender, ActionRequest actionRequest)
    {
        if ((actionRequest != ActionRequest.None) && (sender is PanZoomControl zoomControl))
        {
            zoomControl.ProcessRequest(actionRequest);
        }

        return ActionRequest.None;
    }
}