namespace PozemiuRobotas;

public sealed class TileFactory : ITileFactory
{
    public Tile Create(char c) => new Tile(CharToType(c));

    private static TileType CharToType(char c) => c switch
    {
        GameConfig.WallChar => TileType.Wall,
        GameConfig.KeyChar => TileType.Key,
        GameConfig.DoorChar => TileType.Door,
        GameConfig.TrapChar => TileType.Trap,
        GameConfig.ExitChar => TileType.Exit,
        _ => TileType.Empty
    };
}
