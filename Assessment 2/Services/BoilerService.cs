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
    /// </summary>
    /// <returns>The newly toggled switch state.</returns>
    public InterlockSwitchState ToggleInterlockSwitch()
    {
        _boiler.InterlockState = _boiler.InterlockState == InterlockSwitchState.Open
            ? InterlockSwitchState.Closed
            : InterlockSwitchState.Open;

        _logger.Info("Interlock Switch Toggled", $"Interlock Switch toggled to {_boiler.InterlockState}.");

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
}
