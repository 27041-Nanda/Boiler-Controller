namespace BoilerController.Repository;

using BoilerController.Models;

/// <summary>
/// Contract for persisting boiler controller event logs.
/// </summary>
public interface IBoilerLogRepository
{
    /// <summary>
    /// Appends a new event log entry to the persistent log file.
    /// </summary>
    /// <param name="entry">The log entry to append.</param>
    void AppendLog(LogEntryModel entry);
}
