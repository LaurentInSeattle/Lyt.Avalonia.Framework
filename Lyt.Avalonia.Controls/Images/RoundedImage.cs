namespace Lyt.Avalonia.Controls.Images;

public class RoundedImage : Image
{
    public static readonly AttachedProperty<double> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<RoundedImage, double>(
            "CornerRadius", typeof(RoundedImage), 8);

    public static void SetCornerRadius(AvaloniaObject element, double parameter)
        => element.SetValue(CornerRadiusProperty, parameter);

    public static double GetCornerRadius(AvaloniaObject element)
        => element.GetValue(CornerRadiusProperty);

    protected override Size MeasureOverride(Size availableSize)
    {
        IImage? source = this.Source;
        if (source is not null)
        {
            Size result = 
                this.Stretch.CalculateSize(availableSize, source.Size, this.StretchDirection);
            this.Clip =
                new RectangleGeometry(
                    new Rect(0, 0, result.Width, result.Height),
                    RoundedImage.GetCornerRadius(this),
                    RoundedImage.GetCornerRadius(this));
            return result;
        }

        return base.MeasureOverride(availableSize);
    }
}
