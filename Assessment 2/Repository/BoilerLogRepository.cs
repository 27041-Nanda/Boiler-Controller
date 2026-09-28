using BoilerController.Models;

namespace BoilerController.Repository;

/// <summary>
/// Implements persistent event logging parallel to the application executable.
/// </summary>
public class BoilerLogRepository : IBoilerLogRepository
{
    private readonly string _filePath;
    private readonly object _lockObject = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerLogRepository"/> class.
    /// </summary>
    public BoilerLogRepository()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Boiler Log.txt");
        EnsureFileInitialized();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerLogRepository"/> class with specific file path.
    /// </summary>
    /// <param name="filePath">Target log file path.</param>
    public BoilerLogRepository(string filePath)
    {
        _filePath = filePath;
        EnsureFileInitialized();
    }

    /// <summary>
    /// Appends a log entry to the CSV file.
    /// </summary>
    /// <param name="entry">Log entry model.</param>
    public void AppendLog(LogEntryModel entry)
    {
        lock (_lockObject)
        {
            string line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss}, {entry.Event}, {entry.EventData}";
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }

    private void EnsureFileInitialized()
    {
        lock (_lockObject)
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "Timestamp, Event, Event Data" + Environment.NewLine);
            }
        }
    }
}
