namespace Lyt.Avalonia.Mvvm.Messenger;

public class WeakAction
{
    /// <summary> Just to please the compiler...  </summary>
    protected WeakAction() { }

    /// <summary> Initializes a new instance of the <see cref="WeakAction" /> class. </summary>
    /// <param name="action">The action that will be associated to this instance.</param>
    public WeakAction(Action action) : this(action.Target!, action, false) { }

    /// <summary> Initializes a new instance of the <see cref="WeakAction" /> class. </summary>
    /// <param name="target">The action's owner.</param>
    /// <param name="action">The action that will be associated to this instance.</param>
    public WeakAction(object target, Action action, bool withUiDispatch, bool doNotLog = false)
    {
        if (action.Method.IsStatic)
        {
            throw new NotSupportedException("no static actions");
        }

        this.Method = action.Method;
        this.ActionReference = new WeakReference(action.Target);
        this.Reference = new WeakReference(target);
        this.WithUiDispatch = withUiDispatch;
        this.DoNotLog = doNotLog;
    }

    /// <summary> Gets the name of the method that this WeakAction represents. </summary>
    public string MethodName => this.Method!.Name;

    /// <summary>
    /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
    /// by the Garbage Collector already.
    /// </summary>
    public bool IsAlive => this.Reference != null && this.Reference.IsAlive;

    /// <summary> Gets the Action's owner. This object is stored as a  <see cref="WeakReference" />.</summary>
    /// <summary> 
    /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakAction's method passed in 
    /// the constructor.
    /// </summary>
    protected MethodInfo? Method { get; set; }

    /// <summary> Gets or sets a WeakReference to this WeakAction's action's target. This is not necessarily 
    /// the same as <see cref="Reference" />, for example if the method is anonymous. </summary>
    protected WeakReference? ActionReference { get; set; }

    /// <summary>
    /// Gets or sets a WeakReference to the target passed when constructing the WeakAction. This is not 
    /// necessarily the same as  <see cref="ActionReference" />, for example if the method is anonymous.
    /// </summary>
    protected WeakReference? Reference { get; set; }

    /// <summary> Gets or sets a boolean value indicating the action should be dispatched on the UI thread  </summary>
    protected bool WithUiDispatch { get; set; }

    /// <summary> Gets or sets a boolean value indicating the action should be logged or not.</summary>
    protected bool DoNotLog { get; set; }

    /// <summary> The target of the weak reference. </summary>
    public object Target => this.Reference?.Target!;

    /// <summary> The target of the weak action reference. </summary>
    protected object ActionTarget => this.ActionReference?.Target!;

    /// <summary> Executes the action. This only happens if the action's owner is still alive.</summary>
    public void Execute()
    {
        object actionTarget = this.ActionTarget;
        if (this.IsAlive)
        {
            if ((this.Method != null) && (this.ActionReference != null) && (actionTarget != null))
            {
                if (this.WithUiDispatch)
                {
                    Dispatch.OnUiThread(() => { _ = this.Method.Invoke(actionTarget, null); }, DispatcherPriority.Normal);
                }
                else
                {
                    _ = this.Method.Invoke(actionTarget, null);
                }

            }
        }
    }

    /// <summary> Sets the reference that this instance stores to null. </summary>
    public void MarkForDeletion()
    {
        this.Reference = null;
        this.ActionReference = null;
        this.Method = null;
    }
}
