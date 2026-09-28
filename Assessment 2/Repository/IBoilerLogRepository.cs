using BoilerController.Models;

namespace BoilerController.Repository;

/// <summary>
/// Contract for persisting and retrieving boiler controller event logs.
/// </summary>
public interface IBoilerLogRepository
{
    /// <summary>
    /// Appends a new event log entry to the persistent log file.
    /// </summary>
    /// <param name="entry">The log entry to append.</param>
    void AppendLog(LogEntryModel entry);

    /// <summary>
    /// Retrieves all recorded event logs from the persistent log file.
    /// </summary>
    /// <returns>A read-only collection of log entry models.</returns>
    IReadOnlyList<LogEntryModel> GetAllLogs();
}
