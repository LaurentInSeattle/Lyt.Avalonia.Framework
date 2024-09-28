namespace Lyt.Avalonia.Mvvm.Messenger;

/// <summary>
/// The Messenger is a Singleton class allowing objects to exchange messages.
/// The Messenger API is similar to the Messenger class of the MVVM Light Framework
/// </summary>
public sealed class Messenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, HashSet<WeakAction?>> recipients;
    private readonly ILogger logger; 

    public Messenger(ILogger logger)
    {
        this.logger = logger;
        int concurency = 2 * Environment.ProcessorCount;
        this.recipients = new (concurency, 32);
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage. The action parameter will be executed when
    /// a corresponding message is sent. Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers for.</typeparam>
    /// <param name="recipient"></param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. The recipient that will receive the messages is the Action target.
    /// Note that closures are not supported.
    /// </param>
    public void Subscribe<TMessage>(Action<TMessage> action, bool withUiDispatch = false, bool doNotLog = false) 
        where TMessage : class
    {
        var type = typeof(TMessage);
        if (!this.recipients.TryGetValue(type, out var weakActions))
        {
            weakActions = [];
            if (!this.recipients.TryAdd(type, weakActions))
            {
                weakActions = this.recipients[type];
            }
        }

        object? target = action.Target;
        if ( target is null)
        {
            this.logger.Error( "Static actions are NOT supported.");
            throw new NotSupportedException("no static actions");
        }

        _ = weakActions.Add(new WeakAction<TMessage>(this.logger, target, action, withUiDispatch, doNotLog));
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will reach all recipients that registered
    /// for this message type using the Register methods.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public void Publish<TMessage>(TMessage message) where TMessage : class
    {
        if (!this.recipients.TryGetValue(typeof(TMessage), out var weakActions))
        {
            this.logger.Info( "No recipients for " + typeof(TMessage).Name);
            return;
        }

        if ((weakActions is null) || (weakActions.Count == 0))
        {
            return;
        }

        foreach (var action in weakActions)
        {
            if (action is WeakAction<TMessage> actionOfTMessage)
            {
                actionOfTMessage.Execute(message);
            }
        }
    }

    /// <summary>
    /// Unregisters a recipient completely. After this method is executed, the recipient
    /// will not receive any messages anymore.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    public void Unregister(object recipient)
    {
        var toRemove = new List<WeakAction?>();
        foreach (var hashSet in this.recipients.Values)
        {
            toRemove.Clear();
            foreach (var action in hashSet)
            {
                if (action == null)
                {
                    toRemove.Add(action);
                }
                else
                {
                    if (action.Target == recipient)
                    {
                        toRemove.Add(action);
                        action.MarkForDeletion();
                    }
                }
            }

            foreach (var action in toRemove)
            {
                _ = hashSet.Remove(action);
            }
        }
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages only. After this method is executed,
    /// the recipient will not receive messages of type TMessage anymore, but will still receive other
    /// message types (if it registered for them previously).
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    public void Unregister<TMessage>(object recipient) where TMessage : class
    {
        var toRemove = new List<WeakAction>();
        foreach (var hashSet in this.recipients.Values)
        {
            toRemove.Clear();
            foreach (var action in hashSet)
            {
                if ((action?.Target == recipient) && (action is WeakAction<TMessage>))
                {
                    toRemove.Add(action);
                    action.MarkForDeletion();
                }
            }

            foreach (var action in toRemove)
            {
                _ = hashSet.Remove(action);
            }
        }
    }
}
