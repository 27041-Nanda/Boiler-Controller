namespace AppLogger;

/// <summary>
/// Represents a structured log record.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Gets or sets the exact timestamp of the event.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets relevant supplementary data for the event.
    /// </summary>
    public string EventData { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public LogLevel Level { get; set; } = LogLevel.Info;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    public LogEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with specified values.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="eventData">Additional event details.</param>
    /// <param name="level">The severity level.</param>
    public LogEntry(string eventName, string eventData = "", LogLevel level = LogLevel.Info)
    {
        Timestamp = DateTime.Now;
        Event = eventName;
        EventData = eventData;
        Level = level;
    }
}
