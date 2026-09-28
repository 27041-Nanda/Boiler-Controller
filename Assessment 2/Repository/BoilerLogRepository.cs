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
    /// Appends a log entry to the log file according to CSV specifications.
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

    /// <summary>
    /// Retrieves and parses all entries from the Boiler Log file.
    /// </summary>
    /// <returns>List of parsed log records.</returns>
    public IReadOnlyList<LogEntryModel> GetAllLogs()
    {
        lock (_lockObject)
        {
            var logs = new List<LogEntryModel>();
            if (!File.Exists(_filePath))
            {
                return logs;
            }

            string[] lines = File.ReadAllLines(_filePath);

            // Skip CSV header line (index 0)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(',', 3);
                if (parts.Length >= 2)
                {
                    DateTime.TryParse(parts[0].Trim(), out DateTime timestamp);
                    string eventName = parts[1].Trim();
                    string eventData = parts.Length == 3 ? parts[2].Trim() : string.Empty;
                    logs.Add(new LogEntryModel(timestamp, eventName, eventData));
                }
            }

            return logs;
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
