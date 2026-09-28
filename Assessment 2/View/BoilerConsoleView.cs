using AppCore;
using BoilerController.Models;

namespace BoilerController.View;

/// <summary>
/// Console presentation layer with customized visual layout, cycle progress, and paginated logs.
/// </summary>
public class BoilerConsoleView
{
    private readonly ConsoleLayout _layout;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerConsoleView"/> class.
    /// </summary>
    public BoilerConsoleView()
    {
        _layout = new ConsoleLayout("BOILER CONTROLLER");
    }

    /// <summary>
    /// Displays the welcome banner on application start.
    /// </summary>
    public void DisplayWelcome()
    {
        _layout.Notify("Boiler Controller Initialized.", NotifyType.Info);
        Console.Beep();
    }

    /// <summary>
    /// Displays current status dashboard and styled menu options.
    /// </summary>
    /// <param name="boiler">Current boiler state.</param>
    /// <param name="remainingSeconds">Remaining seconds in timed cycle, if any.</param>
    public void DisplayMenu(BoilerModel boiler, int remainingSeconds = 0)
    {
        _layout.ClearContent();
        DisplayDashboard(boiler, remainingSeconds);

        _layout.WriteContent("┌────────────────────── PRIMARY MENU ───────────────────────┐");
        _layout.WriteContent("│  1. Start Boiler Sequence                                 │");
        _layout.WriteContent("│  2. Stop Boiler Sequence (Active in all running stages)   │");
        _layout.WriteContent("│  3. Simulate Boiler Error (Allowed in all stages)         │");
        _layout.WriteContent("│  4. Toggle Run Interlock Switch (Open / Closed)           │");
        _layout.WriteContent("│  5. Reset Lockout                                         │");
        _layout.WriteContent("│  6. View Event Log (Paginated with graceful erasing)      │");
        _layout.WriteContent("│  7. Exit Application                                      │");
        _layout.WriteContent("└───────────────────────────────────────────────────────────┘");
    }

    /// <summary>
    /// Prompts the user for a menu choice.
    /// </summary>
    /// <returns>Selected choice string.</returns>
    public string? PromptMenuChoice()
    {
        return _layout.Prompt("Select an option (1-7): ");
    }

    /// <summary>
    /// Prompts the user for a simulated failure description.
    /// </summary>
    /// <returns>Description of error.</returns>
    public string PromptErrorDescription()
    {
        string? input = _layout.Prompt("Enter error description (press Enter for default): ");

        return string.IsNullOrWhiteSpace(input) ? "Flame Failure Detected" : input.Trim();
    }

