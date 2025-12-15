namespace PozemiuRobotas;

public sealed class Map
{
    private readonly Tile[,] _grid;

    public int Width { get; }
    public int Height { get; }

    public Map(string[] layout, ITileFactory factory)
    {
        Height = layout.Length;
        Width = layout.Max(r => r.Length);
        _grid = new Tile[Width, Height];

        Fill(layout, factory);
    }

    public bool IsInsideBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public Tile GetTileAt(int x, int y) => _grid[x, y];

    public void DiscoverTilesInRadius(int centerX, int centerY, int radius)
    {
        int r2 = radius * radius;

        int minY = Math.Max(0, centerY - radius);
        int maxY = Math.Min(Height - 1, centerY + radius);
        int minX = Math.Max(0, centerX - radius);
        int maxX = Math.Min(Width - 1, centerX + radius);

        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                if (dx * dx + dy * dy <= r2)
                    _grid[x, y].Discover();
            }
    }

    private void Fill(string[] layout, ITileFactory factory)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                char c = x < layout[y].Length ? layout[y][x] : GameConfig.WallChar;
                _grid[x, y] = factory.Create(c);
            }
    }
}

