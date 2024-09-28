namespace Lyt.Avalonia.Controls.Logging;

public partial class LogViewerWindow : Window, ILogger, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    private readonly SolidColorBrush greenBrushDebug;
    private readonly SolidColorBrush greenBrushInfo;
    private readonly SolidColorBrush orangeBrush;
    private readonly SolidColorBrush redBrush;
    private ObservableCollection<LogEntry> observableLogEntries;

    private List<LogEntry> AllLogEntries { get; set; }
    private bool showingAll;

    public LogViewerWindow()
    {
        this.InitializeComponent();
        this.showingAll = true;
        this.AllLogEntries = [];
        this.observableLogEntries = [];
        this.DataContext = this;
        this.greenBrushDebug = new SolidColorBrush(Color.FromRgb(0x9E, 0xD9, 0xFF));
        this.greenBrushInfo = new SolidColorBrush(Color.FromRgb(0x40, 0xE9, 0xAE));
        this.orangeBrush = new SolidColorBrush(Colors.DarkOrange);
        this.redBrush = new SolidColorBrush(Colors.Orchid);
    }

    public ObservableCollection<LogEntry> ObservableLogEntries
    {
        get => this.observableLogEntries;
        set
        {
            this.observableLogEntries = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ObservableLogEntries)));
        }
    }

    private static DateTime last = DateTime.Now;

    public static string ShortTimeString()
    {
        DateTime now = DateTime.Now;
        int deltaMs = (int)(now - last).TotalMilliseconds;
        string result = string.Format("{0}::{1} ({2}ms) - ", now.Second, now.Millisecond, deltaMs);
        last = now;
        return result;
    }

    public void Debug(string message) => this.Log(LogLevel.Debug, ShortTimeString() + message);

    public void Info(string message) => this.Log(LogLevel.Info, ShortTimeString() + message);

    public void Warning(string message) => this.Log(LogLevel.Warning, ShortTimeString() + message);

    public void Error(string message) => this.Log(LogLevel.Error, ShortTimeString() + message);

    public void Fatal(string message)
    {
        this.Error(message);
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(message);
    }

    private void Log(LogLevel logLevel, string message)
        => Dispatcher.UIThread.Post(() => { this.Update(logLevel, message); }, DispatcherPriority.Background);

    private void OnShowButtonClick(object sender, RoutedEventArgs e)
    {
        this.showingAll = !this.showingAll;
        this.ShowButton.Content = !this.showingAll ? "Show All Log" : "Warning and Errors Only";
        if (this.showingAll)
        {
            this.ObservableLogEntries = new ObservableCollection<LogEntry>(this.AllLogEntries);
        }
        else
        {
            var filtered =
                (from entry in this.AllLogEntries
                 where (entry.LogLevel == LogLevel.Warning) || (entry.LogLevel == LogLevel.Error)
                 select entry).ToList();
            this.ObservableLogEntries = new ObservableCollection<LogEntry>(filtered);
        }
    }

    private void Update(LogLevel logLevel, string message)
    {
        string time = DateTime.Now.ToLongTimeString();
        message = string.Concat(time, " - ", message);
        SolidColorBrush brush = this.redBrush;
        if (logLevel == LogLevel.Debug)
        {
            brush = this.greenBrushDebug;
            System.Diagnostics.Debug.WriteLine(message);
        }
        else if (logLevel == LogLevel.Info)
        {
            brush = this.greenBrushInfo;
            Trace.TraceInformation(message);
        }
        else if (logLevel == LogLevel.Warning)
        {
            brush = this.orangeBrush;
            Trace.TraceWarning(message);
        }
        else
        {
            Trace.TraceError(message);
        }

        var logEntry = new LogEntry { LogLevel = logLevel, Brush = brush, Message = message };
        this.AllLogEntries.Add(logEntry);
        if (this.showingAll)
        {
            this.ObservableLogEntries.Add(logEntry);
        }
        else
        {
            if ((logEntry.LogLevel == LogLevel.Error) || (logEntry.LogLevel == LogLevel.Warning))
            {
                this.ObservableLogEntries.Add(logEntry);
            }
        }

        this.itemsControl.ItemsSource = this.ObservableLogEntries;
        this.itemsControl.ScrollIntoView(this.ObservableLogEntries.Count - 1);
    }
}