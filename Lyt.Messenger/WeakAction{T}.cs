using Lyt.Avalonia.Interfaces.Dispatch;
using Lyt.Avalonia.Interfaces.Logger;

namespace Lyt.Messaging;

public sealed class WeakAction<T> : WeakAction where T : class
{
    private readonly ILogger logger;

    /// <summary> Initializes a new instance of the <see cref="WeakAction" /> class. </summary>
    /// <param name="action">The action that will be associated to this instance.</param>
    public WeakAction(ILogger logger, IDispatch dispatcher, Action<T> action) 
        : this(logger, dispatcher, action.Target!, action, false) { }

    /// <summary> Initializes a new instance of the <see cref="WeakAction" /> class. </summary>
    /// <param name="target">The action's owner.</param>
    /// <param name="action">The action that will be associated to this instance.</param>
    public WeakAction( ILogger logger, IDispatch dispatcher, object target, Action<T> action, bool withUiDispatch, bool doNotLog = false) 
        : base(dispatcher)
    {
        this.logger = logger;

        if (action.Method.IsStatic)
        {
            this.logger.Error( "Static actions are NOT supported.");
            throw new NotSupportedException("no static actions");
        }

        this.Method = action.Method;
        this.ActionReference = new WeakReference(action.Target);
        this.Reference = new WeakReference(target);
        this.WithUiDispatch = withUiDispatch;
        this.DoNotLog = doNotLog;
    }

    /// <summary> Executes the action. This only happens if the action's owner is still alive.</summary>
    public new void Execute() => this.Execute(default);

    /// <summary> Executes the action. This only happens if the action's owner is still alive.</summary>
    public void Execute(T? parameter)
    {
        object actionTarget = this.ActionTarget;
        if (this.IsAlive)
        {
            if ((this.Method != null) && (this.ActionReference != null) && (actionTarget != null))
            {
                try
                {
                    if (!this.DoNotLog)
                    {
                        //this.logger.Info(
                        //    "Executing subscribed action: " + this.Method.Name + "  on:  " + actionTarget.GetType().ToString() );
                    } 

                    if ( this.WithUiDispatch)
                    {
                        this.dispatcher.OnUiThread(
                            () => { _ = this.Method.Invoke(actionTarget, [parameter]); });
                    }
                    else
                    {
                        _ = this.Method.Invoke(actionTarget, [parameter]);
                    }
                }
                catch
                {
                    // do nothing. This can sometimes blow up if everything's not loaded yet.
                }
            }
        }
    }
}
