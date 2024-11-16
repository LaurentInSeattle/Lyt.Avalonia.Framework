namespace Lyt.StateMachine;

public class FiniteStateMachine<TState, TTrigger, TTag> : IDisposable
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    private readonly object lockObject;

    private readonly ILogger logger;

    private readonly Stack<TState> navigationStack;

    private TState state;

    private Action<TState, TState>? onStateChanged;

    private DateTime dateTimeTimerStart;

    private TState stateTimerStart;

    private int currentTimeoutValue;

    private System.Timers.Timer? timer;

    private bool disposedValue;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Frozen dictionaries cannot be created in the constructor, but by design, should never ever change or become null
    /// <summary> State Definitions  indexed by State. </summary>
    protected FrozenDictionary<TState, StateDefinition<TState, TTrigger, TTag>> StateDefinitions { get; private set; }

    public FiniteStateMachine(ILogger logger)
    {
#pragma warning restore CS8618

        this.logger = logger;
        this.lockObject = new();
        this.navigationStack = new Stack<TState>();
    }

    /// <summary> The current state of the machine </summary>
    public TState State
    {
        get => this.state;
        private set
        {
            if (!this.state.Equals(value))
            {
                TState oldState = this.state;
                this.state = value;
                this.onStateChanged?.Invoke(oldState, value);
            }
        }
    }

    /// <summary> The tag for the current state of the machine </summary>
    /// <remarks> can be null </remarks>
    public TTag? Tag => this.StateDefinitions[this.state].Tag;

    /// <summary> True if the machine is properly initialized.</summary>
    public bool IsInitialized { get; private set; }

    /// <summary> For a timeout transition, the time left before firing, or -1 if no timeout.</summary>
    public int TimeRemainingMillisecs
    {
        get
        {
            if (this.timer is null)
            {
                return -1;
            }

            var elapsed = DateTime.Now - this.dateTimeTimerStart;
            int elapsedMilliseconds = (int)elapsed.TotalMilliseconds;
            int millisecsLeft = this.currentTimeoutValue - elapsedMilliseconds;
            return millisecsLeft > 0 ? millisecsLeft : 0;
        }
    }

    /// <summary> Initializes the state machine providing all definitions.</summary>
    /// <remarks> Can be done only once! </remarks>
    public bool Initialize(StateMachineDefinition<TState, TTrigger, TTag> stateMachineDefinition)
    {
        this.CheckNotInitialized();
        try
        {
            // Nullify the callback so that it wont trigger when setting up the initial state
            this.onStateChanged = null;
            this.State = stateMachineDefinition.InitialState;
            this.onStateChanged = stateMachineDefinition.OnStateChanged;
            var dictionary = stateMachineDefinition.StateDefinitions.ToDictionary(x => x.State, x => x);
            this.StateDefinitions = dictionary.ToFrozenDictionary();
            this.IsInitialized = true;
            return true;
        }
        catch (Exception ex)
        {
            this.logger.Error(ex.ToString());
            this.CheckInitialized();
        }

        return false;
    }

    public void ClearBackNavigation() => this.navigationStack.Clear();

    public bool GoBack()
    {
        try
        {
            TState newState = this.navigationStack.Pop();
            this.ExecuteTransition(this.State, newState, pushState: false);
            return true;
        } 
        catch (Exception ex)
        {
            this.logger.Error("GoBack: Exception thrown" + ex.ToString());
            throw new Exception("GoBack: Exception thrown", ex);
        }
    }

    public bool CanGoBack(out TState state)
    {
        state = this.State;
        if (this.navigationStack.Count == 0)
        {
            return false;
        }

        state = this.navigationStack.Peek();
        return true;
    }

    public bool CanGoNext(out TState state, out TTrigger trigger)
    {
        trigger = default;
        state = this.State;
        lock (this.lockObject)
        {
            this.logger.Debug("Trying to advance  from  " + this.State.ToString());
            if (!this.StateDefinitions.TryGetValue(this.State, out var stateDefinition))
            {
                this.logger.Error("Fire: Invalid state");
                throw new Exception("Fire: Invalid state");
            }

            var triggers = stateDefinition.TriggerDefinitions;
            if ((triggers is null) || (triggers.Count == 0))
            {
                this.logger.Debug("No triggers defined for state " + this.State.ToString());
                return false;
            }

            // Count valid transitions
            int validTransitions = 0;
            foreach (var triggerDefinition in triggers)
            {
                // Transition is valid if there is no validator 
                bool validated = true; 
                var validator = triggerDefinition.Validator;
                if (validator is not null)
                {
                    validated = validator.Invoke();
                }

                if (validated)
                {
                    ++validTransitions;
                    state = triggerDefinition.ToState;
                    trigger = triggerDefinition.Trigger;
                    this.logger.Debug("Can Advance: To state " + state.ToString() + " " + trigger.ToString());
                }
            }

            if (validTransitions == 1)
            {
                return true;
            }
        }

        this.logger.Debug("Can Advance: No valid transitions ");
        return false;
    }

    public bool GoNext(TTrigger trigger)
    {
        try
        {
            this.Fire(trigger);
            return true;
        }
        catch (Exception ex)
        {
            this.logger.Error("GoNext: Exception thrown" + ex.ToString());
            throw new Exception("GoNext: Exception thrown", ex);
        }
    }

    /// <summary> Keeps alive the current transition, a timeout transition, restarting its timer. </summary>
    /// <remarks> No effect, if no timers. </remarks>
    public void KeepAlive()
    {
        this.CheckInitialized();

        // Cancel previous timer, if running 
        this.CancelTimer();

        // Start new timeout timer 
        if (!this.StateDefinitions.TryGetValue(this.State, out var stateDefinition))
        {
            this.logger.Error("Keep Alive: Invalid state");
            throw new Exception("Keep Alive: Invalid state");
        }

        if (stateDefinition.TimeoutDefinition is not null)
        {
            this.StartTimer(this.currentTimeoutValue);
        }
    }

    /// <summary> Check if the transition, identified by its trigger, can be fired </summary>
    /// <returns> True if successfull </returns>
    public bool CanFire(TTrigger trigger)
    {
        this.CheckInitialized();

        if (Monitor.IsEntered(this.lockObject))
        {
            // The -> SAME <- thread holds the lock: Not good... 
            // Prevents OnEnter and OnLeave to deadlock the state machine by recursive invocations
            this.logger.Error("The current thread is already firing a trigger.");
            throw new Exception("The current thread is already firing a trigger.");
        }

        lock (this.lockObject)
        {
            Debug.WriteLine("Trying to trigger " + trigger.ToString() + "  from  " + this.State.ToString());
            if (!this.StateDefinitions.TryGetValue(this.State, out var stateDefinition))
            {
                this.logger.Error("Fire: Invalid state");
                throw new Exception("Fire: Invalid state");
            }

            var triggers = stateDefinition.TriggerDefinitions;
            if ((triggers is null) || (triggers.Count == 0))
            {
                this.logger.Warning("No triggers defined for state " + this.State.ToString());
                return false;
            }

            var triggerDefinition =
                (from triggerDef in triggers
                 where triggerDef.Trigger.Equals(trigger)
                 select triggerDef)
                 .FirstOrDefault();
            if (triggerDefinition is null)
            {
                this.logger.Warning(this.State.ToString() + " cannot be triggered by " + trigger.ToString());
                return false;
            }

            // Validate transition 
            bool validated = true;
            var validator = triggerDefinition.Validator;
            if (validator is not null)
            {
                validated = validator.Invoke();
            }

            if (!validated)
            {
                this.logger.Info(this.State.ToString() + "; Validator cancelled trigger: " + trigger.ToString());
                return false;
            }

            return true;
        }
    }

    /// <summary> If valid, Fires the transition, identified by its trigger </summary>
    /// <returns> True if successfull transition</returns>
    public bool Fire(TTrigger trigger)
    {
        try
        {
            if ( this.CanFire(trigger))
            {
                if (this.StateDefinitions.TryGetValue(this.State, out var stateDefinition))
                {
                    var triggers = stateDefinition.TriggerDefinitions;
                    var triggerDefinition =
                        (from triggerDef in triggers
                         where triggerDef.Trigger.Equals(trigger)
                         select triggerDef)
                         .FirstOrDefault();
                    if ( triggerDefinition is not null)
                    {
                        this.ExecuteTransition(this.State, triggerDefinition.ToState);
                        this.logger.Info("Transition complete, new state:  " + this.State.ToString());
                        return true;
                    }
                } 
            }
        }
        catch (Exception ex)
        {
            this.logger.Error("GoNext: Exception thrown" + ex.ToString());
            throw new Exception("GoNext: Exception thrown", ex);
        }

        return false;
    }

    private void ExecuteTransition(TState fromState, TState toState, bool pushState = true)
    {
        if (pushState)
        {
            this.navigationStack.Push(fromState);
        }

        if (!this.StateDefinitions.TryGetValue(fromState, out var stateDefinition))
        {
            this.logger.Error("ExecuteTransition: Invalid new state");
            throw new Exception("ExecuteTransition: Invalid new state");
        }

        // Invoke current state leave delegate, if any
        var leaveAction = stateDefinition.OnLeave;
        leaveAction?.Invoke(toState);

        // Cancel current timer, if running 
        this.CancelTimer();

        // Update state 
        this.State = toState;

        // Start new timeout timer if needed, use new state to lookup
        if (!this.StateDefinitions.TryGetValue(toState, out var toStateDefinition))
        {
            this.logger.Error("ExecuteTransition: Invalid new state");
            throw new Exception("ExecuteTransition: Invalid new state");
        }

        var timeoutDefinition = toStateDefinition.TimeoutDefinition;
        if (timeoutDefinition is not null)
        {
            this.StartTimer(timeoutDefinition.ValueMillisecs);
        }

        // Invoke new state enter delegate, if any
        var enterAction = toStateDefinition.OnEnter;
        enterAction?.Invoke(fromState);
    }

    private void CancelTimer()
    {
        if (this.timer is not null)
        {
            this.timer.Enabled = false;
            this.timer.Stop();
            this.timer.Elapsed -= this.OnTimerElapsed;
            this.timer = null;
        }
    }

    private void StartTimer(int millisec)
    {
        if (millisec <= 0)
        {
            string msg = "Timeout zero or negative, for state: " + this.State.ToString();
            this.logger.Error(msg);
            throw new Exception(msg);
        }

        if (millisec < 50)
        {
            this.logger.Warning("Cannot handle timeout shorter than 50 ms, for state: " + this.State.ToString());
            return;
        }

        this.stateTimerStart = this.State;
        this.dateTimeTimerStart = DateTime.Now;
        this.currentTimeoutValue = millisec;
        if (this.timer is not null)
        {
            this.CancelTimer();
        }

        this.timer = new System.Timers.Timer() { AutoReset = false, Interval = millisec, };
        this.timer.Elapsed += this.OnTimerElapsed;
        this.timer.Start();
    }

    /// <summary> Execute a timeout transition, when the timer elapses</summary>
    private void OnTimerElapsed(object? _1, ElapsedEventArgs _2)
    {
        //  Execute Timeout Transition
        lock (this.lockObject)
        {
            if (!this.stateTimerStart.Equals(this.State))
            {
                // Race condition: State changed while we were waiting for the lock
                this.logger.Warning(
                    "Race condition: State changed while we were waiting for the lock: Expected: " +
                    this.stateTimerStart.ToString() + "   Now: " + this.State.ToString());
                return;
            }

            if (!this.StateDefinitions.TryGetValue(this.State, out var stateDefinition))
            {
                this.logger.Error("OnTimerElapsed: Invalid new state");
                throw new Exception("OnTimerElapsed: Invalid new state");
            }

            var timeoutDefinition = stateDefinition.TimeoutDefinition;
            if (timeoutDefinition is null)
            {
                // Race condition: State changed while we were waiting for the lock
                this.logger.Warning("No timeout defined for state " + this.State.ToString());
                return;
            }

            // No validator for timeouts 
            TState fromState = this.State;
            this.ExecuteTransition(this.State, timeoutDefinition.ToState);

            // Invoke timeout delegate, if any
            var timeoutAction = stateDefinition.OnEnter;
            timeoutAction?.Invoke(fromState);
        }
    }

    // Consider Conditional DEBUG 
    private void CheckInitialized()
    {
        if (!this.IsInitialized)
        {
            // Is this state machine properly defined ? 
            // State Machine is not initialized or failed to initialize.
            if (Debugger.IsAttached) { Debugger.Break(); }
            string msg = "State Machine is not initialized or failed to initialize.";
            this.logger.Error(msg);
            throw new InvalidOperationException(msg);
        }
    }

    // Consider Conditional DEBUG 
    private void CheckNotInitialized()
    {
        if (this.IsInitialized)
        {
            // Is this state machine properly defined ? 
            // State Machine is not initialized or failed to initialize.
            if (Debugger.IsAttached) { Debugger.Break(); }
            string msg = "State Machine is already initialized.";
            this.logger.Error(msg);
            throw new InvalidOperationException(msg);
        }
    }

    #region IDisposable implementation 

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects) here
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            this.CancelTimer();

            // Set large fields to null
            this.disposedValue = true;
        }
    }

    // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~FiniteStateMachine()
    {
        // Do not change this code.
        // Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code.
        // Put cleanup code, if any needed in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable implementation 
}