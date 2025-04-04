namespace Lyt.Avalonia.Mvvm.Splash;

public partial class ImageSplashWindow : Window
{
    public ImageSplashWindow()
        => this.InitializeComponent();

    public ImageSplashWindow(Uri resourceUri)
    {
        this.InitializeComponent();
        var image = new Bitmap(AssetLoader.Open(resourceUri));
        this.Width = image.PixelSize.Width;
        this.Height = image.PixelSize.Height;
        this.Image.Source = image; 
    }
}