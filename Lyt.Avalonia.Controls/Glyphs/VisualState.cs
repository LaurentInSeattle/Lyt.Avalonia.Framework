namespace Lyt.Avalonia.Controls.Glyphs;

#region Old Styles 
/*
    <local:NumericKeypadBase.Resources>
        <Style x:Key="KeypadButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Typography" Value="{StaticResource TextBlockTitleLarge}" />
            <Setter Property="Layout" Value="TextOnly" />
            <Setter Property="ButtonBackground" Value="Rectangle" />
            <Setter Property="BackgroundCornerRadius" Value="6" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="NormalColor" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="BackgroundNormalColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundHotColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundPressedColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundDisabledColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundBorderNormalColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundBorderHotColor" Value="{StaticResource CyprusCerulean_0_040_Brush}" />
            <Setter Property="BackgroundBorderPressedColor" Value="{StaticResource CyprusCerulean_1_040_Brush}" />
            <Setter Property="BackgroundBorderDisabledColor" Value="{StaticResource CyprusCerulean_3_020_Brush}" />
            <Setter Property="BackgroundBorderThickness" Value="2" />
            <Setter Property="IsDisabled" Value="False" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="Width" Value="48" />
            <Setter Property="Height" Value="48" />
            <Setter Property="SnapsToDevicePixels" Value="True" />
        </Style> 
        <Style x:Key="KeypadFunctionButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Typography" Value="{StaticResource TextBlockTitleMedium}" />
            <Setter Property="Layout" Value="IconOnly" />
            <Setter Property="ButtonBackground" Value="Rectangle" />
            <Setter Property="BackgroundCornerRadius" Value="8" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="NormalColor" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="BackgroundNormalColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundHotColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundPressedColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundDisabledColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundBorderNormalColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundBorderHotColor" Value="{StaticResource CyprusCerulean_0_040_Brush}" />
            <Setter Property="BackgroundBorderPressedColor" Value="{StaticResource CyprusCerulean_1_040_Brush}" />
            <Setter Property="BackgroundBorderDisabledColor" Value="{StaticResource CyprusCerulean_3_020_Brush}" />
            <Setter Property="BackgroundBorderThickness" Value="2" />
            <Setter Property="IsDisabled" Value="False" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="Width" Value="48" />
            <Setter Property="Height" Value="48" />
            <Setter Property="SnapsToDevicePixels" Value="True" />
        </Style>

        <Style x:Key="PrimaryButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Typography" Value="{StaticResource TextBlockTestStyle}" />
            <Setter Property="Layout" Value="TextOnly" />
            <Setter Property="ButtonBackground" Value="Rectangle" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="NormalColor" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="BackgroundNormalColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundHotColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundPressedColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundDisabledColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundBorderNormalColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundBorderHotColor" Value="{StaticResource CyprusCerulean_0_040_Brush}" />
            <Setter Property="BackgroundBorderPressedColor" Value="{StaticResource CyprusCerulean_1_040_Brush}" />
            <Setter Property="BackgroundBorderDisabledColor" Value="{StaticResource CyprusCerulean_3_020_Brush}" />
            <Setter Property="IsDisabled" Value="False" />
        </Style>
        <Style x:Key="KeypadButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Typography" Value="{StaticResource TextBlockTitleLarge}" />
            <Setter Property="Layout" Value="TextOnly" />
            <Setter Property="ButtonBackground" Value="Rectangle" />
            <Setter Property="BackgroundCornerRadius" Value="8" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="NormalColor" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="BackgroundNormalColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundHotColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundPressedColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundDisabledColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundBorderNormalColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundBorderHotColor" Value="{StaticResource CyprusCerulean_0_040_Brush}" />
            <Setter Property="BackgroundBorderPressedColor" Value="{StaticResource CyprusCerulean_1_040_Brush}" />
            <Setter Property="BackgroundBorderDisabledColor" Value="{StaticResource CyprusCerulean_3_020_Brush}" />
            <Setter Property="IsDisabled" Value="False" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="HorizontalAlignment" Value="Right" />
            <Setter Property="Width" Value="52" />
            <Setter Property="Height" Value="52" />
        </Style>
        <Style x:Key="KeypadFunctionButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Typography" Value="{StaticResource TextBlockTitleLarge}" />
            <Setter Property="Layout" Value="IconOnly" />
            <Setter Property="ButtonBackground" Value="Rectangle" />
            <Setter Property="BackgroundCornerRadius" Value="8" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="NormalColor" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="BackgroundNormalColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundHotColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundPressedColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundDisabledColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundBorderNormalColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundBorderHotColor" Value="{StaticResource CyprusCerulean_0_040_Brush}" />
            <Setter Property="BackgroundBorderPressedColor" Value="{StaticResource CyprusCerulean_1_040_Brush}" />
            <Setter Property="BackgroundBorderDisabledColor" Value="{StaticResource CyprusCerulean_3_020_Brush}" />
            <Setter Property="IsDisabled" Value="False" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="HorizontalAlignment" Value="Right" />
            <Setter Property="Width" Value="52" />
            <Setter Property="Height" Value="52" />
        </Style>
        <Style x:Key="IconButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Layout" Value="IconOnly" />
            <Setter Property="ButtonBackground" Value="BorderOnly" />
            <Setter Property="NormalColor" Value="{StaticResource KeyLime_1_100_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="BackgroundCornerRadius" Value="8" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="BackgroundNormalColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundHotColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundPressedColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundDisabledColor" Value="{StaticResource CyprusCerulean_3_040_Brush}" />
            <Setter Property="BackgroundBorderNormalColor" Value="{StaticResource CyprusCerulean_2_040_Brush}" />
            <Setter Property="BackgroundBorderHotColor" Value="{StaticResource CyprusCerulean_0_040_Brush}" />
            <Setter Property="BackgroundBorderPressedColor" Value="{StaticResource CyprusCerulean_1_040_Brush}" />
            <Setter Property="BackgroundBorderDisabledColor" Value="{StaticResource CyprusCerulean_3_020_Brush}" />
            <Setter Property="IsDisabled" Value="False" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="Width" Value="80" />
            <Setter Property="Height" Value="80" />
        </Style>
        <Style x:Key="ToolbarButton" TargetType="glyphs:GlyphButton">
            <Setter Property="Typography" Value="{StaticResource TextBlockBodySuperSmall}" />
            <Setter Property="Layout" Value="IconTextBelow" />
            <Setter Property="ButtonBackground" Value="None" />
            <Setter Property="TextForeground" Value="{StaticResource CyprusCerulean_0_100_Brush}" />
            <Setter Property="NormalColor" Value="{StaticResource BrazilianCherry_2_080_Brush}" />
            <Setter Property="HotColor" Value="{StaticResource JaffaOrange_0_100_Brush}" />
            <Setter Property="PressedColor" Value="{StaticResource JaffaOrange_1_100_Brush}" />
            <Setter Property="DisabledColor" Value="{StaticResource CyprusCerulean_4_040_Brush}" />
            <Setter Property="IsDisabled" Value="False" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="HorizontalAlignment" Value="Right" />
            <Setter Property="Width" Value="92" />
            <Setter Property="Height" Value="92" />
        </Style>

*/
#endregion Old Styles 

