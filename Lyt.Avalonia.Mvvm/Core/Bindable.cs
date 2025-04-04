﻿// #define VERBOSE_Bindable

namespace Lyt.Avalonia.Mvvm.Core;

[AttributeUsage(AttributeTargets.All)]
public class DoNotLogAttribute : Attribute { }

/// <summary> Bindable class, aka a View Model.  </summary>
/// <remarks> All bound properties are held in a dictionary.</remarks>
public class Bindable : NotifyPropertyChanged, ISupportBehaviors
{
    private static readonly ILocalizer? StaticLocalizer;

    private static readonly ILogger StaticLogger;

    private static readonly IMessenger StaticMessenger;

    private static readonly IProfiler StaticProfiler;

    static Bindable()
    {
        try
        {
            StaticMessenger = ApplicationBase.GetRequiredService<IMessenger>();
            StaticLogger = ApplicationBase.GetRequiredService<ILogger>();
            StaticProfiler = ApplicationBase.GetRequiredService<IProfiler>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Missing essential services \n" + ex.ToString());
            throw;
        }

        try
        {
            StaticLocalizer = ApplicationBase.GetOptionalService<ILocalizer>();
            if (StaticLocalizer is null)
            {
                StaticLogger.Error("Missing localizer service, will not be able to localize. \n");
            }
        }
        catch (Exception ex)
        {
            StaticLogger.Error("Exception when trying to retrieve localizer service. \n" + ex.ToString());
        }
    }

    /// <summary> The property currently being set. </summary>
    /// <remarks> 
    /// Needed in some special cases to prevent spurious calls to Set from Avalonia controls, such as the radio button.
    /// =>> Last implemented solution was creating issues of view models non updating properly. 
    /// </remarks>
    //  private string setPropertyName = string.Empty;
    /// <summary> The bounds properties.</summary>
    protected readonly Dictionary<string, object?> properties = [];

    /// <summary> Actions to invoke for changing properties.</summary>
    protected readonly Dictionary<string, MethodInfo> actions = [];

    public Bindable(bool disablePropertyChangedLogging = false, bool disableAutomaticBindingsLogging = false)
        : this()
    {
        this.DisablePropertyChangedLogging = disablePropertyChangedLogging;
        this.DisableAutomaticBindingsLogging = disableAutomaticBindingsLogging;
    }

    public Bindable()
    {
        try
        {
            this.CreateAndBindCommands();
            this.CreateAndBindPropertyChangedActions();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to bind commands and property changed actions \n" + ex.ToString());
            throw;
        }
    }

#pragma warning disable IDE0079
#pragma warning disable CA1822 // Mark members as static

    public bool CanLocalize => StaticLocalizer is not null;

    public ILocalizer Localizer =>
        this.CanLocalize ? StaticLocalizer! : throw new Exception("Should have checked CanLocalize property.");

    public ILogger Logger => StaticLogger;

    public IMessenger Messenger => StaticMessenger;

    public IProfiler Profiler => StaticProfiler;

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079

    /// <summary> The control, its Data Context is this instance. </summary>
    /// <remarks> Aka, the "View" </remarks>
    public Control? Control { get; private set; }

    /// <summary> Allows to disable logging when properties are changing so that we do not flood the logs. </summary>
    /// <remarks> Use for quickly changing properties, mouse, sliders, etc.</remarks>
    public bool DisablePropertyChangedLogging { get; set; }

    /// <summary> Allows to disable logging of automatic bindings so that we do not flood the logs. </summary>
    public bool DisableAutomaticBindingsLogging { get; set; }

    /// <summary> Binds any object, when possible.</summary>
    public object? XamlView
    {
        get => this.Control;
        set
        {
            if (value is Control control)
            {
                this.Bind(control);
            }
        }
    }

    public List<object> Behaviors { get; private set; } = [];

    /// <summary> Binds a control and setup callbacks. </summary>
    public void BindOnDataContextChanged(Control control)
    {
        this.Control = control;
        this.OnDataBinding();
        this.Control.Loaded += (s, e) => this.OnViewLoaded();
    }

