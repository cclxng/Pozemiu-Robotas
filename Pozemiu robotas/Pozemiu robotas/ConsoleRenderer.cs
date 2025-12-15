namespace PozemiuRobotas;

public sealed class ConsoleRenderer
{
    public void RenderFrame(GameEngine engine)
    {
        Console.SetCursorPosition(0, 0);

        for (int y = 0; y < engine.MapHeight; y++)
        {
            for (int x = 0; x < engine.MapWidth; x++)
                Console.Write(RenderTileChar(engine, x, y));

            Console.WriteLine();
        }

        RenderHud(engine);
    }

    private char RenderTileChar(GameEngine engine, int x, int y)
    {
        if (engine.Player.X == x && engine.Player.Y == y) return 'R';

        var tile = engine.GetTileAt(x, y);
        if (!IsTileVisible(engine, x, y, tile)) return ' ';

        return ConvertTileToChar(tile.Type);
    }

    private bool IsTileVisible(GameEngine engine, int x, int y, Tile tile)
    {
        int r = engine.Player.VisionRadius;
        int dx = x - engine.Player.X;
        int dy = y - engine.Player.Y;

        bool inVision = dx * dx + dy * dy <= r * r;
        return inVision && tile.IsDiscovered;
    }

    private void RenderHud(GameEngine engine)
    {
        Console.WriteLine();
        Console.WriteLine($"Energija: {engine.Player.Energy}   Raktai: {engine.Player.Keys}");

        var mods = engine.Player.Modules.Select((m, i) =>
            $"{i + 1}:{m.Name}[{(m.Enabled ? "Įjungtas" : "Išjungtas")}]");

        Console.WriteLine(string.Join("  ", mods));
        Console.WriteLine("Valdymas: Rodyklės – judėti, 1/2 – perjungti modulius, ESC – baigti");
    }

    private static char ConvertTileToChar(TileType type) => type switch
    {
        TileType.Empty => '.',
        TileType.Wall => '#',
        TileType.Key => 'K',
        TileType.Door => 'D',
        TileType.Trap => '^',
        TileType.Exit => 'E',
        _ => '?'
    };
}

