using AppLogger;
using AppLogger.Formatters;
using AppLogger.Writers;
using BoilerController.Repository;
using BoilerController.Services;
using BoilerController.View;

namespace BoilerController;

/// <summary>
/// Application startup entry point with manual dependency injection wiring.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main execution entry point.
    /// </summary>
    public static void Main()
    {
        string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Boiler Log.txt");
        var csvFormatter = new CsvFormatter();
        var fileWriter = new FileLogWriter(logFilePath, csvFormatter.GetHeader());
        var logger = new Logger(fileWriter, csvFormatter, LogLevel.Info);

        IBoilerLogRepository logRepository = new BoilerLogRepository(logFilePath);
        IBoilerService boilerService = new BoilerService(logRepository, logger);
        var consoleView = new BoilerConsoleView();
        var controller = new BoilerController.Controller.BoilerController(boilerService, consoleView);

        controller.Run();
    }
}
