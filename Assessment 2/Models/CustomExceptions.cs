namespace BoilerController.Models;

/// <summary>
/// Exception thrown during invalid boiler operational requests.
/// </summary>
public class BoilerOperationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerOperationException"/> class.
    /// </summary>
    /// <param name="message">The exception description.</param>
    public BoilerOperationException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when safety interlock violations occur during active operation.
/// </summary>
public class InterlockSafetyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InterlockSafetyException"/> class.
    /// </summary>
    /// <param name="message">The exception description.</param>
    public InterlockSafetyException(string message) : base(message)
    {
    }
}
