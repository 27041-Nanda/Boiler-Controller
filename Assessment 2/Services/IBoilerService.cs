using BoilerController.Models;

namespace BoilerController.Services;

/// <summary>
/// Contract for boiler business logic and simulation operations.
/// </summary>
public interface IBoilerService
{
    /// <summary>
    /// Gets the current boiler state model.
    /// </summary>
    BoilerModel CurrentBoiler { get; }

    /// <summary>
    /// Toggles the position of the run interlock switch.
    /// </summary>
    /// <returns>The newly toggled switch state.</returns>
    InterlockSwitchState ToggleInterlockSwitch();

    /// <summary>
    /// Resets the lockout status if the interlock switch is closed.
    /// </summary>
    /// <param name="message">Feedback message for the operation.</param>
    /// <returns>True if reset succeeded; otherwise, false.</returns>
    bool ResetLockout(out string message);

    /// <summary>
    /// Initiates the multi-phase boiler startup sequence with pre-purge and ignition timers.
    /// </summary>
    /// <param name="progressCallback">Callback reporting active phase and remaining seconds.</param>
    void StartBoilerSequence(Action<string, int> progressCallback);

    /// <summary>
    /// Stops the boiler sequence and transitions status back to Lockout.
    /// </summary>
    /// <param name="reason">Description of the stop request.</param>
    void StopBoilerSequence(string reason);

    /// <summary>
    /// Simulates a boiler operational failure (allowed only in Operational mode).
    /// </summary>
    /// <param name="errorDescription">Description of the simulated failure.</param>
    void SimulateError(string errorDescription);

    /// <summary>
    /// Retrieves all recorded event log records.
    /// </summary>
    /// <returns>Read-only list of log entries.</returns>
    IReadOnlyList<LogEntryModel> GetEventLogs();
}
