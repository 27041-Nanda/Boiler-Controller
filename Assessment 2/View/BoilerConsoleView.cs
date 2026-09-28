using AppCore;
using BoilerController.Models;

namespace BoilerController.View;

/// <summary>
/// Manages console interface presentation using the ConsoleLayout helper.
/// </summary>
public class BoilerConsoleView
{
    private readonly ConsoleLayout _layout;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerConsoleView"/> class.
    /// </summary>
    public BoilerConsoleView()
    {
        _layout = new ConsoleLayout("BOILER STARTUP CONTROLLER");
    }

    /// <summary>
    /// Displays the welcoming initialization banner.
    /// </summary>
    public void DisplayWelcome()
    {
        _layout.Notify("Boiler Controller Initialized.", NotifyType.Info);
    }

    /// <summary>
    /// Displays current status and main menu options.
    /// </summary>
    /// <param name="boiler">Current boiler state.</param>
    public void DisplayMenu(BoilerModel boiler)
    {
        _layout.ClearContent();
        _layout.WriteContent($"System Status: [{boiler.Status}]  |  Run Interlock: [{boiler.InterlockState}]");
        _layout.WriteContent("────────────────────────────────────────────────────────");
        _layout.WriteContent("1. Start Boiler Sequence");
        _layout.WriteContent("2. Stop Boiler Sequence");
        _layout.WriteContent("3. Simulate Boiler Error");
        _layout.WriteContent("4. Toggle Run Interlock Switch (Open/Closed)");
        _layout.WriteContent("5. Reset Lockout");
        _layout.WriteContent("6. View Event Log");
        _layout.WriteContent("7. Exit Application");
        _layout.WriteContent("────────────────────────────────────────────────────────");
    }

    /// <summary>
    /// Prompts the user for a menu choice.
    /// </summary>
    /// <returns>Input string entered by user.</returns>
    public string? PromptMenuChoice()
    {
        return _layout.Prompt("Select an option (1-7): ");
    }

    /// <summary>
    /// Prompts the user for a simulated failure description.
    /// </summary>
    /// <returns>The description of the error.</returns>
    public string PromptErrorDescription()
    {
        string? input = _layout.Prompt("Enter simulated error description (or press Enter for default): ");

        return string.IsNullOrWhiteSpace(input) ? "Error While Operating" : input.Trim();
    }

    /// <summary>
    /// Displays countdown for the active simulation phase.
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    /// <param name="remainingSeconds">Seconds left in the countdown.</param>
    public void DisplayPhaseCountdown(string phaseName, int remainingSeconds)
    {
        _layout.ClearContent();
        _layout.WriteContent($"Active Phase: {phaseName}");
        _layout.WriteContent($"Time Remaining: {remainingSeconds} seconds...");
        _layout.WriteContent("Please wait while the cycle completes safely.");
    }

    /// <summary>
    /// Displays an informational notification banner.
    /// </summary>
    /// <param name="message">Message text.</param>
    public void NotifyInfo(string message)
    {
        _layout.Notify(message, NotifyType.Info);
    }

    /// <summary>
    /// Displays a success notification banner.
    /// </summary>
    /// <param name="message">Message text.</param>
    public void NotifySuccess(string message)
    {
        _layout.Notify(message, NotifyType.Success);
    }

    /// <summary>
    /// Displays an error notification banner.
    /// </summary>
    /// <param name="message">Message text.</param>
    public void NotifyError(string message)
    {
        _layout.Notify(message, NotifyType.Error);
    }

    /// <summary>
    /// Prompts the user to press any key to proceed.
    /// </summary>
    public void PressAnyKey()
    {
        _layout.PressAnyKey();
    }
}
