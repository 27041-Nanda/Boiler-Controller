namespace AppLogger;

/// <summary>
/// Represents configuration properties for application logging.
/// </summary>
public class LogConfig
{
    /// <summary>
    /// Gets or sets the target file name or relative path.
    /// </summary>
    public string FilePath { get; set; } = "Boiler Log.txt";

    /// <summary>
    /// Gets or sets the active formatter name.
    /// </summary>
    public string Formatter { get; set; } = "Csv";

    /// <summary>
    /// Gets or sets the minimum active log level.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;
}
