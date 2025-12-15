namespace PozemiuRobotas;

public sealed class MoveCommand : ICommand
{
    private readonly int _dx;
    private readonly int _dy;

    public MoveCommand(int dx, int dy)
    {
        _dx = dx;
        _dy = dy;
    }

    public void Execute(GameEngine engine) => engine.TryMovePlayerBy(_dx, _dy);
}
