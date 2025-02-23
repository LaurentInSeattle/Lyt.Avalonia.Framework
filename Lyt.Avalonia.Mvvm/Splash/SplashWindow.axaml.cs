namespace Lyt.Avalonia.Mvvm.Splash;

public partial class SplashWindow : Window
{
    public SplashWindow()
        => this.InitializeComponent();

    public SplashWindow(Uri resourceUri)
    {
        this.InitializeComponent();
        var image = new Bitmap(AssetLoader.Open(resourceUri));
        this.Width = image.PixelSize.Width;
        this.Height = image.PixelSize.Height;
        this.Image.Source = image; 
    }
}