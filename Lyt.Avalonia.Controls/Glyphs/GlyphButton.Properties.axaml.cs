namespace Lyt.Avalonia.Controls.Glyphs;

public partial class GlyphButton
{
    #region State and Layout Styled Properties 

    /// <summary> Style Styled Property </summary>
    public static readonly StyledProperty<ControlTheme> StyleProperty =
        AvaloniaProperty.Register<GlyphButton, ControlTheme>(nameof(Style));

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

    /// <summary> Behaviour Styled Property </summary>
    public static readonly StyledProperty<ButtonBehaviour> BehaviourProperty =
        AvaloniaProperty.Register<GlyphButton, ButtonBehaviour>(nameof(Behaviour), defaultValue: ButtonBehaviour.Tap);

    /// <summary> Gets or sets the Behaviour property.</summary>
    public ButtonBehaviour Behaviour
    {
        get => this.GetValue(BehaviourProperty);
        set
        {
            this.SetValue(BehaviourProperty, value);
            this.ChangeBehaviour(value);
        }
    }

    /// <summary> Layout Styled Property </summary>
    public static readonly StyledProperty<ButtonLayout> LayoutProperty =
        AvaloniaProperty.Register<GlyphButton, ButtonLayout>(nameof(Layout), defaultValue: ButtonLayout.IconOnly);

    /// <summary> Gets or sets the Layout property.</summary>
    public ButtonLayout Layout
    {
        get => this.GetValue(LayoutProperty);
        set
        {
            this.SetValue(LayoutProperty, value);
            this.ChangeLayout(value);
        }
    }

    /// <summary> ButtonBackground Styled Property </summary>
    public static readonly StyledProperty<ButtonBackground> ButtonBackgroundProperty =
        AvaloniaProperty.Register<GlyphButton, ButtonBackground>(
            nameof(ButtonBackground), defaultValue: ButtonBackground.None);

    /// <summary> Gets or sets the ButtonBackground property.</summary>
    public ButtonBackground ButtonBackground
    {
        get => this.GetValue(ButtonBackgroundProperty);
        set
        {
            this.SetValue(ButtonBackgroundProperty, value);
            this.ChangeButtonBackground(value);
        }
    }

    /// <summary> IsShown Styled Property </summary>
    public static readonly StyledProperty<bool> IsShownProperty =
        AvaloniaProperty.Register<GlyphButton, bool>(nameof(IsShown), defaultValue: true);

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
        AvaloniaProperty.Register<GlyphButton, bool>(nameof(IsDisabled), defaultValue: false);

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

    /// <summary> IsSelected Styled Property </summary>
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<GlyphButton, bool>(nameof(IsSelected), defaultValue: false);

    /// <summary> Gets or sets the IsSelected property.</summary>
    public bool IsSelected
    {
        get => this.GetValue(IsSelectedProperty);
        set
        {
            this.SetValue(IsSelectedProperty, value);
            this.UpdateVisualState();
        }
    }

    /// <summary> Group Styled Property </summary>
    public static readonly StyledProperty<SelectionGroup?> GroupProperty =
        AvaloniaProperty.Register<GlyphButton, SelectionGroup?>(
            nameof(Group),
            defaultValue: null,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGroup,
            enableDataValidation: false);

    /// <summary> Gets or sets the Group property.</summary>
    public SelectionGroup? Group
    {
        get => this.GetValue(GroupProperty);
        set => this.SetValue(GroupProperty, value);
    }

    /// <summary> Coerces the Group value. </summary>
    private static SelectionGroup? CoerceGroup(AvaloniaObject sender, SelectionGroup? newGroup)
    {
        if ((sender is GlyphButton glyphButton) && (newGroup is not null))
        {
            newGroup.Register(glyphButton);
        }

        return newGroup; 
    }

    #endregion State and Layout Styled Properties 

    #region Commanding Styled Properties 

    /// <summary> Click Styled Property </summary>
    public static readonly StyledProperty<RoutedEventDelegate> ClickProperty =
        AvaloniaProperty.Register<GlyphButton, RoutedEventDelegate>(nameof(Click));

    /// <summary> Gets or sets the Click property.</summary>
    public RoutedEventDelegate Click
    {
        get => this.GetValue(ClickProperty);
        set => this.SetValue(ClickProperty, value);
    }

    /// <summary> Command Styled Property </summary>
    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<GlyphButton, ICommand>(nameof(Command));

    /// <summary> Gets or sets the Command property.</summary>
    public ICommand Command
    {
        get => this.GetValue(CommandProperty);
        set => this.SetValue(CommandProperty, value);
    }

