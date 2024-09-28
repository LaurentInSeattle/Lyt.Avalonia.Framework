namespace Lyt.Avalonia.Mvvm.Core;

/// <summary> Minimal implementation of INotifyPropertyChanged</summary>
public class NotifyPropertyChanged : INotifyPropertyChanged
{
    /// <summary> Occurs when a property value changes. </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary> Raises the property changed event. </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged(string propertyName)
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