    /// <summary> Binds a control and setup callbacks. </summary>
    public void Bind(Control control)
    {
        this.Control = control;
        this.Unbind();
        try
        {
            this.Control.DataContext = this;
        }
        catch (InvalidCastException ex)
        {
            // Crash here ? This should never happen, ever. 
            // Major issue when defining the view, usually conflicting DaataContext by inheritance
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(this.GetType().FullName);
            Debug.WriteLine(this.Control.GetType().FullName);
            Debug.WriteLine(this.Control.DataContext?.GetType().FullName);
            Debug.WriteLine(ex);
        }

        this.OnDataBinding();
        this.Control.Loaded += (s, e) => this.OnViewLoaded();
    }

    /// <summary> Unbinds this bindable. </summary>
    public void Unbind()
    {
        if (this.Control is not null)
        {
            if (this.Control.DataContext != null)
            {
                this.Control.DataContext = null;
            }

            this.Control.Loaded -= (s, e) => this.OnViewLoaded();
        }
    }

    /// <summary> Unbinds the provided control. </summary>
    public static void Unbind(Control control)
    {
        if (control is not null)
        {
            if (control.DataContext is Bindable bindable)
            {
                bindable.Control = null;
                control.DataContext = null;
                control.Loaded -= (s, e) => bindable.OnViewLoaded();
            }
        }
    }

    /// <summary> Invoked when this bindable is bound </summary>
    protected virtual void OnDataBinding() { }

    /// <summary> Invoked when this bindable control is loaded. </summary>
    protected virtual void OnViewLoaded() { }

    /// <summary> Usually invoked when this bindable is about to be shown, but could be used for other purposes. </summary>
    public virtual void Activate(object? activationParameters) => this.LogActivation(activationParameters);

    /// <summary> Usually invoked when this bindable is about to be hidden, and same as above. </summary>
    public virtual void Deactivate() => this.LogDeactivation();

    /// <summary> Gets the value of a property </summary>
    public T? Get<T>([CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            // Bindable.Logger?.Fatal("Get property: no name");
            throw new Exception("Get property: no name");
        }

        return this.properties.TryGetValue(name, out object? value) ? value == null ? default : (T)value : default;
    }

    /// <summary> Sets the value of a property </summary>
    /// <returns> True, if the value was changed, false otherwise. </returns>
    public bool Set<T>(T? value, [CallerMemberName] string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // Bindable.Logger?.Fatal("Set property: no name");
            throw new Exception("Set property: no name");
        }

        T? current = this.Get<T>(name);
        if (Equals(value, current))
        {
            return false;
        }

        this.properties[name] = value;
        this.OnPropertyChanged(name);
        if (!this.DisablePropertyChangedLogging)
        {
            this.LogPropertyChanged(name, value);
        }

        if (this.actions.TryGetValue(name, out MethodInfo? methodInfo) && (methodInfo is not null))
        {
            methodInfo.Invoke(this, [current, value]);
        }

