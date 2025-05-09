namespace Lyt.Avalonia.Controls.Progress;

public sealed class ProgressRing : TemplatedControl
{
    // Important: Do not forget to include this style in App.Xaml
    //
    // 		<StyleInclude Source="avares://Lyt.Avalonia.Controls/Progress/ProgressRing.axaml"/>
    //
    // or else nothing will show up :( 


    private const string LargeState = ":large";
    private const string SmallState = ":small";

    private const string InactiveState = ":inactive";
    private const string ActiveState = ":active";

    private double maxSideLength;
    private double ellipseDiameter;
    private Thickness ellipseOffset;

    static ProgressRing()
    {
        //DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing),
        //    new FrameworkPropertyMetadata(typeof(ProgressRing)));
    }

    public ProgressRing()
    {
        this.maxSideLength = 10.0;
        this.ellipseDiameter = 10.0;
        this.ellipseOffset = new Thickness(2);
    }

    public bool IsActive
    {
        get => (bool)this.GetValue(IsActiveProperty);
        set => this.SetValue(IsActiveProperty, value);
    }


    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<ProgressRing, bool>(
            nameof(IsActive),
            defaultValue: true);

#pragma warning disable IDE0051 // Remove unused private members
    private static void OnIsActiveChanged(AvaloniaObject obj, bool _)
        => ((ProgressRing)obj).UpdateVisualStates();
#pragma warning restore IDE0051 

    public static readonly DirectProperty<ProgressRing, double> MaxSideLengthProperty =
        AvaloniaProperty.RegisterDirect<ProgressRing, double>(
           nameof(MaxSideLength),
           o => o.MaxSideLength);

    public double MaxSideLength
    {
        get => this.maxSideLength;
        private set => this.SetAndRaise(MaxSideLengthProperty, ref this.maxSideLength, value);
    }

    public static readonly DirectProperty<ProgressRing, double> EllipseDiameterProperty =
        AvaloniaProperty.RegisterDirect<ProgressRing, double>(
           nameof(EllipseDiameter),
           o => o.EllipseDiameter);

    public double EllipseDiameter
    {
        get => this.ellipseDiameter;
        private set => this.SetAndRaise(EllipseDiameterProperty, ref this.ellipseDiameter, value);
    }

    public static readonly DirectProperty<ProgressRing, Thickness> EllipseOffsetProperty =
        AvaloniaProperty.RegisterDirect<ProgressRing, Thickness>(
           nameof(EllipseOffset),
           o => o.EllipseOffset);

    public Thickness EllipseOffset
    {
        get => this.ellipseOffset;
        private set => this.SetAndRaise(EllipseOffsetProperty, ref this.ellipseOffset, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        double maxSideLength = Math.Min(this.Width, this.Height);
        double ellipseDiameter = 0.1 * maxSideLength;
        if (maxSideLength <= 40.0)
        {
            ellipseDiameter += 1.0;
        }

        this.EllipseDiameter = ellipseDiameter;
        this.MaxSideLength = maxSideLength;
        this.EllipseOffset = new Thickness(0, maxSideLength / 2 - ellipseDiameter, 0, 0);
        this.UpdateVisualStates();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsActiveProperty)
        {
            this.UpdateVisualStates();
        }
    }

    private void UpdateVisualStates()
    {
        this.PseudoClasses.Remove(ActiveState);
        this.PseudoClasses.Remove(InactiveState);
        this.PseudoClasses.Remove(SmallState);
        this.PseudoClasses.Remove(LargeState);
        this.PseudoClasses.Add(this.IsActive ? ActiveState : InactiveState);
        this.PseudoClasses.Add(this.maxSideLength < 60 ? SmallState : LargeState);
    }
}