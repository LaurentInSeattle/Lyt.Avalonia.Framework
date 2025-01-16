namespace Lyt.Avalonia.Mvvm.Converters;

public class EnumBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if ((parameter is not string parameterString) || (value is null))
        {
            return AvaloniaProperty.UnsetValue;
        }

        if (! Enum.IsDefined(value.GetType(), value) )
        {
            return AvaloniaProperty.UnsetValue;
        } 

        object parameterValue = Enum.Parse(value.GetType(), parameterString);
        return parameterValue.Equals(value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string parameterString)
        {
            return AvaloniaProperty.UnsetValue;
        }

        return Enum.Parse(targetType, parameterString);
    }
}