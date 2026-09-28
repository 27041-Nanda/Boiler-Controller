using BoilerController.Models;
using BoilerController.Services;
using BoilerController.View;

namespace BoilerController.Controller;

/// <summary>
/// Coordinates user interface actions and service operations.
/// </summary>
public class BoilerController
{
    private readonly IBoilerService _boilerService;
    private readonly BoilerConsoleView _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoilerController"/> class.
    /// </summary>
    /// <param name="boilerService">Boiler service instance.</param>
    /// <param name="view">Console view instance.</param>
    public BoilerController(IBoilerService boilerService, BoilerConsoleView view)
    {
        _boilerService = boilerService;
        _view = view;
    }

    /// <summary>
    /// Executes the primary application workflow loop.
    /// </summary>
    public void Run()
    {
        _view.DisplayWelcome();
        bool isRunning = true;

        while (isRunning)
        {
            _view.DisplayMenu(_boilerService.CurrentBoiler);
            string? input = _view.PromptMenuChoice();

            switch (input?.Trim())
            {
                case "1":
                    try
                    {
                        _view.NotifyInfo("Initiating Boiler Start Sequence...");
                        _boilerService.StartBoilerSequence((phase, remaining) => _view.DisplayPhaseCountdown(phase, remaining));
                        _view.NotifySuccess("Boiler startup sequence completed successfully. Status: Operational.");
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
                    _view.NotifyInfo("Event Log Viewer will be implemented in Phase 4.");
                    _view.PressAnyKey();
                    break;

                case "7":
                    _view.NotifyInfo("Shutting down Boiler Controller. Goodbye!");
                    isRunning = false;
                    break;

                default:
                    _view.NotifyError("Invalid option selected. Please enter a number between 1 and 7.");
                    _view.PressAnyKey();
                    break;
            }
        }
    }
}
