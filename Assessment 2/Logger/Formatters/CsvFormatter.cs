using System;

namespace AppLogger.Formatters;

/// <summary>
/// Formats log entries according to comma-separated values specification.
/// </summary>
public class CsvFormatter : ILogFormatter
{
    /// <summary>
    /// Gets the standard CSV header line.
    /// </summary>
    /// <returns>The header text.</returns>
    public string? GetHeader()
    {
        return "Timestamp, Event, Event Data";
    }

    /// <summary>
    /// Formats the log entry as CSV line matching the problem specification.
    /// </summary>
    /// <param name="entry">The log entry.</param>
    /// <returns>A comma-separated string.</returns>
    public string Format(LogEntry entry)
    {
        string timestampText = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

        return $"{timestampText}, {entry.Event}, {entry.EventData}";
    }
}
