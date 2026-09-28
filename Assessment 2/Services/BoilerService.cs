using AppLogger;
using BoilerController.Models;
using BoilerController.Repository;

namespace BoilerController.Services;

/// <summary>
/// Implements business logic and state management for the boiler controller.
/// </summary>
public class BoilerService : IBoilerService
{
    private readonly IBoilerLogRepository _logRepository;
    private readonly Logger _logger;
    private readonly BoilerModel _boiler;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerService"/> class.
    /// </summary>
    /// <param name="logRepository">Log repository instance.</param>
    /// <param name="logger">Application logger instance.</param>
    public BoilerService(IBoilerLogRepository logRepository, Logger logger)
    {
        _logRepository = logRepository;
        _logger = logger;
        _boiler = new BoilerModel
        {
            Status = BoilerStatus.Lockout,
            InterlockState = InterlockSwitchState.Open
        };

        _logger.Info("Boiler Initialized", "Initial status set to Lockout");
    }

    /// <summary>
    /// Gets the current boiler state model.
    /// </summary>
    public BoilerModel CurrentBoiler => _boiler;

    /// <summary>
    /// Toggles the position of the run interlock switch between Open and Closed.
    /// Trips system to Lockout if opened during active operations.
    /// </summary>
    /// <returns>The newly toggled switch state.</returns>
    public InterlockSwitchState ToggleInterlockSwitch()
    {
        _boiler.InterlockState = _boiler.InterlockState == InterlockSwitchState.Open
            ? InterlockSwitchState.Closed
            : InterlockSwitchState.Open;

        _logger.Info("Interlock Switch Toggled", $"Interlock Switch toggled to {_boiler.InterlockState}.");

        // Safety enforcement: If switch opened while running, immediately lockout
        if (_boiler.InterlockState == InterlockSwitchState.Open &&
            (_boiler.Status == BoilerStatus.PrePurge ||
             _boiler.Status == BoilerStatus.Ignition ||
             _boiler.Status == BoilerStatus.Operational))
        {
            _boiler.Status = BoilerStatus.Lockout;
            _logger.Error("Safety Trip", "Error: Interlock switch opened during operation. System in Lockout.");

            throw new InterlockSafetyException("Error: Interlock switch opened during operation. System in Lockout.");
        }

        return _boiler.InterlockState;
    }

    /// <summary>
    /// Resets the lockout status if the interlock switch is closed.
    /// </summary>
    /// <param name="message">Feedback message for the operation.</param>
    /// <returns>True if reset succeeded; otherwise, false.</returns>
    public bool ResetLockout(out string message)
    {
        if (_boiler.InterlockState == InterlockSwitchState.Closed)
        {
            _boiler.Status = BoilerStatus.Ready;
            _logger.Info("Boiler Status Changed", "Boiler Status changed to Ready");
            message = "Boiler status successfully transitioned to Ready.";

            return true;
        }

        message = "Run Interlock switch is Open. Close the switch before resetting lockout.";

        return false;
    }

    /// <summary>
    /// Executes the boiler startup sequence simulating pre-purge and ignition cycles.
    /// </summary>
    /// <param name="progressCallback">Progress callback indicating active phase and remaining seconds.</param>
    public void StartBoilerSequence(Action<string, int> progressCallback)
    {
        if (_boiler.Status != BoilerStatus.Ready)
        {
            throw new BoilerOperationException("Cannot start boiler: System must be in Ready status (reset lockout first).");
        }

        if (_boiler.InterlockState != InterlockSwitchState.Closed)
        {
            throw new BoilerOperationException("Cannot start boiler: Run Interlock switch must be Closed.");
        }

        // 1. Pre-Purge Cycle (10 seconds)
        _boiler.Status = BoilerStatus.PrePurge;
        for (int second = 10; second >= 1; second--)
        {
            progressCallback("Pre-Purge", second);
            Thread.Sleep(1000);
        }

        _logger.Info("Pre-Purge Cycle", "Pre-Purge completed.");

        // 2. Ignition Phase (10 seconds)
        _boiler.Status = BoilerStatus.Ignition;
        for (int second = 10; second >= 1; second--)
        {
            progressCallback("Ignition", second);
            Thread.Sleep(1000);
        }

        _logger.Info("Ignition Phase", "Ignition phase completed.");

        // 3. Operational State
        _boiler.Status = BoilerStatus.Operational;
        _logger.Info("Operational State", "Boiler now operational.");
    }

    /// <summary>
    /// Stops the boiler sequence and transitions back to Lockout.
    /// </summary>
    /// <param name="reason">Description of the stop cause.</param>
    public void StopBoilerSequence(string reason)
    {
        if (_boiler.Status == BoilerStatus.PrePurge ||
            _boiler.Status == BoilerStatus.Ignition ||
            _boiler.Status == BoilerStatus.Operational)
        {
            _boiler.Status = BoilerStatus.Lockout;
            _logger.Info("Boiler Stopped", $"Boiler sequence stopped: {reason}. System in Lockout.");

            return;
        }

        throw new BoilerOperationException("Boiler is not currently active or operational.");
    }

    /// <summary>
    /// Simulates a failure condition when boiler is in operational state.
    /// </summary>
    /// <param name="errorDescription">Failure description.</param>
    public void SimulateError(string errorDescription)
    {
        if (_boiler.Status != BoilerStatus.Operational)
        {
            throw new BoilerOperationException("Simulate error can only be used when the boiler is in Operational mode.");
        }

        _boiler.Status = BoilerStatus.Lockout;
        _logger.Error("Boiler Error", $"Error: {errorDescription}. System in Lockout.");
    }
}
