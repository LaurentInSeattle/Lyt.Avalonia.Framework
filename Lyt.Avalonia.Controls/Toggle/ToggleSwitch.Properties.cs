using Lyt.Avalonia.Controls.Glyphs;

namespace Lyt.Avalonia.Controls.Toggle;

public partial class ToggleSwitch
{
    #region State and Layout Styled Properties 

    /// <summary> Value Styled Property </summary>
    public static readonly StyledProperty<bool> ValueProperty =
        AvaloniaProperty.Register<ToggleSwitch, bool>(
            nameof(IsShown), defaultValue: true, defaultBindingMode: BindingMode.TwoWay);

    /// <summary> Gets or sets the Value property.</summary>
    public bool Value
    {
        get => this.GetValue(ValueProperty);
        set
        {
            this.SetValue(ValueProperty, value);
            this.switchEllipse.HorizontalAlignment =
                value ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            this.UpdateVisualState();
        }
    }

    /// <summary> Style Styled Property </summary>
    public static readonly StyledProperty<ControlTheme> StyleProperty =
        AvaloniaProperty.Register<ToggleSwitch, ControlTheme>(nameof(Style));

    /// <summary> Gets or sets the Style property.</summary>
    public ControlTheme Style
    {
        get => this.GetValue(StyleProperty);
        set
        {
            this.SetValue(StyleProperty, value);
            this.ApplyControlTheme(value);
        }
    }

    /// <summary> IsShown Styled Property </summary>
    public static readonly StyledProperty<bool> IsShownProperty =
        AvaloniaProperty.Register<ToggleSwitch, bool>(
            nameof(IsShown), defaultValue: true, defaultBindingMode:BindingMode.TwoWay);

    /// <summary> Gets or sets the IsShown property.</summary>
    public bool IsShown
    {
        get => this.GetValue(IsShownProperty);
        set
        {
            this.SetValue(IsShownProperty, value);
            this.mainGrid.IsVisible = value;
        }
    }

    /// <summary> IsDisabled Styled Property </summary>
    public static readonly StyledProperty<bool> IsDisabledProperty =
        AvaloniaProperty.Register<ToggleSwitch, bool>(nameof(IsDisabled), defaultValue: false);

    /// <summary> Gets or sets the IsDisabled property.</summary>
    public bool IsDisabled
    {
        get => this.GetValue(IsDisabledProperty);
        set
        {
            this.SetValue(IsDisabledProperty, value);
            this.UpdateVisualState();
        }
    }

    #endregion State and Layout Styled Properties 

    #region Visual State Styled Properties

    /// <summary> GeneralVisualState Styled Property </summary>
    public static readonly StyledProperty<VisualState> GeneralVisualStateProperty =
        AvaloniaProperty.Register<ToggleSwitch, VisualState>(nameof(GeneralVisualState));

    /// <summary> Gets or sets the GeneralVisualState property.</summary>
    public VisualState GeneralVisualState
    {
        get => this.GetValue(GeneralVisualStateProperty);
        set
        {
            this.SetValue(GeneralVisualStateProperty, value);
            this.UpdateVisualState();
        }
    }

    /// <summary> BackgroundVisualState Styled Property </summary>
    public static readonly StyledProperty<VisualState> BackgroundVisualStateProperty =
        AvaloniaProperty.Register<ToggleSwitch, VisualState>(nameof(BackgroundVisualState));

    /// <summary> Gets or sets the BackgroundVisualState property.</summary>
    public VisualState BackgroundVisualState
    {
        get => this.GetValue(BackgroundVisualStateProperty);
        set
        {
            this.SetValue(BackgroundVisualStateProperty, value);
            this.UpdateVisualState();
        }
    }

    /// <summary> BackgroundVisualState Styled Property </summary>
    public static readonly StyledProperty<VisualState> BackgroundBorderVisualStateProperty =
        AvaloniaProperty.Register<ToggleSwitch, VisualState>(nameof(BackgroundBorderVisualState));

    /// <summary> Gets or sets the BackgroundBorderVisualState property.</summary>
    public VisualState BackgroundBorderVisualState
    {
        get => this.GetValue(BackgroundBorderVisualStateProperty);
        set
        {
            this.SetValue(BackgroundBorderVisualStateProperty, value);
            this.UpdateVisualState();
        }
    }

    #endregion Visual State Styled Properties

    #region Commanding Styled Properties 

    /// <summary> Click Styled Property </summary>
    public static readonly StyledProperty<RoutedEventDelegate> ClickProperty =
        AvaloniaProperty.Register<ToggleSwitch, RoutedEventDelegate>(nameof(Click));

