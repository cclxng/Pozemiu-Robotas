namespace PozemiuRobotas;

public sealed class Tile
{
    public TileType Type { get; private set; }
    public bool IsDiscovered { get; private set; }

    public Tile(TileType type)
    {
        Type = type;
        IsDiscovered = false;
    }

    public void Discover() => IsDiscovered = true;

    public void ChangeTypeTo(TileType newType) => Type = newType;
}
