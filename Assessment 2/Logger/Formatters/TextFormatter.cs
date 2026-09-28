using System;

namespace AppLogger.Formatters;

/// <summary>
/// Formats log entries into human-readable plain text.
/// </summary>
public class TextFormatter : ILogFormatter
{
    /// <summary>
    /// Gets the header for plain text output.
    /// </summary>
    /// <returns>Null for plain text.</returns>
    public string? GetHeader()
    {
        return null;
    }

    /// <summary>
    /// Formats the log entry into text.
    /// </summary>
    /// <param name="entry">The log entry.</param>
    /// <returns>The formatted plain text string.</returns>
    public string Format(LogEntry entry)
    {
        return $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Level}] {entry.Event} - {entry.EventData}";
    }
}
