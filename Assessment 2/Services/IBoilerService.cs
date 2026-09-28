namespace BoilerController.Services;


using BoilerController.Models;

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
}
