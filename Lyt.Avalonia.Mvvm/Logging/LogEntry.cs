namespace Lyt.Avalonia.Controls.Logging;

public sealed class LogEntry
{
    public LogLevel LogLevel { get; set; } = LogLevel.Info;

    public string? Message { get; set; } = string.Empty;

    public SolidColorBrush Brush { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);
}
