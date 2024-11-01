namespace Lyt.Avalonia.Mvvm.Core;

// TODO
// Change Control to StyledElement which is the class that defines the DataContext property
// Inheritance chain: Control -> InputELement -> Interactive -> Layoutable -> Visual -> StyledElement
// Wait until V1 of Avalonia to avoid breaking changes  

/// <summary> Strongly typed bindable </summary>
/// <typeparam name="TControl"></typeparam>
public class Bindable<TControl> : Bindable where TControl : Control, new()
{
    public Bindable() : base() { }

    public Bindable(bool disablePropertyChangedLogging = false, bool disableAutomaticBindingsLogging = false)
        : base(disablePropertyChangedLogging, disableAutomaticBindingsLogging) { }

    public Bindable(TControl control) : base() => this.Bind(control);

    public TControl CreateViewAndBind()
    {
        var view = new TControl();
        this.Bind(view);
        return view;
    }

    public TControl View
        => this.Control as TControl ?? throw new InvalidOperationException("View is null");
}