public sealed class VisualState : AvaloniaObject
{
    /// <summary> Normal Styled Property </summary>
    public static readonly StyledProperty<IBrush> NormalProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Normal), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Normal property.</summary>
    public IBrush Normal
    {
        get => this.GetValue(NormalProperty);
        set => this.SetValue(NormalProperty, value);
    }

    /// <summary> Selected Styled Property </summary>
    public static readonly StyledProperty<IBrush> SelectedProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Selected), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Selected property.</summary>
    public IBrush Selected
    {
        get => this.GetValue(SelectedProperty);
        set => this.SetValue(SelectedProperty, value);
    }

    /// <summary> Disabled Styled Property </summary>
    public static readonly StyledProperty<IBrush> DisabledProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Disabled), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Disabled property.</summary>
    public IBrush Disabled
    {
        get => this.GetValue(DisabledProperty);
        set => this.SetValue(DisabledProperty, value);
    }

    /// <summary> Pressed Styled Property </summary>
    public static readonly StyledProperty<IBrush> PressedProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Pressed), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Pressed property.</summary>
    public IBrush Pressed
    {
        get => this.GetValue(PressedProperty);
        set => this.SetValue(PressedProperty, value);
    }

    /// <summary> Hot Styled Property </summary>
    public static readonly StyledProperty<IBrush> HotProperty =
        AvaloniaProperty.Register<VisualState, IBrush>(nameof(Hot), defaultValue: Brushes.Aquamarine);

    /// <summary> Gets or sets the Hot property.</summary>
    public IBrush Hot
    {
        get => this.GetValue(HotProperty);
        set => this.SetValue(HotProperty, value);
    }
}
