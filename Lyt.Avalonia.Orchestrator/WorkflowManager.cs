namespace Lyt.Avalonia.Orchestrator;

public sealed class WorkflowManager<TState, TTrigger> : IDisposable
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public const int DefaultAnimationDuration = 0; // milliseconds
    public const int MinimumAnimationDuration = 100; // milliseconds

    private readonly ILogger logger;
    private readonly IMessenger messenger;
    private readonly FiniteStateMachine<TState, TTrigger, Bindable> stateMachine;
    private readonly Dictionary<TState, WorkflowPage<TState, TTrigger>> pageIndex;
    private readonly Grid navigationGrid;

    private bool disposedValue;

    public ContentControl HostControl { get; private set; }

    public WorkflowManager(
        ILogger logger,
        IMessenger messenger,
        ContentControl hostControl,
        StateMachineDefinition<TState, TTrigger, Bindable> stateMachineDefinition)
    {
        this.logger = logger;
        this.messenger = messenger;
        this.HostControl = hostControl;
        this.stateMachine = new(this.logger);
        this.pageIndex = [];
        this.navigationGrid = new Grid()
        {
            Background = Brushes.Transparent,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        this.HostControl.Content = this.navigationGrid;
        this.CreateWorkflowStateMachine(stateMachineDefinition);
    }

    public WorkflowPage<TState, TTrigger>? GetPage(TState state)
        => this.pageIndex.TryGetValue(state, out var workflowPage) ? workflowPage : null;

    public WorkflowPage<TState, TTrigger>? ActivePage { get; private set; }

    public bool IsTransitioning { get; private set; }

    private void CreateWorkflowStateMachine(StateMachineDefinition<TState, TTrigger, Bindable> stateMachineDefinition)
    {
        try
        {
            this.stateMachine.Initialize(stateMachineDefinition);
            foreach (StateDefinition<TState, TTrigger, Bindable> stateDefinition in stateMachineDefinition.StateDefinitions)
            {
                TState state = stateDefinition.State;
                var page = stateDefinition.Tag as WorkflowPage<TState, TTrigger>;
                if (page is not null)
                {
                    page.WorkflowManager = this; 
                    this.pageIndex.Add(state, page);
                    var element = page.Control;
                    if ( element is not null )
                    { 
                        this.navigationGrid.Children.Add(element);
                        element.IsVisible = false;
                    }
                    else
                    {
                        this.logger.Error("Page missing its control - Improperly defined state machine: " + state.ToString());
                    }
                }
                else
                {
                    this.logger.Error("Null page or not Workflow Page: Improperly defined state machine: " + state.ToString());
                }
            }
        }
        catch (Exception e)
        {
            this.logger.Error("Improperly defined state machine:\n" + e.ToString());
            throw;
        }
    }

    public async Task Initialize()
    {
        foreach (var page in this.pageIndex.Values)
        {
            await page.OnInitialize();
        }
    }

    public async Task Shutdown()
    {
        foreach (var page in this.pageIndex.Values)
        {
            await page.OnShutdown();
        }
    }

    public async Task Start()
    {
        var newState = this.stateMachine.State;
        var activated = await this.ActivatePage(newState);
        this.UpdateVisuals();

        // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
        this.messenger.Publish(new NavigationMessage(activated, null));
    }

    public void UpdateVisuals() { /* TODO */ }  // => this.UpdateVisuals();

    public void ClearBackNavigation() => this.stateMachine.ClearBackNavigation();

    public async Task<bool> Next(int fadeDuration = DefaultAnimationDuration)
    {
        if (this.CanGoNext(out var _, out var _))
        {
            return await this.TryGoNext(fadeDuration);
        }

        return false;
    }

    public async Task<bool> Back(int fadeDuration = DefaultAnimationDuration)
    {
        if (this.CanGoBack(out TState _))
        {
            return await this.TryGoBack(fadeDuration);
        }

        return false;
    }

    public async Task<bool> Fire(TTrigger trigger, int fadeDuration = DefaultAnimationDuration)
    {
        if (this.CanFire(trigger))
        {
            return await this.TryFire(trigger, fadeDuration);
        }

        return false;
    }

    public bool CanGoBack(out TState state)
    {
        // We can go back if we are not transitioning and the navigation stack is not empty
        state = this.stateMachine.State;
        return !this.IsTransitioning && this.stateMachine.CanGoBack(out state);
    }

    public bool CanGoNext(out TState state, out TTrigger trigger)
    {
        state = default;
        trigger = default;
        if (this.IsTransitioning)
        {
            return false;
        }

        return this.stateMachine.CanGoNext(out state, out trigger);
    }

    public bool CanFire(TTrigger trigger) => this.stateMachine.CanFire(trigger);

    private async Task<bool> TryGoBack(int fadeDuration = DefaultAnimationDuration)
    {
        var oldState = this.stateMachine.State;
        if (!this.CanGoBack(out TState newState))
        {
            this.logger.Debug(string.Format("Cannot go back from: {0} ", oldState.ToString()));
            return false;
        }

        this.IsTransitioning = true;
        this.UpdateVisuals();
        this.stateMachine.GoBack();
        var deactivated = await this.DeactivatePage(oldState, fadeDuration);
        var activated = await this.ActivatePage(newState, fadeDuration);
        this.IsTransitioning = false;
        this.UpdateVisuals();

        // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
        this.messenger.Publish(new NavigationMessage(activated, deactivated));

        string message =
            string.Format("Backwards workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
        this.logger.Info(message);
        return true;
    }

    private async Task<bool> TryGoNext(int fadeDuration = DefaultAnimationDuration)
    {
        this.UpdateVisuals();
        var oldState = this.stateMachine.State;
        bool canGoNext = this.stateMachine.CanGoNext(out TState newState, out TTrigger trigger);
        if (canGoNext)
        {
            this.IsTransitioning = true;
            this.UpdateVisuals();
            this.stateMachine.GoNext(trigger);
            var deactivated = await this.DeactivatePage(oldState, fadeDuration);
            var activated = await this.ActivatePage(newState, fadeDuration);
            this.IsTransitioning = false;
            this.UpdateVisuals();

            // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
            this.messenger.Publish(new NavigationMessage(activated, deactivated));

            string message =
                string.Format("Forward workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
            this.logger.Info(message);
        }

        return canGoNext;
    }

    private async Task<bool> TryFire(TTrigger trigger, int fadeDuration)
    {
        this.UpdateVisuals();
        var oldState = this.stateMachine.State;
        bool canFire = this.stateMachine.CanFire(trigger);
        if (canFire)
        {
            this.IsTransitioning = true;
            this.UpdateVisuals();
            this.stateMachine.Fire(trigger);
            TState newState = this.stateMachine.State;
            var deactivated = await this.DeactivatePage(oldState, fadeDuration);
            var activated = await this.ActivatePage(newState, fadeDuration);
            this.IsTransitioning = false;
            this.UpdateVisuals();

            // Raise the Navigate weak event so that workflow related widgets, if any, will dismiss.
            this.messenger.Publish(new NavigationMessage(activated, deactivated));

            string message =
                string.Format("Forward workflow transition from: {0} to {1}", oldState.ToString(), newState.ToString());
            this.logger.Info(message);
        }

        return canFire;
    }

    private async Task<WorkflowPage<TState, TTrigger>?> DeactivatePage(
        TState oldState, int fadeDuration = DefaultAnimationDuration)
    {
        this.logger.Info("Orchestrator: Deactivating " + oldState.ToString());
        var deactivated = this.ActivePage;
        if (this.ActivePage != null)
        {
            await this.ActivePage.OnDeactivateAsync(oldState);
            if (fadeDuration > MinimumAnimationDuration)
            {
                // Fading out hides the control at the end of the animation
                // this.ActivePage.Control!.FadeOut(fadeDuration);
            }
            else
            {
                this.ActivePage.Control!.IsVisible = false;
            }

        }

        return deactivated;
    }

    private async Task<WorkflowPage<TState, TTrigger>> ActivatePage(
        TState newState, int fadeDuration = DefaultAnimationDuration)
    {
        // Get the new page, activates it
        this.logger.Info("Orchestrator: Activating " + newState.ToString());
        this.ActivePage = this.pageIndex[newState];
        if (fadeDuration > MinimumAnimationDuration)
        {
            // Fading in shows the control at the beginning of the animation
            // this.ActivePage.Control!.FadeIn(fadeDuration);
        }
        else
        {
            this.ActivePage.Control!.IsVisible = true;
        }

        await this.ActivePage.OnActivateAsync(newState);
        return this.ActivePage;
    }

    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                this.stateMachine.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            this.pageIndex.Clear();

            this.disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~WorkflowManager()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}