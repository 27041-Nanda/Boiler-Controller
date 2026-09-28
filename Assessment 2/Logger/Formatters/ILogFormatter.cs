namespace AppLogger.Formatters;

/// <summary>
/// Defines formatting contracts for log entries.
/// </summary>
public interface ILogFormatter
{
    /// <summary>
    /// Gets the header string for formatted files, if applicable.
    /// </summary>
    /// <returns>The header text or null.</returns>
    string? GetHeader();

    /// <summary>
    /// Formats a log entry into a string.
    /// </summary>
    /// <param name="entry">The log entry to format.</param>
    /// <returns>The formatted log string.</returns>
    string Format(LogEntry entry);
}
