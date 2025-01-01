namespace Lyt.Avalonia.Mvvm.Input; 

public sealed class Keyboard
{
    private Window? window;

    public KeyModifiers Modifiers { get; private set; }

    public bool IsStarted { get; private set; }

    public void Start (Window window)
    {
        if (this.IsStarted)
        {
            Debug.WriteLine("Keyboard monitor already started"); 
            return;
        }

        this.Modifiers = KeyModifiers.None;
        this.window = window;
        this.window.AddHandler(
            InputElement.KeyDownEvent, this.OnKey, RoutingStrategies.Tunnel, handledEventsToo: true);
        this.window.AddHandler(
            InputElement.KeyUpEvent, this.OnKey, RoutingStrategies.Tunnel, handledEventsToo: true);
        this.IsStarted = true;
    }

    public void Stop()
    {
        if (!this.IsStarted || this.window is null)
        {
            Debug.WriteLine("Keyboard monitor has no window or is not started, cannot be stopped.");
            return;
        }

        this.Modifiers = KeyModifiers.None;
        this.window.RemoveHandler(InputElement.KeyDownEvent, this.OnKey);
        this.window.RemoveHandler(InputElement.KeyUpEvent, this.OnKey);
        this.IsStarted = false;
    }

    // NOT handled or else cant type anything :( 
    // DONT => args.Handled = true;
    private void OnKey(object? _, KeyEventArgs args) => this.Modifiers = args.KeyModifiers;
}
