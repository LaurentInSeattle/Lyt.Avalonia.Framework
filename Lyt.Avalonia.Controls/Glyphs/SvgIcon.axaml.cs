namespace Lyt.Avalonia.Controls.Glyphs;

public partial class SvgIcon : UserControl
{
    // TODO 
    // Support basic GeometryDrawing objects, see Avalonia.IconPacks on GitHub for samples.
    // Should be able to generate and then use a DrawingImage from a GeometryDrawing 
    // 
    // TODO 
    // private const string DefaultSvgIcon = "DefaultSvgIcon";

    private bool imageUpdateRequired;

    private DrawingImage? drawingImage;

    public SvgIcon() => this.InitializeComponent();

    public void UpdateImage()
    {
        //if ( this.Source == "measure" )
        //{
        //    // TODO: Figure out why this one is not loading
        //    // Debugger.Break(); 
        //}

        if (this.drawingImage is null)
        {
            return;
        }

        if (this.drawingImage.Drawing is DrawingGroup drawingGroup)
        {
            this.ProcessDrawingGroup(drawingGroup);
            this.image.Source = this.drawingImage;
            this.image.InvalidateVisual();
            this.viewBox.InvalidateVisual();
        }
    }

    private void CreateDrawingImage(GeometryDrawing geometryDrawing)
    {
        try
        {
            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(geometryDrawing);
            this.drawingImage = new DrawingImage { Drawing = drawingGroup };
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    private void CreateDrawingImage(DrawingGroup drawingGroup)
    {
        try
        {
            this.drawingImage = new DrawingImage { Drawing = drawingGroup };
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    private void ProcessDrawingGroup(DrawingGroup drawingGroup)
    {
        if (drawingGroup.Children != null)
        {
            foreach (var child in drawingGroup.Children)
            {
                if (child is DrawingGroup childDrawingGroup)
                {
                    this.ProcessDrawingGroup(childDrawingGroup);
                }

                if (child is GeometryDrawing geometryDrawing)
                {
                    this.ProcessGeometryDrawing(geometryDrawing);
                }
            }
        }
    }

    private void ProcessGeometryDrawing(GeometryDrawing geometryDrawing)
    {
        if (geometryDrawing.Brush is not null)
        {
            // Brush not null: we need to fill 
            geometryDrawing.Brush = this.Foreground;
        }

        if (geometryDrawing.Pen is Pen pen)
        {
            // If the pen is null, no stroke, no need to do anything 
            if ((pen.Brush is SolidColorBrush) ||
                (pen.Brush is ImmutableSolidColorBrush) || 
                (pen.Brush is ImmutableLinearGradientBrush))
            {
                geometryDrawing.Pen = new Pen() { Thickness = this.StrokeThickness, Brush = this.Foreground };
            }
            else
            {
                if (pen.Brush is not null)
                {
                    Debug.WriteLine("Unsupported pen brush: " + pen.Brush.GetType().Name);
                }
                else
                {
                    Debug.WriteLine("No brush ??? ");
                }

                // if (Debugger.IsAttached) { Debugger.Break(); }
            }
        }
        else
        {
            geometryDrawing.Pen = new Pen() { Thickness = this.StrokeThickness, Brush = this.Foreground };
        }
    }

    #region Styled Properties

    #region Styled Property Source

    /// <summary> Source Styled Property </summary>
    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<SvgIcon, string>(
            nameof(Source),
            defaultValue: "info",
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceSource,
            enableDataValidation: false);

    /// <summary> Gets or sets the Source property.</summary>
    public string Source
    {
        get => this.GetValue(SourceProperty);
        set
        {
            this.SetValue(SourceProperty, value);
            if (this.imageUpdateRequired)
            {
                this.UpdateImage();
                this.imageUpdateRequired = false;
            }
        }
    }

    private static string CoerceSource(AvaloniaObject sender, string newSource)
    {
        bool valid = !string.IsNullOrWhiteSpace(newSource);
        if (valid && (sender is SvgIcon svgIcon))
        {
            string source = string.Concat("icon_", newSource, "DrawingImage");
            if (Utilities.TryFindResource<DrawingImage>(newSource, out svgIcon.drawingImage) ||
                Utilities.TryFindResource<DrawingImage>(source, out svgIcon.drawingImage))
            {
                svgIcon.imageUpdateRequired = true;
            }
            else if (Utilities.TryFindResource<GeometryDrawing>(newSource, out GeometryDrawing? geometryDrawing))
            {
                if (geometryDrawing is not null)
                {
                    svgIcon.CreateDrawingImage(geometryDrawing);
                    svgIcon.imageUpdateRequired = true;
                }
            }
            else if (Utilities.TryFindResource<DrawingGroup>(newSource, out DrawingGroup? drawingGroup))
            {
                if (drawingGroup is not null)
                {
                    svgIcon.CreateDrawingImage(drawingGroup);
                    svgIcon.imageUpdateRequired = true;
                }
            }

            if (svgIcon.imageUpdateRequired)
            {
                svgIcon.UpdateImage();
                svgIcon.imageUpdateRequired = false;
                return newSource;
            }
        }

        return string.Empty;
    }

    #endregion Styled Property Source

    #region Styled Property Wadding

    /// <summary> Wadding Styled Property </summary>
    public static readonly StyledProperty<Thickness> WaddingProperty =
        AvaloniaProperty.Register<SvgIcon, Thickness>(
            nameof(Wadding),
            defaultValue: new Thickness(0),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceWadding,
            enableDataValidation: false);

    /// <summary> Gets or sets the Wadding property.</summary>
    public Thickness Wadding
    {
        get => (Thickness)this.GetValue(WaddingProperty);
        set => this.SetValue(WaddingProperty, value);
    }

    private static Thickness CoerceWadding(AvaloniaObject sender, Thickness newWadding)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.viewBox.Margin = newWadding;
        }

        return newWadding;
    }

    #endregion Styled  Property Wadding

    #region Styled  Property Height

    /// <summary> Height Styled Property </summary>
    public static readonly new StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<SvgIcon, double>(
            nameof(Height),
            defaultValue: 32.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceHeight,
            enableDataValidation: false);


    /// <summary> Gets or sets the Height property.</summary>
    public new double Height
    {
        get => this.GetValue(HeightProperty);
        set => this.SetValue(HeightProperty, value);
    }

    /// <summary> Coerces the Height value. </summary>
    private static double CoerceHeight(AvaloniaObject sender, double newHeight)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.grid.Height = newHeight;
        }

        return newHeight;
    }