    /// <summary>
    /// Displays live countdown with an ASCII progress bar and available quick action keys.
    /// </summary>
    /// <param name="phaseName">Active phase name.</param>
    /// <param name="remainingSeconds">Seconds left.</param>
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
        _layout.WriteContent("  Keys: [S]top | [E]rror Simulate | [T]oggle Switch | [M]enu");
    }

    /// <summary>
    /// Displays recorded log records with pagination and graceful clearing between pages.
    /// </summary>
    /// <param name="logs">Collection of log entries.</param>
    public void DisplayEventLogs(IReadOnlyList<LogEntryModel> logs)
    {
        const int pageSize = 5;
        int totalLogs = logs.Count;

        if (totalLogs == 0)
        {
            _layout.ClearContent();
            _layout.WriteContent("┌─────────────────────────── AUDIT EVENT LOGS ───────────────────────────┐");
            _layout.WriteContent("│  No event records found in Boiler Log.txt.                             │");
            _layout.WriteContent("└────────────────────────────────────────────────────────────────────────┘");
            _layout.PressAnyKey();

            return;
        }

        int totalPages = (int)Math.Ceiling((double)totalLogs / pageSize);
        int currentPage = 1;
        bool inViewer = true;

        while (inViewer)
        {
            // Gracefully erase previous page content completely
            _layout.ClearContent();

            int startIndex = (currentPage - 1) * pageSize;
            int count = Math.Min(pageSize, totalLogs - startIndex);

            _layout.WriteContent("┌─────────────────────────── AUDIT EVENT LOGS ───────────────────────────┐");
            _layout.WriteContent(string.Format("│  Page {0} of {1}  (Total Events: {2}){3}│", currentPage, totalPages, totalLogs, new string(' ', Math.Max(0, 39 - totalLogs.ToString().Length))));
            _layout.WriteContent("├─────────────────────┼────────────────────────────┼──────────────────────┤");
            _layout.WriteContent(string.Format("│ {0,-19} │ {1,-26} │ {2,-20} │", "TIMESTAMP", "EVENT", "EVENT DATA"));
            _layout.WriteContent("├─────────────────────┼────────────────────────────┼──────────────────────┤");

            for (int i = 0; i < count; i++)
            {
                var log = logs[startIndex + i];
                string timeText = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                string eventName = log.Event.Length > 26 ? log.Event.Substring(0, 23) + "..." : log.Event;
                string eventData = log.EventData.Length > 20 ? log.EventData.Substring(0, 17) + "..." : log.EventData;

                _layout.WriteContent(string.Format("│ {0,-19} │ {1,-26} │ {2,-20} │", timeText, eventName, eventData));
            }

            _layout.WriteContent("└─────────────────────┴────────────────────────────┴──────────────────────┘");
            _layout.WriteContent("  [N]ext Page  |  [P]revious Page  |  [Q]uit to Main Menu");

            string? navChoice = _layout.Prompt("Navigation command (N/P/Q): ");
            string command = navChoice?.Trim().ToUpperInvariant() ?? string.Empty;

            switch (command)
            {
                case "N":
                    if (currentPage < totalPages)
                    {
                        currentPage++;
                    }
                    else
                    {
                        _layout.Notify("You are already on the last page.", NotifyType.Info);
                    }
                    break;

                case "P":
                    if (currentPage > 1)
                    {
                        currentPage--;
                    }
                    else
                    {
                        _layout.Notify("You are already on the first page.", NotifyType.Info);
                    }
                    break;

                case "Q":
                case "EXIT":
                case "":
                    inViewer = false;
                    break;

                default:
                    _layout.Notify("Invalid input. Enter 'N' for next, 'P' for previous, or 'Q' to quit.", NotifyType.Error);
                    break;
            }
        }
    }

    /// <summary>
    /// Displays an informational message.
    /// </summary>
    /// <param name="message">Message text.</param>
    public void NotifyInfo(string message)
    {
        _layout.Notify(message, NotifyType.Info);
    }

    /// <summary>
    /// Displays a success message.
    /// </summary>
    /// <param name="message">Message text.</param>
    public void NotifySuccess(string message)
    {
        _layout.Notify(message, NotifyType.Success);
    }

    /// <summary>
    /// Displays an error message.
    /// </summary>
    /// <param name="message">Message text.</param>
    public void NotifyError(string message)
    {
        _layout.Notify(message, NotifyType.Error);
    }

    /// <summary>
    /// Prompts user to press any key before returning to menu.
    /// </summary>
    public void PressAnyKey()
    {
        _layout.PressAnyKey();
    }

    private void DisplayDashboard(BoilerModel boiler, int remainingSeconds = 0)
    {
        string statusText = boiler.Status.ToString().ToUpperInvariant();
        if ((boiler.Status == BoilerStatus.PrePurge || boiler.Status == BoilerStatus.Ignition) && remainingSeconds > 0)
        {
            statusText += $" ({remainingSeconds}S)";
        }

        string statusBadge = $"[{statusText}]";
        string switchBadge = $"[{boiler.InterlockState.ToString().ToUpperInvariant()}]";

        _layout.WriteContent("╔═════════════════════ SYSTEM DASHBOARD ════════════════════╗");
        _layout.WriteContent(string.Format("║  System Status:  {0,-14}  Run Interlock: {1,-10}║", statusBadge, switchBadge));
        _layout.WriteContent("╚═══════════════════════════════════════════════════════════╝");
    }
}
