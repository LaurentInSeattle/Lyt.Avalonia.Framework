namespace Lyt.Avalonia.Interfaces.Messenger;

/// <summary> See implementation for documentation </summary>
public interface IMessenger
{
    void Subscribe<TMessage>(Action<TMessage> action, bool withUiDispatch = false, bool doNotLog = false)
        where TMessage : class;

    void Publish<TMessage>(TMessage message) where TMessage : class;

    void Unregister(object recipient);

    void Unregister<TMessage>(object recipient) where TMessage : class;
}
