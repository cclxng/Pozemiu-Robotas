namespace PozemiuRobotas;

public sealed class QuitCommand : ICommand
{
    public void Execute(GameEngine engine) => engine.ForceLose();
}
