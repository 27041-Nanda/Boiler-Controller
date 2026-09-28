namespace BoilerController.Models;

/// <summary>
/// Represents the operating states of the boiler system.
/// </summary>
public enum BoilerStatus
{
    /// <summary>
    /// Safety lockout state.
    /// </summary>
    Lockout,

    /// <summary>
    /// Ready state indicating system is reset and interlock is closed.
    /// </summary>
    Ready
}
