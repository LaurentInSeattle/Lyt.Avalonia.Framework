namespace Lyt.Avalonia.Controls.PanZoom;

public partial class PanZoomControl : UserControl
{
    public const double DragStrength = 1.35;
    public const double MaxZoom = 8.0;

    private Size contentSize;
    private bool isDragging;
    private Point startDragPoint;
    private Point currentDragPoint;

    public PanZoomControl()
    {
        this.InitializeComponent();

        this.Loaded += this.OnLoaded;
        this.ZoomContentPresenter.SizeChanged += this.OnZoomContentPresenterSizeChanged;
        this.ScrollViewer.SizeChanged += this.OnScrollViewerSizeChanged;
        this.PointerPressed += this.OnPointerPressed;
        this.PointerReleased += this.OnPointerReleased;
        this.PointerMoved += this.OnPointerMoved;
    }

    ~PanZoomControl()
    {
        this.Loaded -= this.OnLoaded;
        this.ZoomContentPresenter.SizeChanged -= this.OnZoomContentPresenterSizeChanged;
        this.ScrollViewer.SizeChanged -= this.OnScrollViewerSizeChanged;
        this.PointerPressed -= this.OnPointerPressed;
        this.PointerReleased -= this.OnPointerReleased;
        this.PointerMoved -= this.OnPointerMoved;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // get the presenter, and initialize
        if (this.ZoomContentPresenter is null)
        {
            throw new Exception("no presenter");
        }

        if (this.ZoomableContent is null)
        {
            Debug.WriteLine("Loaded: no content");
            return;
        }

        this.UpdateContent(this.ZoomableContent);
        this.UpdateZoom(this.Zoom);
    }

    private void OnScrollViewerSizeChanged(object? sender, SizeChangedEventArgs e)
        => this.ZoomToFit();

    private void OnZoomContentPresenterSizeChanged(object? sender, SizeChangedEventArgs e)
        => this.AdjustContentSize(this.ZoomContentPresenter.Content);

    private void AdjustContentSize(object? controlObject)
    {
        if (controlObject is not Control control)
        {
            return;
        }

        this.contentSize = control.Bounds.Size;
        if ((this.contentSize.Width <= 0.000_001) || (this.contentSize.Height <= 0.000_001))
        {
            Debug.WriteLine("Invalid content size, forcing a layout pass");
            control.InvalidateVisual();
            this.contentSize = control.Bounds.Size;
            if ((this.contentSize.Width <= 0.000_001) || (this.contentSize.Height <= 0.000_001))
            {
                Debug.WriteLine("Still... Invalid content size");
            }
        }
    }

    private void UpdateContent(Control control)
    {
        this.ZoomContentPresenter.Content = control;
        this.AdjustContentSize(control);
    }

    private void UpdateZoom(double zoom)
    {
        if ((this.contentSize.Width <= 0.000_001) || (this.contentSize.Height <= 0.000_001))
        {
            Debug.WriteLine("Invalid content");
            this.AdjustContentSize(this.ZoomContentPresenter.Content);
        }

        this.Grid.Width = this.contentSize.Width * zoom;
        this.Grid.Height = this.contentSize.Height * zoom;
    }

    private void ProcessRequest(ActionRequest newActionRequest)
    {
        Debug.WriteLine("Request: " + newActionRequest.ToString());
        switch (newActionRequest)
        {
            default:
            case ActionRequest.None: return;

            case ActionRequest.Fit:
                this.ZoomToFit();
                break;
            case ActionRequest.One:
                this.ZoomToOne();
                break;
        }
    }

    private void ZoomToFit()
    {
        Debug.WriteLine("Executing Request: ZoomToFit");
        double zoom = this.GetFitZoomFactor();
        this.Zoom = zoom;
        this.Grid.Width = this.contentSize.Width * zoom;
        this.Grid.Height = this.contentSize.Height * zoom;
    }

    private double GetFitZoomFactor()
    {
        Size size = this.ScrollViewer.Bounds.Size;
        double zoomX = size.Width / this.contentSize.Width;
        double zoomY = size.Height / this.contentSize.Height;
        return  Math.Min(zoomX, zoomY);
    }

    private void ZoomToOne()
    {
        Debug.WriteLine("Executing Request: Zoom To One");
        this.Zoom = 1.0;
        this.Grid.Width = this.contentSize.Width;
        this.Grid.Height = this.contentSize.Height;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (!args.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            return;
        }

        if (this.ZoomContentPresenter.Content is Control control)
        {
            this.isDragging = true;
            this.startDragPoint = args.GetPosition(control);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args) =>  this.isDragging = false;
    
    private void OnPointerMoved(object? sender, PointerEventArgs args)
    {
        if (!this.isDragging)
        {
            return;
        }

        // CONSIDER: Creare a property to select the KeyModifier allowing drag 
        if (!args.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            this.isDragging = false;
            return;
        }

        if (this.ZoomContentPresenter.Content is Control control)
        {
            this.isDragging = true;
            this.currentDragPoint = args.GetPosition(control);
            double distance = Point.Distance(this.startDragPoint, this.currentDragPoint);
            if (distance > 2.0)
            {
                Size extent = this.ScrollViewer.Extent;
                Vector offset = this.ScrollViewer.Offset;
                double dx = (this.currentDragPoint.X - this.startDragPoint.X) * this.Zoom / PanZoomControl.DragStrength;
                double dy = (this.currentDragPoint.Y - this.startDragPoint.Y) * this.Zoom / PanZoomControl.DragStrength;
                double x = Math.Min(offset.X - dx, extent.Width);
                double y = Math.Min(offset.Y - dy, extent.Height);
                x = Math.Max(x, 0.0);
                y = Math.Max(y, 0.0);
                this.ScrollViewer.Offset = new Vector(x, y);
                this.startDragPoint = this.currentDragPoint;
            }
        }
    }
}