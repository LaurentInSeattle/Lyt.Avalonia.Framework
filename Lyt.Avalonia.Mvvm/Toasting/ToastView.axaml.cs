namespace Lyt.Avalonia.Controls.Toasting;

public partial class ToastView : UserControl
{
    public ToastView()
    {
        this.InitializeComponent();
        this.SetValue(Panel.ZIndexProperty, 999);
        this.Loaded += (_, _) => this.OuterGrid.Opacity = 1.0;

        this.DataContextChanged += (_, _) =>
        {
            if (this.DataContext is ToastViewModel toastViewModel)
            {
                // There should be no reason 
                this.Icon.Source = toastViewModel.IconName;
            }
        };
    }
}
