namespace AppLogger.Writers;

/// <summary>
/// Appends log entries to a file.
/// </summary>
public class FileLogWriter : ILogWriter
{
    private readonly string _filePath;
    private readonly object _lockObject = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogWriter"/> class.
    /// Creates the file and writes the header if it does not already exist.
    /// </summary>
    /// <param name="filePath">Target destination file path.</param>
    /// <param name="header">Optional header line to write upon file creation.</param>
    public FileLogWriter(string filePath, string? header = null)
    {
        _filePath = filePath;

        lock (_lockObject)
        {
            if (!File.Exists(_filePath))
            {
                string? directoryPath = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (header != null)
                {
                    File.WriteAllText(_filePath, header + Environment.NewLine);
                }
            }
        }
    }

    /// <summary>
    /// Appends the formatted log line to the file.
    /// </summary>
    /// <param name="message">The formatted log text.</param>
    public void Write(string message)
    {
        lock (_lockObject)
        {
            File.AppendAllText(_filePath, message + Environment.NewLine);
        }
    }
}
