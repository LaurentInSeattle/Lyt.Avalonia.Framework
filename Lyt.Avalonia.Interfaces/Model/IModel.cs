namespace Lyt.Avalonia.Interfaces.Model;

public interface IModel
{
    /// <summary>  Initializes the model. </summary>
    Task Initialize();

    /// <summary> Shutdowns the model </summary>
    Task Shutdown();

    /// <summary> Subscribes to updates from the model </summary>
    void SubscribeToUpdates(Action<ModelUpdateMessage> onUpdate, bool withUiDispatch = false);
}

public interface IApplicationModel
{
    /// <summary>  Initializes all models. </summary>
    Task Initialize();

    /// <summary> Shutdowns all models </summary>
    Task Shutdown();
}
