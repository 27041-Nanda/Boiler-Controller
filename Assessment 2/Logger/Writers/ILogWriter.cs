namespace AppLogger.Writers;

/// <summary>
/// Defines the destination writer contract for log outputs.
/// </summary>
public interface ILogWriter
{
    /// <summary>
    /// Writes a log message string to the destination.
    /// </summary>
    /// <param name="message">The formatted log message.</param>
    void Write(string message);
}