    /// <summary> CommandParameter Styled Property </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<GlyphButton, object?>(nameof(CommandParameter));

    /// <summary> Gets or sets the CommandParameter property.</summary>
    public object? CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    #endregion Commanding Styled Properties 

    #region Glyph Related Styled Properties 

    /// <summary> GlyphSource Styled Property </summary>
    public static readonly StyledProperty<string> GlyphSourceProperty =
        AvaloniaProperty.Register<GlyphButton, string>(
            nameof(GlyphSource),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGlyphSource,
            enableDataValidation: false);

    /// <summary> Gets or sets the GlyphSource property.</summary>
    public string GlyphSource
    {
        get => this.GetValue(GlyphSourceProperty);
        set
        {
            this.SetValue(GlyphSourceProperty, value);
            this.icon.Source = value;
            this.icon.UpdateImage();
        }
    }

    /// <summary> Coerces the GlyphSource value. </summary>
    private static string CoerceGlyphSource(AvaloniaObject sender, string newText)
    {
        if ( sender is GlyphButton glyphButton)
        {
            glyphButton.icon.Source = newText;
            glyphButton.icon.UpdateImage();
        }

        return newText;
    } 

    /// <summary> GlyphAngle Styled Property </summary>
    public static readonly StyledProperty<double> GlyphAngleProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(GlyphAngle),
            defaultValue: 0.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGlyphAngle,
            enableDataValidation: false);


    /// <summary> Gets or sets the GlyphAngle property.</summary>
    public double GlyphAngle
    {
        get => this.GetValue(GlyphAngleProperty);
        set
        {
            if (!this.IsHot)
            {
                this.glyphAngle = value;
            }

            this.SetValue(GlyphAngleProperty, value);
            // See: https://stackoverflow.com/questions/70116300/layouttransform-scaletransform-in-avalonia
            // TODO: Verify the rotation 
            this.icon.RenderTransform = new RotateTransform(value);
        }
    }

    /// <summary> Coerces the GlyphAngle value. </summary>
    private static double CoerceGlyphAngle(AvaloniaObject sender, double newGlyphAngle) => newGlyphAngle;

    /// <summary> GlyphStrokeThickness Styled Property </summary>
    public static readonly StyledProperty<double> GlyphStrokeThicknessProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(GlyphStrokeThickness),
            defaultValue: 1.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceGlyphStrokeThickness,
            enableDataValidation: false);


    /// <summary> Gets or sets the GlyphStrokeThickness property.</summary>
    public double GlyphStrokeThickness
    {
        get => this.GetValue(GlyphStrokeThicknessProperty);
        set
        {
            this.SetValue(GlyphStrokeThicknessProperty, value);
            this.icon.StrokeThickness = value;
            this.icon.UpdateImage();
        }
    }

    /// <summary> Coerces the GlyphStrokeThickness value. </summary>
    private static double CoerceGlyphStrokeThickness(AvaloniaObject sender, double newGlyphStrokeThickness) => newGlyphStrokeThickness;

    #endregion Glyph Related Styled Properties 

    #region Text Related Styled Properties

    /// <summary> Text Styled Property </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<GlyphButton, string>(
            nameof(Text),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceText,
            enableDataValidation: false);

    /// <summary> Gets or sets the Text property.</summary>
    public string Text
    {
        get => this.GetValue(TextProperty);
        set
        {
            this.SetValue(TextProperty, value);
            this.textBlock.Text = value;
        }
    }

    /// <summary> Coerces the Text value. </summary>
    private static string CoerceText(AvaloniaObject sender, string newText) => newText;

    /// <summary> Typography Styled Property </summary>
    public static readonly StyledProperty<ControlTheme> TypographyProperty =
        AvaloniaProperty.Register<GlyphButton, ControlTheme>(
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
        AvaloniaProperty.Register<GlyphButton, double>(
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
            this.rectanglePopup.RadiusX = value;
            this.rectanglePopup.RadiusY = value;
        }
    }

    /// <summary> Coerces the BackgroundCornerRadius value. </summary>
    private static double CoerceBackgroundCornerRadius(AvaloniaObject sender, double newBackgroundCornerRadius)
        => newBackgroundCornerRadius;

    #endregion Dependency Property BackgroundCornerRadius

    #region Dependency Property BackgroundBorderThickness

    /// <summary> BackgroundBorderThickness Styled Property </summary>
    public static readonly StyledProperty<double> BackgroundBorderThicknessProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
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

    #region Visual State Styled Properties

    /// <summary> GeneralVisualState Styled Property </summary>
    public static readonly StyledProperty<VisualState> GeneralVisualStateProperty =
        AvaloniaProperty.Register<GlyphButton, VisualState>(nameof(GeneralVisualState));

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
        AvaloniaProperty.Register<GlyphButton, VisualState>(nameof(BackgroundVisualState));

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
        AvaloniaProperty.Register<GlyphButton, VisualState>(nameof(BackgroundBorderVisualState));

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

    #region Dependency Property Countdown

    /// <summary> Countdown Styled Property </summary>
    public static readonly StyledProperty<double> CountdownProperty =
        AvaloniaProperty.Register<GlyphButton, double>(
            nameof(Countdown),
            defaultValue: 8.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceCountdown,
            enableDataValidation: false);


    /// <summary> Gets or sets the Countdown property.</summary>
    public double Countdown
    {
        get => this.GetValue(CountdownProperty);
        set => this.SetValue(CountdownProperty, value);
    }

    /// <summary> Coerces the Countdown value. </summary>
    private static double CoerceCountdown(AvaloniaObject sender, double newCountdown) => newCountdown;

    #endregion Dependency Property Countdown

    #region LATER : Popup Keyboards Properties 

    /*
        #region Dependency Property Keys

        /// <summary> Keys Dependency Property </summary>
        public static readonly DependencyProperty KeysProperty =
            DependencyProperty.Register("Keys", typeof(string), typeof(GlyphButton),
                new FrameworkPropertyMetadata((string)string.Empty,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnKeysChanged),
                    new CoerceValueCallback(CoerceKeys)));

        /// <summary> Gets or sets the Keys property.</summary>
        public string Keys
        {
            get => (string)this.GetValue(KeysProperty);
            set => this.SetValue(KeysProperty, value);
        }

        /// <summary> Handles changes to the Keys property. </summary>
        private static void OnKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (GlyphButton)d;
            var oldKeys = (string)e.OldValue;
            var newKeys = target.Keys;
            target.OnKeysChanged(oldKeys, newKeys);
        }

        /// <summary> Provides derived classes an opportunity to handle changes to the Keys property. </summary>
        protected virtual void OnKeysChanged(string oldKeys, string newKeys)
        {
            if (!string.IsNullOrEmpty(newKeys))
            {
                this.Behaviour = ButtonBehaviour.Keyboard;
            }
        }

        /// <summary> Coerces the Keys value. </summary>
        private static object CoerceKeys(DependencyObject d, object value)
        {
            var target = (GlyphButton)d;
            var desiredKeys = (string)value;
            // TODO
            return desiredKeys;
        }

        #endregion Dependency Property Keys

        #region Dependency Property IsShifted

        /// <summary> IsShifted Dependency Property </summary>
        public static readonly DependencyProperty IsShiftedProperty =
            DependencyProperty.Register("IsShifted", typeof(bool), typeof(GlyphButton),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnIsShiftedChanged),
                    new CoerceValueCallback(CoerceIsShifted)));

        /// <summary> Gets or sets the IsShifted property.</summary>
        public bool IsShifted
        {
            get => (bool)this.GetValue(IsShiftedProperty);
            set => this.SetValue(IsShiftedProperty, value);
        }

        /// <summary> Handles changes to the IsShifted property. </summary>
        private static void OnIsShiftedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (GlyphButton)d;
            var oldIsShifted = (bool)e.OldValue;
            var newIsShifted = target.IsShifted;
            target.OnIsShiftedChanged(oldIsShifted, newIsShifted);
        }

        /// <summary> Provides derived classes an opportunity to handle changes to the IsShifted property. </summary>
        protected virtual void OnIsShiftedChanged(bool oldIsShifted, bool newIsShifted)
        {
            if ((this.Behaviour == ButtonBehaviour.Keyboard) || (this.Behaviour == ButtonBehaviour.PopupKeyboard))
            {
                if (!string.IsNullOrEmpty(this.Text))
                {
                    this.Text = newIsShifted ? this.Text.ToUpper() : this.Text.ToLower();
                }

                if (this.popupKeyboard is not null)
                {
                    this.popupKeyboard.Shift(newIsShifted);
                }
            }
        }

        /// <summary> Coerces the IsShifted value. </summary>
        private static object CoerceIsShifted(DependencyObject d, object value)
        {
            var target = (GlyphButton)d;
            var desiredIsShifted = (bool)value;
            // TODO
            return desiredIsShifted;
        }

        #endregion Dependency Property IsShifted
*/
    #endregion Popup Keyboards Properties 
}
