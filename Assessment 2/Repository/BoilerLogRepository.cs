using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoilerController.Models;

namespace BoilerController.Repository;

/// <summary>
/// Handles writing and reading event logs to Boiler Log.txt parallel to the executable.
/// Includes edge-case parsing for files containing extra commas or irregular formatting.
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
        EnsureFileExists();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerLogRepository"/> class with a specific path.
    /// </summary>
    /// <param name="filePath">Target log file path.</param>
    public BoilerLogRepository(string filePath)
    {
        _filePath = filePath;
        EnsureFileExists();
    }

    /// <summary>
    /// Appends a log entry to the file.
    /// </summary>
    /// <param name="entry">The log entry to append.</param>
    public void AppendLog(LogEntryModel entry)
    {
        lock (_lockObject)
        {
            string line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss}, {entry.Event}, {entry.EventData}";
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }

    /// <summary>
    /// Reads and parses all entries from the Boiler Log file.
    /// Gracefully handles edge cases such as extra trailing commas or internal commas in data.
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

            // Skip CSV header line if present
            int startIndex = 0;
            if (lines.Length > 0 && lines[0].IndexOf("Timestamp", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                startIndex = 1;
            }

            for (int i = startIndex; i < lines.Length; i++)
            {
                string rawLine = lines[i];
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    continue;
                }

                string trimmedLine = rawLine.Trim();

                // Edge Case 1: Handle extra trailing commas (e.g. "2026-09-28 10:00:00, Event, Data,")
                while (trimmedLine.EndsWith(",") && trimmedLine.Length > 0)
                {
                    trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 1).Trim();
                }

                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                // Split by comma
                string[] parts = trimmedLine.Split(',');
                if (parts.Length >= 2)
                {
                    DateTime timestamp = DateTime.Now;
                    if (!DateTime.TryParse(parts[0].Trim(), out timestamp))
                    {
                        timestamp = DateTime.Now;
                    }

                    string eventName = parts[1].Trim();

                    // Edge Case 2: Handle line with extra commas in event data or extra columns
                    // Join everything beyond index 1 back into event data
                    string eventData = string.Empty;
                    if (parts.Length > 2)
                    {
                        eventData = string.Join(",", parts.Skip(2)).Trim();
                    }

                    logs.Add(new LogEntryModel(timestamp, eventName, eventData));
                }
            }

            return logs;
        }
    }

    private void EnsureFileExists()
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