    #endregion Styled Property Height

    #region Styled  Property Width

    /// <summary> Width Styled Property </summary>
    public static readonly new StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<SvgIcon, double>(
            nameof(Width),
            defaultValue: 32.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceWidth,
            enableDataValidation: false);


    /// <summary> Gets or sets the Width property.</summary>
    public new double Width
    {
        get => this.GetValue(WidthProperty);
        set => this.SetValue(WidthProperty, value);
    }

    /// <summary> Coerces the Width value. </summary>
    private static double CoerceWidth(AvaloniaObject sender, double newWidth)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.grid.Width = newWidth;
        }

        return newWidth;
    }

    #endregion Styled Property Width

    #region Styled Property Background

    /// <summary> Background Styled Property </summary>
    public static new readonly StyledProperty<IBrush> BackgroundProperty =
        AvaloniaProperty.Register<SvgIcon, IBrush>(
            nameof(Background),
            defaultValue: new SolidColorBrush(Colors.Aquamarine, 1.0),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBackground,
            enableDataValidation: false);

    /// <summary> Gets or sets the Background property.</summary>
    public new IBrush Background
    {
        get => this.GetValue(BackgroundProperty);
        set => this.SetValue(BackgroundProperty, value);
    }

    /// <summary> Coerces the Background value. </summary>
    private static IBrush CoerceBackground(AvaloniaObject sender, IBrush newBackground)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.grid.Background = newBackground;
        }

        return newBackground;
    }

    #endregion Styled Property Background

    #region Styled Property Foreground

    /// <summary> Foreground Styled Property </summary>
    public static new readonly StyledProperty<IBrush> ForegroundProperty =
        AvaloniaProperty.Register<SvgIcon, IBrush>(
            nameof(Foreground),
            defaultValue: Brushes.Aquamarine,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceForeground,
            enableDataValidation: false);

    /// <summary> Gets or sets the Foreground property.</summary>
    public new IBrush Foreground
    {
        get => this.GetValue(ForegroundProperty);

        set
        {
            this.SetValue(ForegroundProperty, value);
            if (this.imageUpdateRequired)
            {
                this.UpdateImage();
                this.imageUpdateRequired = false;
            }
        }
    }

    /// <summary> Coerces the Foreground value. </summary>
    private static IBrush CoerceForeground(AvaloniaObject sender, IBrush newForeground)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.imageUpdateRequired = true;
        }

        return newForeground;
    }

    #endregion Styled Property Foreground

    #region Styled Property StrokeThickness

    /// <summary> StrokeThickness Styled Property </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<SvgIcon, double>(
            nameof(StrokeThickness),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceStrokeThickness,
            enableDataValidation: false);

    /// <summary> Gets or sets the StrokeThickness property.</summary>
    public double StrokeThickness
    {
        get => this.GetValue(StrokeThicknessProperty);
        set
        {
            this.SetValue(StrokeThicknessProperty, value);
            if (this.imageUpdateRequired)
            {
                this.UpdateImage();
                this.imageUpdateRequired = false;
            }
        }
    }

    /// <summary> Coerces the StrokeThickness value. </summary>
    private static double CoerceStrokeThickness(AvaloniaObject sender, double newStrokeThickness)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.imageUpdateRequired = true;
        }

        return newStrokeThickness;
    }

    #endregion Styled Property StrokeThickness

    #endregion Styled Properties
}
