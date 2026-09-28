using BoilerController.Models;
using BoilerController.Services;
using BoilerController.View;

namespace BoilerController.Controller;

/// <summary>
/// Controller coordinating user input and service calls with full interactive operations at any stage.
/// </summary>
public class BoilerController
{
    private readonly IBoilerService _boilerService;
    private readonly BoilerConsoleView _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerController"/> class.
    /// </summary>
    /// <param name="boilerService">Boiler service.</param>
    /// <param name="view">Console view.</param>
    public BoilerController(IBoilerService boilerService, BoilerConsoleView view)
    {
        _boilerService = boilerService;
        _view = view;
    }

    /// <summary>
    /// Runs the main interaction loop.
    /// </summary>
    public void Run()
    {
        _view.DisplayWelcome();
        bool isRunning = true;

        while (isRunning)
        {
            _view.DisplayMenu(_boilerService.CurrentBoiler, _boilerService.RemainingSeconds);
            string? choice = _view.PromptMenuChoice();

            switch (choice?.Trim())
            {
                case "1":
                    try
                    {
                        _boilerService.StartBoilerSequence();
                        _view.NotifyInfo("Startup sequence started. Press S to stop, E to error, T to toggle, M for menu.");

                        // Monitor active cycle while allowing keyboard actions on the fly
                        while (_boilerService.IsSequenceActive)
                        {
                            string phase = _boilerService.CurrentBoiler.Status.ToString();
                            int remaining = _boilerService.RemainingSeconds;
                            _view.DisplayPhaseCountdown(phase, remaining);

                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(true).Key;
                                if (key == ConsoleKey.S)
                                {
                                    _boilerService.StopBoilerSequence("Stopped during cycle by operator");
                                    _view.NotifySuccess("Boiler stopped during cycle. System transitioned to Lockout.");
                                    break;
                                }

                                if (key == ConsoleKey.E)
                                {
                                    string err = _view.PromptErrorDescription();
                                    _boilerService.SimulateError(err);
                                    _view.NotifyError($"Error: {err}. System in Lockout.");
                                    break;
                                }

                                if (key == ConsoleKey.T)
                                {
                                    try
                                    {
                                        var switchState = _boilerService.ToggleInterlockSwitch();
                                        _view.NotifySuccess($"Interlock Switch toggled to {switchState}.");
                                    }
                                    catch (InterlockSafetyException ex)
                                    {
                                        _view.NotifyError(ex.Message);
                                    }
                                    break;
                                }

                                if (key == ConsoleKey.M || key == ConsoleKey.Escape)
                                {
                                    _view.NotifyInfo("Returned to Main Menu. Cycle continues running in background.");
                                    break;
                                }
                            }

                            Thread.Sleep(200);
                        }

                        if (_boilerService.CurrentBoiler.Status == BoilerStatus.Operational)
                        {
                            _view.NotifySuccess("Boiler startup sequence completed successfully. Status: Operational.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _view.NotifyError(ex.Message);
                    }
                    _view.PressAnyKey();
                    break;

                case "2":
                    try
                    {
                        _boilerService.StopBoilerSequence("Stopped manually by operator");
                        _view.NotifySuccess("Boiler sequence stopped. System transitioned to Lockout.");
                    }
                    catch (Exception ex)
                    {
                        _view.NotifyError(ex.Message);
                    }
                    _view.PressAnyKey();
                    break;

                case "3":
                    try
                    {
                        string errorDesc = _view.PromptErrorDescription();
                        _boilerService.SimulateError(errorDesc);
                        _view.NotifyError($"Error: {errorDesc}. System in Lockout.");
                    }
                    catch (Exception ex)
                    {
                        _view.NotifyError(ex.Message);
                    }
                    _view.PressAnyKey();
                    break;

                case "4":
                    try
                    {
                        var newState = _boilerService.ToggleInterlockSwitch();
                        _view.NotifySuccess($"Interlock Switch toggled to {newState}.");
                    }
                    catch (InterlockSafetyException ex)
                    {
                        _view.NotifyError(ex.Message);
                    }
                    _view.PressAnyKey();
                    break;

                case "5":
                    if (_boilerService.ResetLockout(out string message))
                    {
                        _view.NotifySuccess(message);
                    }
                    else
                    {
                        _view.NotifyError(message);
                    }
                    _view.PressAnyKey();
                    break;

                case "6":
                    var logs = _boilerService.GetEventLogs();
                    _view.DisplayEventLogs(logs);
                    break;

                case "7":
                    _view.NotifyInfo("Exiting Boiler Controller. Goodbye!");
                    isRunning = false;
                    break;

                default:
                    _view.NotifyError("Invalid option selected. Please enter a number from 1 to 7.");
                    _view.PressAnyKey();
                    break;
            }
        }
    }
}