        return true;
    }

    /// <summary> Clear and Dispose when applicable, all properties </summary>
    protected void Clear()
    {
        foreach (object? property in this.properties.Values)
        {
            if (property is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        this.properties.Clear();
    }

    public virtual bool CanEscape { get; set; } = true;

    public virtual bool CanEnter { get; set; } = true;

    public virtual bool Validate() => true;

    public virtual bool TrySaveAndClose() => true;

    public virtual void CancelViewModel() { }

    public virtual void Cancel() { }

    private void CreateAndBindCommands()
    {
        var type = this.GetType();
        // Debug.WriteLine("CreateAndBindCommands: " + type.Name);
        PropertyInfo[] propertyInfos =
            type.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
        if (propertyInfos is null || propertyInfos.Length == 0)
        {
            return;
        }

        foreach (PropertyInfo property in propertyInfos)
        {
            string propertyName = property.Name;
            if (!propertyName.EndsWith("Command"))
            {
                continue;
            }

            if (property.PropertyType.Name != "ICommand")
            {
                continue;
            }

            if (this.properties.TryGetValue(propertyName, out object? value) && (value is not null))
            {
                continue;
            }

            string name = property.Name.Replace("Command", "");
            string methodName = string.Format("On{0}", name);

            MethodInfo? methodInfo =
                type.GetMethod(
                    methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            if (methodInfo is null)
            {
                continue;
            }

            this.properties[propertyName] = new Command(methodInfo, this);
            this.OnPropertyChanged(propertyName);
            if (!this.DisableAutomaticBindingsLogging)
            {
#if VERBOSE_Bindable
                this.Logger.Info(
                    string.Format("{0}: Command {1} has been bound to {2}", type.Name, propertyName, methodName));
#endif
            }
        }
    }

    private void CreateAndBindPropertyChangedActions()
    {
        var type = this.GetType();
        // Debug.WriteLine("CreateAndBindPropertyChangedActions: " + type.Name);
        var methodInfos =
            type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
        if (methodInfos is null || methodInfos.Length == 0)
        {
            this.Logger.Warning("Could not retreive instance methods for " + type.Name);
            return;
        }

        foreach (MethodInfo methodInfo in methodInfos)
        {
            string methodName = methodInfo.Name;
            if (!methodName.EndsWith("Changed"))
            {
                continue;
            }

            if (!methodName.StartsWith("On"))
            {
                continue;
            }

            string propertyName = methodInfo.Name.Replace("Changed", "");
            propertyName = propertyName.Replace("On", "");
            PropertyInfo? propertyInfo =
                type.GetProperty(
                    propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            if (propertyInfo is null)
            {
                continue;
            }

            this.actions.Add(propertyName, methodInfo);
            if (!this.DisableAutomaticBindingsLogging)
            {
#if VERBOSE_Bindable
                this.Logger.Info(
                    string.Format("{0}: Changes of property {1} have been bound to {2}", type.Name, propertyName, methodName));
#endif
            }
        }
    }

    #region Debug Utilities 

    /// <summary> Logs that a bindable is being deactivated. </summary>
    [Conditional("DEBUG")]
    private void LogDeactivation()
    {
        string typeName = this.GetType().Name;
        string message = string.Format("Deactivating {0}", typeName);
        this.Logger.Info(message);
    }

    /// <summary> Logs that a bindable is being activated. </summary>
    [Conditional("DEBUG")]
    private void LogActivation(object? parameter)
    {
        string parameterString =
            parameter is null ? "<null>" : parameter.GetType().Name + " - " + parameter.ToString();
        string typeName = this.GetType().Name;
        string message = string.Format("Activating {0} with {1}", typeName, parameterString);
        this.Logger.Info(message);
    }

    /// <summary> Logs that a property is changing. </summary>
    [Conditional("DEBUG")]
    private void LogPropertyChanged(string name, object? value)
    {
        int frameIndex = 1;
        string typeName;
        do
        {
            ++frameIndex;
            var frame = new StackFrame(frameIndex);
            var frameMethod = frame.GetMethod();
            if (frameMethod == null)
            {
                return;
            }

            var logAttribute = frameMethod.GetCustomAttribute<DoNotLogAttribute>();
            if (logAttribute is not null)
            {
                return;
            }

            typeName = frameMethod.DeclaringType!.Name;
        }
        while (typeName.StartsWith("Bindable"));

        ++frameIndex;
        var frameAbove = new StackFrame(frameIndex);
        var methodAbove = frameAbove.GetMethod();
        if (methodAbove is not null)
        {
            var logAttribute = methodAbove.GetCustomAttribute<DoNotLogAttribute>();
            if (logAttribute is not null)
            {
                return;
            }
        }

        string methodAboveName = methodAbove != null ? methodAbove.Name : "<none>";
        string message =
            string.Format(
                "From {0} in {1}: Property {2} changed to:   {3}",
                typeName, methodAboveName, name, value == null ? "null" : value.ToString());
        this.Logger.Info(message);
    }

    //[Conditional("DEBUG")]
    //private void Log()
    //{
    //    // TODO : Serialize all properties to JSon and then log the Json string 

    //}

    #endregion Debug Utilities 
}
