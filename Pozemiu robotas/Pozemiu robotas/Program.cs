// File: Program.cs
namespace PozemiuRobotas;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        var (normalized, start) = LevelLoader.Load(Levels.Level1);

        var map = new Map(normalized, new TileFactory());
        var modules = new IRobotModule[] { new SensorModule(), new EfficiencyModule() };
        var player = new Player(start.x, start.y, GameConfig.InitialEnergy, modules);

        var engine = new GameEngine(map, player);
        var renderer = new ConsoleRenderer();

        while (engine.State == GameState.Running)
        {
            renderer.RenderFrame(engine);

            var key = Console.ReadKey(true);
            var command = ConsoleCommandMapper.Map(key);
            command?.Execute(engine);
        }

        renderer.RenderFrame(engine);
        Console.WriteLine();
        Console.WriteLine(engine.State == GameState.Won
            ? "\nSveikinimai! Radote išėjimą."
            : "\nDeja, pralaimėjote.");

        Console.CursorVisible = true;
    }
}
