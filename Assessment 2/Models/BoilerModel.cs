namespace BoilerController.Models;

/// <summary>
/// Model having runtime state of the boiler controller.
/// </summary>
public class BoilerModel
{
    /// <summary>
    /// Gets or sets the current operating status.
    /// </summary>
    public BoilerStatus Status { get; set; } = BoilerStatus.Lockout;

    /// <summary>
    /// Gets or sets the run interlock switch position.
    /// </summary>
    public InterlockSwitchState InterlockState { get; set; } = InterlockSwitchState.Open;
}
