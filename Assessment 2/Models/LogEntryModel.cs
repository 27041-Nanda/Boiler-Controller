namespace BoilerController.Models;

/// <summary>
/// Represents an event log record.
/// </summary>
public class LogEntryModel
{
    /// <summary>
    /// Gets or sets the timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the name or category of the event.
    /// </summary>
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the associated event details.
    /// </summary>
    public string EventData { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntryModel"/> class.
    /// </summary>
    public LogEntryModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntryModel"/> class with parameters.
    /// </summary>
    /// <param name="timestamp">Timestamp of the event.</param>
    /// <param name="eventName">Designation of the event.</param>
    /// <param name="eventData">Details of the event.</param>
    public LogEntryModel(DateTime timestamp, string eventName, string eventData)
    {
        Timestamp = timestamp;
        Event = eventName;
        EventData = eventData;
    }
}
