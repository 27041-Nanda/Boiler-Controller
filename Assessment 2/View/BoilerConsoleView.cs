using AppCore;
using BoilerController.Models;

namespace BoilerController.View;

/// <summary>
/// Manages enhanced console interface presentation with visual customization and polish.
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
    /// Displays current status dashboard and styled main menu options.
    /// </summary>
    /// <param name="boiler">Current boiler state.</param>
    public void DisplayMenu(BoilerModel boiler)
    {
        _layout.ClearContent();
        DisplayDashboard(boiler);

        _layout.WriteContent("┌────────────────────── PRIMARY MENU ───────────────────────┐");
        _layout.WriteContent("│  1. Start Boiler Sequence                                 │");
        _layout.WriteContent("│  2. Stop Boiler Sequence                                  │");
        _layout.WriteContent("│  3. Simulate Boiler Error (Operational Mode Only)         │");
        _layout.WriteContent("│  4. Toggle Run Interlock Switch (Open / Closed)           │");
        _layout.WriteContent("│  5. Reset Lockout                                         │");
        _layout.WriteContent("│  6. View Event Log                                        │");
        _layout.WriteContent("│  7. Exit Application                                      │");
        _layout.WriteContent("└───────────────────────────────────────────────────────────┘");
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
        string? input = _layout.Prompt("Enter simulated error description (Press Enter for default): ");

        return string.IsNullOrWhiteSpace(input) ? "Flame Failure Detected" : input.Trim();
    }

    /// <summary>
    /// Displays an animated graphical progress bar during timed cycle transitions.
    /// </summary>
    /// <param name="phaseName">Active cycle phase designation.</param>
    /// <param name="remainingSeconds">Remaining seconds.</param>
    public void DisplayPhaseCountdown(string phaseName, int remainingSeconds)
    {
        _layout.ClearContent();
        int elapsedSeconds = 10 - remainingSeconds + 1;
        int percent = Math.Clamp(elapsedSeconds * 10, 0, 100);
        int filledCount = percent / 5;
        int emptyCount = 20 - filledCount;

        string progressBar = new string('█', filledCount) + new string('░', emptyCount);

        _layout.WriteContent("┌──────────────────── CYCLE IN PROGRESS ────────────────────┐");
        _layout.WriteContent($"│  Current Phase: {phaseName,-41} │");
        _layout.WriteContent($"│  Progress:      [{progressBar}] {percent,3}%               │");
        _layout.WriteContent($"│  Time Left:     {remainingSeconds,2} seconds remaining                      │");
        _layout.WriteContent("└───────────────────────────────────────────────────────────┘");
        _layout.WriteContent("  * Safety monitored - Opening interlock will trigger lockout.");
    }

    /// <summary>
    /// Displays all recorded log records in an elegant framed table.
    /// </summary>
    /// <param name="logs">Collection of log entries.</param>
    public void DisplayEventLogs(IReadOnlyList<LogEntryModel> logs)
    {
        _layout.ClearContent();
        _layout.WriteContent("┌─────────────────────────── AUDIT EVENT LOGS ───────────────────────────┐");

        if (logs.Count == 0)
        {
            _layout.WriteContent("│  No event records found in Boiler Log.txt.                             │");
            _layout.WriteContent("└────────────────────────────────────────────────────────────────────────┘");

            return;
        }

        _layout.WriteContent(string.Format("│ {0,-19} │ {1,-26} │ {2,-20} │", "TIMESTAMP", "EVENT", "EVENT DATA"));
        _layout.WriteContent("├─────────────────────┼────────────────────────────┼──────────────────────┤");

        foreach (var log in logs)
        {
            string timeText = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            string eventName = log.Event.Length > 26 ? log.Event.Substring(0, 23) + "..." : log.Event;
            string eventData = log.EventData.Length > 20 ? log.EventData.Substring(0, 17) + "..." : log.EventData;

            _layout.WriteContent(string.Format("│ {0,-19} │ {1,-26} │ {2,-20} │", timeText, eventName, eventData));
        }

        _layout.WriteContent("└─────────────────────┴────────────────────────────┴──────────────────────┘");
        _layout.WriteContent($"  Total recorded events: {logs.Count}");
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

    private void DisplayDashboard(BoilerModel boiler)
    {
        string statusBadge = $"[{boiler.Status.ToString().ToUpperInvariant()}]";
        string switchBadge = $"[{boiler.InterlockState.ToString().ToUpperInvariant()}]";

        _layout.WriteContent("╔═════════════════════ SYSTEM DASHBOARD ════════════════════╗");
        _layout.WriteContent($"║  System Status:  {statusBadge,-14}  Run Interlock: {switchBadge,-10}║");
        _layout.WriteContent("╚═══════════════════════════════════════════════════════════╝");
    }
}
