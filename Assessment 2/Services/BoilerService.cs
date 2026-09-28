using AppLogger;
using BoilerController.Models;
using BoilerController.Repository;

namespace BoilerController.Services;

/// <summary>
/// Implements business logic and non-blocking asynchronous state management for the boiler.
/// </summary>
public class BoilerService : IBoilerService
{
    private readonly IBoilerLogRepository _logRepository;
    private readonly Logger _logger;
    private readonly BoilerModel _boiler;
    private readonly object _stateLock = new object();
    private CancellationTokenSource? _cancellationTokenSource;
    private int _remainingSeconds;

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
    /// Gets the remaining seconds in active cycle.
    /// </summary>
    public int RemainingSeconds => _remainingSeconds;

    /// <summary>
    /// Gets whether a timed startup cycle is active.
    /// </summary>
    public bool IsSequenceActive => _boiler.Status == BoilerStatus.PrePurge || _boiler.Status == BoilerStatus.Ignition;

    /// <summary>
    /// Toggles the run interlock switch. Trips to Lockout if opened during active operation.
    /// </summary>
    /// <returns>The new switch position.</returns>
    public InterlockSwitchState ToggleInterlockSwitch()
    {
        lock (_stateLock)
        {
            _boiler.InterlockState = _boiler.InterlockState == InterlockSwitchState.Open
                ? InterlockSwitchState.Closed
                : InterlockSwitchState.Open;

            _logger.Info("Interlock Switch Toggled", $"Interlock Switch toggled to {_boiler.InterlockState}.");


            if (_boiler.InterlockState == InterlockSwitchState.Open &&
                (_boiler.Status == BoilerStatus.PrePurge ||
                 _boiler.Status == BoilerStatus.Ignition ||
                 _boiler.Status == BoilerStatus.Operational))
            {
                _cancellationTokenSource?.Cancel();
                _boiler.Status = BoilerStatus.Lockout;
                _remainingSeconds = 0;

                _logger.Error("Safety Trip", "Error: Interlock switch opened during operation. System in Lockout.");

                throw new InterlockSafetyException("Error: Interlock switch opened during operation. System in Lockout.");
            }

            return _boiler.InterlockState;
        }
    }

    /// <summary>
    /// Resets the lockout status if the interlock switch is closed.
    /// </summary>
    /// <param name="message">Feedback message for the user.</param>
    /// <returns>True if reset was successful; otherwise, false.</returns>
    public bool ResetLockout(out string message)
    {
        lock (_stateLock)
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
    }

    /// <summary>
    /// Initiates non-blocking boiler startup sequence with pre-purge and ignition timers.
    /// </summary>
    /// <param name="onProgress">Optional progress callback.</param>
    public void StartBoilerSequence(Action<string, int>? onProgress = null)
    {
        lock (_stateLock)
        {
            if (_boiler.Status != BoilerStatus.Ready)
            {
                throw new BoilerOperationException("Cannot start boiler: System must be in Ready status (reset lockout first).");
            }

            if (_boiler.InterlockState != InterlockSwitchState.Closed)
            {
                throw new BoilerOperationException("Cannot start boiler: Run Interlock switch must be Closed.");
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            Task.Run(() => ExecuteStartupCycle(token, onProgress), token);
        }
    }

    /// <summary>
    /// Stops the boiler during pre-purge, ignition, or operational stages.
    /// </summary>
    /// <param name="reason">Description of stop request.</param>
    public void StopBoilerSequence(string reason)
    {
        lock (_stateLock)
        {
            if (_boiler.Status == BoilerStatus.PrePurge ||
                _boiler.Status == BoilerStatus.Ignition ||
                _boiler.Status == BoilerStatus.Operational)
            {
                _cancellationTokenSource?.Cancel();
                _boiler.Status = BoilerStatus.Lockout;
                _remainingSeconds = 0;

                _logger.Info("Boiler Stopped", $"Boiler sequence stopped: {reason}. System in Lockout.");

                return;
            }

            throw new BoilerOperationException("Boiler is not currently active, starting, or operational.");
        }
    }

    /// <summary>
    /// Simulates a failure at any stage of operation, immediately tripping to Lockout.
    /// </summary>
    /// <param name="errorDescription">Failure description.</param>
    public void SimulateError(string errorDescription)
    {
        lock (_stateLock)
        {
            _cancellationTokenSource?.Cancel();
            _boiler.Status = BoilerStatus.Lockout;
            _remainingSeconds = 0;

            _logger.Error("Boiler Error", $"Error: {errorDescription}. System in Lockout.");
        }
    }

    /// <summary>
    /// Retrieves all recorded event logs from the repository.
    /// </summary>
    /// <returns>List of log entry models.</returns>
    public IReadOnlyList<LogEntryModel> GetEventLogs()
    {
        return _logRepository.GetAllLogs();
    }

    private void ExecuteStartupCycle(CancellationToken token, Action<string, int>? onProgress)
    {
        try
        {

            lock (_stateLock)
            {
                _boiler.Status = BoilerStatus.PrePurge;
            }

            for (int second = 10; second >= 1; second--)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _remainingSeconds = second;
                onProgress?.Invoke("Pre-Purge", second);
                Thread.Sleep(1000);
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            _logger.Info("Pre-Purge Cycle", "Pre-Purge completed.");


            lock (_stateLock)
            {
                _boiler.Status = BoilerStatus.Ignition;
            }

            for (int second = 10; second >= 1; second--)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _remainingSeconds = second;
                onProgress?.Invoke("Ignition", second);
                Thread.Sleep(1000);
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            _logger.Info("Ignition Phase", "Ignition phase completed.");


            lock (_stateLock)
            {
                _boiler.Status = BoilerStatus.Operational;
                _remainingSeconds = 0;
            }

            _logger.Info("Operational State", "Boiler now operational.");
            onProgress?.Invoke("Operational", 0);
        }
        catch (Exception ex)
        {
            lock (_stateLock)
            {
                _boiler.Status = BoilerStatus.Lockout;
                _remainingSeconds = 0;
            }

            _logger.Error("Boiler Startup Failed", ex.Message);
        }
    }
}
