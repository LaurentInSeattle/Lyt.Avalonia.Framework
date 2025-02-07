namespace Lyt.Avalonia.Controls.Countdown;

public partial class CountDownBarControl : UserControl
{
    public CountDownBarControl()
    {
        this.InitializeComponent();
        this.Loaded += this.OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.AdjustSizes();
        this.backgroundRectangle.Fill = this.BackgroundBrush;
        this.foregroundRectangle.Fill = this.ForegroundBrush;
        this.AdjustForegroundSize(this.Value);
    }

    public void AdjustForegroundSize(float value)
    {
        if ((this.Total < 0.000_1f) || (value < 0.000_1f))
        {
            this.foregroundRectangle.IsVisible = false;
            return;
        }

        float ratio = value / this.Total;
        if (ratio < 0.005f)
        {
            this.foregroundRectangle.IsVisible = false;
        }
        else
        {
            double width = this.Bounds.Width;
            width *= ratio;
            this.foregroundRectangle.IsVisible = true;
            this.foregroundRectangle.HorizontalAlignment =
                this.IsProgress ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            this.foregroundRectangle.Width = width;
        }
    }

    private void AdjustSizes()
    {
        float halfHeight = this.BarHeight / 2.0f;
        this.backgroundRectangle.Height = this.BarHeight;
        this.backgroundRectangle.RadiusX = halfHeight;
        this.backgroundRectangle.RadiusY = halfHeight;
        this.foregroundRectangle.Height = this.BarHeight;
        this.foregroundRectangle.RadiusX = halfHeight;
        this.foregroundRectangle.RadiusY = halfHeight;
        this.foregroundRectangle.HorizontalAlignment = 
            this.IsProgress ? HorizontalAlignment.Left : HorizontalAlignment.Right; 
    }

    /// <summary> ForegroundBrush Styled Property </summary>
    public static readonly StyledProperty<IBrush> ForegroundBrushProperty =
        AvaloniaProperty.Register<CountDownBarControl, IBrush>(nameof(ForegroundBrush), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the ForegroundBrush property.</summary>
    public IBrush ForegroundBrush
    {
        get => this.GetValue(ForegroundBrushProperty);
        set
        {
            this.SetValue(ForegroundBrushProperty, value);
            this.foregroundRectangle.Fill = value;
        }
    }

    /// <summary> BackgroundBrush Styled Property </summary>
    public static readonly StyledProperty<IBrush> BackgroundBrushProperty =
        AvaloniaProperty.Register<CountDownBarControl, IBrush>(nameof(BackgroundBrush), defaultValue: Brushes.LightGray);

    /// <summary> Gets or sets the BackgroundBrush property.</summary>
    public IBrush BackgroundBrush
    {
        get => this.GetValue(BackgroundBrushProperty);
        set
        {
            this.SetValue(BackgroundBrushProperty, value);
            this.backgroundRectangle.Fill = value;
        }
    }

    /// <summary> BarHeight Styled Property </summary>
    public static readonly StyledProperty<float> BarHeightProperty =
        AvaloniaProperty.Register<CountDownBarControl, float>(
            nameof(BarHeight),
            defaultValue: 16.0f,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBarHeight,
            enableDataValidation: false);


    /// <summary> Gets or sets the BarHeight property.</summary>
    public float BarHeight
    {
        get => this.GetValue(BarHeightProperty);
        set
        {
            if ((value < 0.0f) || (value > 10_000.0f))
            {
                value = 16.0f;
            }

            this.SetValue(BarHeightProperty, value);
            this.AdjustSizes();
        }
    }

    /// <summary> Coerces the BarHeight value. </summary>
    private static float CoerceBarHeight(AvaloniaObject sender, float value)
    {
        if (sender is not CountDownBarControl control)
        {
            return value;
        }

        if ((value < 0.0f) || (value > 10_000.0f))
        {
            value = 16.0f;
        }

        control.AdjustSizes();
        return value;
    }

    /// <summary> Value Styled Property </summary>
    public static readonly StyledProperty<float> ValueProperty =
        AvaloniaProperty.Register<CountDownBarControl, float>(
            nameof(Value),
            defaultValue: 0.5f,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceValue,
            enableDataValidation: false);

    /// <summary> Gets or sets the Value property.</summary>
    public float Value
    {
        get => this.GetValue(ValueProperty);
        set
        {
            if (value < 0.0f)
            {
                value = 0.0f;
            }

            if (value > this.Total)
            {
                value = this.Total;
            }

            this.SetValue(ValueProperty, value);
            this.AdjustForegroundSize(value);
        }
    }

    /// <summary> Coerces the Value value. </summary>
    private static float CoerceValue(AvaloniaObject sender, float value)
    {
        if (sender is not CountDownBarControl control)
        {
            return value;
        }

        if (value < 0.0f)
        {
            value = 0.0f;
        }

        if (value > control.Total)
        {
            value = control.Total;
        }

        control.AdjustForegroundSize(value);
        return value;
    }

    /// <summary> Total Styled Property </summary>
    public static readonly StyledProperty<float> TotalProperty =
        AvaloniaProperty.Register<CountDownBarControl, float>(
            nameof(Total),
            defaultValue: 1.0f,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceTotal,
            enableDataValidation: false);


    /// <summary> Gets or sets the Total property.</summary>
    public float Total
    {
        get => this.GetValue(TotalProperty);
        set
        {
            if (value < 0.0f)
            {
                value = 100.0f;
            }

            this.SetValue(TotalProperty, value);
            this.AdjustForegroundSize(this.Value);
        }
    }

    /// <summary> Coerces the Total value. </summary>
    private static float CoerceTotal(AvaloniaObject sender, float value)
    {
        if (sender is not CountDownBarControl control)
        {
            return value;
        }

        if (value < 0.0f)
        {
            value = 0.0f;
        }

        control.AdjustForegroundSize(control.Value);
        return value;
    }

    /// <summary> IsProgress Styled Property </summary>
    public static readonly StyledProperty<bool> IsProgressProperty =
        AvaloniaProperty.Register<CountDownBarControl, bool>(
            nameof(IsProgress),
            defaultValue: false,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceIsProgress,
            enableDataValidation: false);


    /// <summary> Gets or sets the IsProgress property.</summary>
    public bool IsProgress
    {
        get => this.GetValue(IsProgressProperty);
        set
        {
            this.SetValue(IsProgressProperty, value);
            this.AdjustForegroundSize(this.Value);
        }
    }

    /// <summary> Coerces the IsProgress value. </summary>
    private static bool CoerceIsProgress(AvaloniaObject sender, bool value)
    {
        if (sender is not CountDownBarControl control)
        {
            return value;
        }

        control.AdjustForegroundSize(control.Value);
        return value;
    }
}