using System;
using System.Collections.Generic;
using BoilerController.Models;

namespace BoilerController.Services;

/// <summary>
/// Service contract for managing boiler logic and state transitions.
/// </summary>
public interface IBoilerService
{
    /// <summary>
    /// Gets the current boiler state.
    /// </summary>
    BoilerModel CurrentBoiler { get; }

    /// <summary>
    /// Gets the seconds remaining in the active cycle.
    /// </summary>
    int RemainingSeconds { get; }

    /// <summary>
    /// Gets a value indicating whether a timed cycle (Pre-Purge or Ignition) is active.
    /// </summary>
    bool IsSequenceActive { get; }

    /// <summary>
    /// Toggles the run interlock switch between Open and Closed. Trips boiler if opened while running.
    /// </summary>
    /// <returns>The new switch state.</returns>
    InterlockSwitchState ToggleInterlockSwitch();

    /// <summary>
    /// Resets the lockout status if the interlock switch is closed.
    /// </summary>
    /// <param name="message">Feedback message for the user.</param>
    /// <returns>True if reset succeeded; otherwise, false.</returns>
    bool ResetLockout(out string message);

    /// <summary>
    /// Starts the asynchronous boiler startup sequence.
    /// </summary>
    /// <param name="onProgress">Optional callback reporting phase and remaining seconds.</param>
    void StartBoilerSequence(Action<string, int>? onProgress = null);

    /// <summary>
    /// Stops the boiler during any active stage (Pre-Purge, Ignition, or Operational).
    /// </summary>
    /// <param name="reason">Reason description.</param>
    void StopBoilerSequence(string reason);

    /// <summary>
    /// Simulates a failure at any stage of operation, immediately tripping to Lockout.
    /// </summary>
    /// <param name="errorDescription">Failure description.</param>
    void SimulateError(string errorDescription);

    /// <summary>
    /// Retrieves all recorded event logs.
    /// </summary>
    /// <returns>Read-only list of log entries.</returns>
    IReadOnlyList<LogEntryModel> GetEventLogs();
}
