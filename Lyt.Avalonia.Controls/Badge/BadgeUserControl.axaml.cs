namespace Lyt.Avalonia.Controls.BadgeControl;

public partial class BadgeUserControl : UserControl
{
    private ContentControl? contentControl; 
    
    public BadgeUserControl() 
    {
        this.InitializeComponent();
        this.ApplyTemplate();
        this.Loaded += this.OnLoaded; ;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (this.contentControl is not null)
        {
            this.contentControl.Content = this.Content;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.contentControl = e.NameScope.Find<ContentControl>("contentControl");
    }
}


/* 
 USAGE : 

    <UserControl.Styles>
		<Style Selector="TextBlock.badge">
			<Setter Property="FontSize" Value="16"/>
			<Setter Property="FontWeight" Value="Bold"/>
			<Setter Property="Foreground" Value="White"/>
		</Style>
		<Style Selector="Ellipse.badge">
			<Setter Property="Fill" Value="Green"/>
			<Setter Property="Margin" Value="2"/>
		</Style>
	</UserControl.Styles>	

		<badgeControl:BadgeUserControl
			Margin="24"
            Height="80" Width="180"
            HorizontalAlignment="Left" VerticalAlignment="Top"
			>
			<Rectangle 
				RadiusX="10" RadiusY="10"
				Fill="DarkOrange" Opacity="0.5"
				/>
		</badgeControl:BadgeUserControl>
		<badge:Badge 
			BadgePosition="LeftTop" 
			BadgeContent="9+"
			Margin="140 24 24 24"
            Height="80" Width="180"
            HorizontalAlignment="Left" VerticalAlignment="Top"
			>
			<Rectangle
				RadiusX="10" RadiusY="10"
				Fill="DarkOrange" Opacity="0.5"
				/>
		</badge:Badge>


*/