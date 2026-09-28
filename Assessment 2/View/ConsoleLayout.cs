namespace AppCore;

public enum NotifyType { Info, Success, Error }

public class ConsoleLayout
{
    private readonly int _notifyRow = 3;
    private readonly int _contentRow = 5;
    private readonly int _width;

    /// <summary>
    /// Initializes a new instance of the ConsoleLayout class and displays the application title in a formatted console
    /// window.
    /// </summary>
    /// <param name="appTitle">The title of the application to display in the console header.</param>
    public ConsoleLayout(string appTitle)
    {
        Console.Clear();
        Console.CursorVisible = true;
        _width = Math.Max(Console.WindowWidth, 44);
        int pad = _width - 2;

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("╔" + new string('═', pad) + "╗");

        Console.Write("║");
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.White;
        string inner = appTitle.PadLeft((pad + appTitle.Length) / 2).PadRight(pad);
        Console.Write(inner);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("║");

        Console.WriteLine("╚" + new string('═', pad) + "╝");
        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('─', _width));
        Console.ResetColor();
    }

    /// <summary>
    /// Notify the through console notify bar.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    public void Notify(string message, NotifyType type = NotifyType.Info)
    {
        int savedLeft = Console.CursorLeft;
        int savedTop = Console.CursorTop;

        Console.SetCursorPosition(0, _notifyRow);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, _notifyRow);

        var (prefix, prefixColor, bgColor) = type switch
        {
            NotifyType.Success => (" OK  ", ConsoleColor.White, ConsoleColor.DarkGreen),
            NotifyType.Error => (" ERR ", ConsoleColor.White, ConsoleColor.DarkRed),
            _ => ("INFO", ConsoleColor.Black, ConsoleColor.DarkCyan)
        };

        Console.Write("  ");
        Console.BackgroundColor = bgColor;
        Console.ForegroundColor = prefixColor;
        Console.Write(prefix);
        Console.ResetColor();

        Console.ForegroundColor = type switch
        {
            NotifyType.Success => ConsoleColor.Green,
            NotifyType.Error => ConsoleColor.Red,
            _ => ConsoleColor.Cyan
        };
        Console.Write($" {message}");
        Console.ResetColor();

        Console.SetCursorPosition(savedLeft, savedTop);
    }


    /// <summary>
    /// Used to clear the contents in the screen.
    /// </summary>
    public void ClearContent()
    {
        for (int row = _contentRow; row < Console.WindowHeight; row++)
        {
            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth));
        }
        Console.SetCursorPosition(0, _contentRow);
    }

    /// <summary>
    /// Used to write content to the screen.
    /// </summary>
    /// <param name="line"></param>
    public void WriteContent(string line)
    {
        Console.WriteLine($"  {line}");
    }

    /// <summary>
    /// Used to display any user prompts.
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public string? Prompt(string label)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($">>> {label}");
        Console.ResetColor();
        return Console.ReadLine();
    }

    /// <summary>
    /// Displays a prompt instructing the user to press any key and waits for a key press.
    /// </summary>
    public void PressAnyKey()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}
