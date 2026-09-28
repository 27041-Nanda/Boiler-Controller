using System;
using AppLogger.Formatters;
using AppLogger.Writers;

namespace AppLogger;

/// <summary>
/// Central logger for recording application events.
/// </summary>
public class Logger
{
    private readonly ILogWriter _writer;
    private readonly ILogFormatter _formatter;
    private readonly LogLevel _minimumLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="writer">The output destination writer.</param>
    /// <param name="formatter">The message formatter.</param>
    /// <param name="minimumLevel">The minimum log level to record.</param>
    public Logger(ILogWriter writer, ILogFormatter formatter, LogLevel minimumLevel = LogLevel.Info)
    {
        _writer = writer;
        _formatter = formatter;
        _minimumLevel = minimumLevel;
    }

    /// <summary>
    /// Records a structured log entry.
    /// </summary>
    /// <param name="eventName">Event designation.</param>
    /// <param name="eventData">Associated event details.</param>
    /// <param name="level">Log severity.</param>
    public void Log(string eventName, string eventData = "", LogLevel level = LogLevel.Info)
    {
        if (level < _minimumLevel)
        {
            return;
        }

        var entry = new LogEntry(eventName, eventData, level);
        string formattedMessage = _formatter.Format(entry);
        _writer.Write(formattedMessage);
    }

    /// <summary>
    /// Records an informational log entry.
    /// </summary>
    /// <param name="eventName">Event designation.</param>
    /// <param name="eventData">Associated event details.</param>
    public void Info(string eventName, string eventData = "")
    {
        Log(eventName, eventData, LogLevel.Info);
    }

    /// <summary>
    /// Records an error log entry.
    /// </summary>
    /// <param name="eventName">Event designation.</param>
    /// <param name="eventData">Associated event details.</param>
    public void Error(string eventName, string eventData = "")
    {
        Log(eventName, eventData, LogLevel.Error);
    }
}
