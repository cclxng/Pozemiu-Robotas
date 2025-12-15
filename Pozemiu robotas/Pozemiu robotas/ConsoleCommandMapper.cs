namespace PozemiuRobotas;

public static class ConsoleCommandMapper
{
    public static ICommand? Map(ConsoleKeyInfo key) => key.Key switch
    {
        ConsoleKey.UpArrow => new MoveCommand(0, -1),
        ConsoleKey.DownArrow => new MoveCommand(0, 1),
        ConsoleKey.LeftArrow => new MoveCommand(-1, 0),
        ConsoleKey.RightArrow => new MoveCommand(1, 0),

        ConsoleKey.D1 => new ToggleModuleCommand(0),
        ConsoleKey.D2 => new ToggleModuleCommand(1),

        ConsoleKey.Escape => new QuitCommand(),
        _ => null
    };
}

