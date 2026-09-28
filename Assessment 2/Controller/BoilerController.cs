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
                    _view.NotifyInfo("Start Boiler Sequence will be implemented in Phase 2.");
                    _view.PressAnyKey();
                    break;

                case "2":
                    _view.NotifyInfo("Stop Boiler Sequence will be implemented in Phase 3.");
                    _view.PressAnyKey();
                    break;

                case "3":
                    _view.NotifyInfo("Simulate Boiler Error will be implemented in Phase 3.");
                    _view.PressAnyKey();
                    break;

                case "4":
                    var newState = _boilerService.ToggleInterlockSwitch();
                    _view.NotifySuccess($"Interlock Switch toggled to {newState}.");
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