    /// <summary> Gets or sets the Click property.</summary>
    public RoutedEventDelegate Click
    {
        get => this.GetValue(ClickProperty);
        set => this.SetValue(ClickProperty, value);
    }

    /// <summary> Command Styled Property </summary>
    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<ToggleSwitch, ICommand>(nameof(Command));

    /// <summary> Gets or sets the Command property.</summary>
    public ICommand Command
    {
        get => this.GetValue(CommandProperty);
        set => this.SetValue(CommandProperty, value);
    }

    /// <summary> CommandParameter Styled Property </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ToggleSwitch, object?>(nameof(CommandParameter));

    /// <summary> Gets or sets the CommandParameter property.</summary>
    public object? CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    #endregion Commanding Styled Properties 

    #region Text Related Styled Properties

    /// <summary> TrueText Styled Property </summary>
    public static readonly StyledProperty<string> TrueTextProperty =
        AvaloniaProperty.Register<ToggleSwitch, string>(
            nameof(TrueText),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: null,
            enableDataValidation: false);

    /// <summary> Gets or sets the Text property.</summary>
    public string TrueText
    {
        get => this.GetValue(TrueTextProperty);
        set
        {
            this.SetValue(TrueTextProperty, value);
            this.trueTextBlock.Text = value;
        }
    }

    /// <summary> FalseText Styled Property </summary>
    public static readonly StyledProperty<string> FalseTextProperty =
        AvaloniaProperty.Register<ToggleSwitch, string>(
            nameof(FalseText),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: null,
            enableDataValidation: false);

    /// <summary> Gets or sets the FalseText property.</summary>
    public string FalseText
    {
        get => this.GetValue(FalseTextProperty);
        set
        {
            this.SetValue(FalseTextProperty, value);
            this.falseTextBlock.Text = value;
        }
    }

    /// <summary> Typography Styled Property </summary>
    public static readonly StyledProperty<ControlTheme> TypographyProperty =
        AvaloniaProperty.Register<ToggleSwitch, ControlTheme>(
            nameof(Typography),
            defaultValue: new ControlTheme(),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: null,
            enableDataValidation: false);

    /// <summary> Gets or sets the Typography property.</summary>
    public ControlTheme Typography
    {
        get => this.GetValue(TypographyProperty);
        set
        {
            this.SetValue(TypographyProperty, value);
            this.ChangeTypography(value);
        }
    }

    #endregion Text Related Styled Properties

    #region Dependency Property BackgroundCornerRadius

    /// <summary> BackgroundCornerRadius Styled Property </summary>
    public static readonly StyledProperty<double> BackgroundCornerRadiusProperty =
        AvaloniaProperty.Register<ToggleSwitch, double>(
            nameof(BackgroundCornerRadius),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBackgroundCornerRadius,
            enableDataValidation: false);


    /// <summary> Gets or sets the BackgroundCornerRadius property.</summary>
    public double BackgroundCornerRadius
    {
        get => this.GetValue(BackgroundCornerRadiusProperty);
        set
        {
            this.SetValue(BackgroundCornerRadiusProperty, value);
            this.rectangleBackground.RadiusX = value;
            this.rectangleBackground.RadiusY = value;
        }
    }

    /// <summary> Coerces the BackgroundCornerRadius value. </summary>
    private static double CoerceBackgroundCornerRadius(AvaloniaObject sender, double newBackgroundCornerRadius)
        => newBackgroundCornerRadius;

    #endregion Dependency Property BackgroundCornerRadius

    #region Dependency Property BackgroundBorderThickness

    /// <summary> BackgroundBorderThickness Styled Property </summary>
    public static readonly StyledProperty<double> BackgroundBorderThicknessProperty =
        AvaloniaProperty.Register<ToggleSwitch, double>(
            nameof(BackgroundBorderThickness),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBackgroundBorderThickness,
            enableDataValidation: false);


    /// <summary> Gets or sets the BackgroundBorderThickness property.</summary>
    public double BackgroundBorderThickness
    {
        get => this.GetValue(BackgroundBorderThicknessProperty);
        set
        {
            this.SetValue(BackgroundBorderThicknessProperty, value);
            this.rectangleBackground.StrokeThickness = value;
        }
    }

    /// <summary> Coerces the BackgroundBorderThickness value. </summary>
    private static double CoerceBackgroundBorderThickness(AvaloniaObject sender, double newBackgroundBorderThickness) => newBackgroundBorderThickness;

    #endregion Dependency Property BackgroundBorderThickness
}
