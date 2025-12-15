namespace PozemiuRobotas;

public sealed class GameEngine
{
    private readonly Map _map;

    public Player Player { get; }
    public GameState State { get; private set; }

    public int MapWidth => _map.Width;
    public int MapHeight => _map.Height;

    public GameEngine(Map map, Player player)
    {
        _map = map;
        Player = player;
        State = GameState.Running;

        _map.DiscoverTilesInRadius(Player.X, Player.Y, Player.VisionRadius);
    }

    public Tile GetTileAt(int x, int y) => _map.GetTileAt(x, y);

    public void ToggleModule(int moduleIndex)
    {
        Player.ToggleModule(moduleIndex);
        _map.DiscoverTilesInRadius(Player.X, Player.Y, Player.VisionRadius);
    }

    public void ForceLose() => State = GameState.Lost;

    public void TryMovePlayerBy(int dx, int dy)
    {
        if (State != GameState.Running) return;

        int targetX = Player.X + dx;
        int targetY = Player.Y + dy;

        if (!_map.IsInsideBounds(targetX, targetY)) return;

        var destination = _map.GetTileAt(targetX, targetY);
        if (destination.Type == TileType.Wall) return;

        if (!TryOpenDoorIfNeeded(destination)) return;

        Player.ConsumeEnergy();
        if (!Player.Alive) { State = GameState.Lost; return; }

        Player.MoveTo(targetX, targetY);
        ApplyTileEffects(destination);

        _map.DiscoverTilesInRadius(Player.X, Player.Y, Player.VisionRadius);
    }

    private bool TryOpenDoorIfNeeded(Tile destination)
    {
        if (destination.Type != TileType.Door) return true;
        if (!Player.TryUseKey()) return false;

        destination.ChangeTypeTo(TileType.Empty);
        return true;
    }

    private void ApplyTileEffects(Tile tile)
    {
        switch (tile.Type)
        {
            case TileType.Key:
                Player.AddKey();
                tile.ChangeTypeTo(TileType.Empty);
                break;

            case TileType.Trap:
                Player.Kill();
                State = GameState.Lost;
                break;

            case TileType.Exit:
                State = GameState.Won;
                break;
        }
    }
}
