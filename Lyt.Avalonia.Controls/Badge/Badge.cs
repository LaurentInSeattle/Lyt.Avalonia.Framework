namespace Lyt.Avalonia.Controls.Badge;

public enum BadgePosition
{
    Right,
    Left,
    Top,
    Bottom,
    RightTop,
    RightBottom,
    LeftTop,
    LeftBottom
}

public partial class Badge : ContentControl
{
    private ContentPresenter? badgePresenter;

    static Badge()
    {
        ClipToBoundsProperty.OverrideDefaultValue<Badge>(false);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.GetControl(e, "PART_BadgePresenter", out this.badgePresenter);

        if( this.badgePresenter is not null)
        {
            IObservable<object?> observable = this.badgePresenter.GetObservable(ContentProperty); 
            observable.Subscribe(this.SetBadgeMargin);
            BoundsProperty.Changed.Subscribe(this.SetBadgeMargin);
            this.SetBadgeMargin(null);
        }
    }

    private void SetBadgeMargin(object? _)
    {
        if (this.badgePresenter is null)
        {
            return;
        } 

       double top = 0, left = 0, right = 0, bottom = 0;

        switch (this.BadgePosition)
        {
            case BadgePosition.Right:
                right = -this.badgePresenter.Bounds.Width / 2;
                BadgeVerticalAlignment = VerticalAlignment.Center;
                BadgeHorizontalAlignment = HorizontalAlignment.Right;
                break;

            case BadgePosition.Left:
                left = -this.badgePresenter.Bounds.Width / 2;
                BadgeVerticalAlignment = VerticalAlignment.Center;
                BadgeHorizontalAlignment = HorizontalAlignment.Left;
                break;

            case BadgePosition.Top:
                top = -this.badgePresenter.Bounds.Height / 2;
                BadgeVerticalAlignment = VerticalAlignment.Top;
                BadgeHorizontalAlignment = HorizontalAlignment.Center;
                break;

            case BadgePosition.Bottom:
                bottom = -badgePresenter.Bounds.Height / 2;
                BadgeVerticalAlignment = VerticalAlignment.Bottom;
                BadgeHorizontalAlignment = HorizontalAlignment.Center;
                break;

            case BadgePosition.RightTop:
                right = -badgePresenter.Bounds.Width / 2;
                top = -badgePresenter.Bounds.Height / 2;
                BadgeVerticalAlignment = VerticalAlignment.Top;
                BadgeHorizontalAlignment = HorizontalAlignment.Right;
                break;

            case BadgePosition.LeftTop:
                left = -badgePresenter.Bounds.Width / 2;
                top = -badgePresenter.Bounds.Height / 2;
                BadgeVerticalAlignment = VerticalAlignment.Top;
                BadgeHorizontalAlignment = HorizontalAlignment.Left;
                break;

            case BadgePosition.RightBottom:
                right = -badgePresenter.Bounds.Width / 2;
                bottom = -badgePresenter.Bounds.Height / 2;
                BadgeVerticalAlignment = VerticalAlignment.Bottom;
                BadgeHorizontalAlignment = HorizontalAlignment.Right;
                break;

            case BadgePosition.LeftBottom:
                left = -badgePresenter.Bounds.Width / 2;
                bottom = -badgePresenter.Bounds.Height / 2;
                BadgeVerticalAlignment = VerticalAlignment.Bottom;
                BadgeHorizontalAlignment = HorizontalAlignment.Left;
                break;
        }

        //Debug.WriteLine(top);
        //Debug.WriteLine(left);
        this.BadgeThickness = new(left, top, right, bottom);
    }

    #region Properties 

    private VerticalAlignment badgeVerticalAlignment;

    private HorizontalAlignment badgeHorizontalAlignment;
    
    private Thickness badgeThickness;

    public static readonly StyledProperty<object> BadgeContentProperty =
        AvaloniaProperty.Register<Badge, object>(nameof(BadgeContent));

    public static readonly StyledProperty<IDataTemplate> BadgeContentTemplateProperty =
        AvaloniaProperty.Register<Badge, IDataTemplate>(nameof(BadgeContentTemplate));
    
    public static readonly StyledProperty<BadgePosition> BadgePositionProperty =
        AvaloniaProperty.Register<Badge, BadgePosition>(nameof(BadgePosition));
    
    public static readonly DirectProperty<Badge, VerticalAlignment> BadgeVerticalAlignmentProperty =
        AvaloniaProperty.RegisterDirect<Badge, VerticalAlignment>(nameof(BadgeVerticalAlignment), o => o.BadgeVerticalAlignment);
    
    public static readonly DirectProperty<Badge, HorizontalAlignment> BadgeHorizontalAlignmentProperty =
        AvaloniaProperty.RegisterDirect<Badge, HorizontalAlignment>(nameof(BadgeHorizontalAlignment), o => o.BadgeHorizontalAlignment);
    
    public static readonly DirectProperty<Badge, Thickness> BadgeThicknessProperty =
        AvaloniaProperty.RegisterDirect<Badge, Thickness>(nameof(BadgeThickness), o => o.BadgeThickness);

    public object BadgeContent
    {
        get => this.GetValue(BadgeContentProperty);
        set => this.SetValue(BadgeContentProperty, value);
    }

    public IDataTemplate BadgeContentTemplate
    {
        get => this.GetValue(BadgeContentTemplateProperty);
        set => this.SetValue(BadgeContentTemplateProperty, value);
    }
    
    public BadgePosition BadgePosition
    {
        get => this.GetValue(BadgePositionProperty);
        set => this.SetValue(BadgePositionProperty, value);
    }
    
    public VerticalAlignment BadgeVerticalAlignment
    {
        get => this.badgeVerticalAlignment;
        private set => this.SetAndRaise(BadgeVerticalAlignmentProperty, ref this.badgeVerticalAlignment, value);
    }
    
    public HorizontalAlignment BadgeHorizontalAlignment
    {
        get => this.badgeHorizontalAlignment;
        private set => this.SetAndRaise(BadgeHorizontalAlignmentProperty, ref this.badgeHorizontalAlignment, value);
    }
    
    public Thickness BadgeThickness
    {
        get => this.badgeThickness;
        set => this.SetAndRaise(BadgeThicknessProperty, ref this.badgeThickness, value);
    }

    #endregion Properties 
}
