namespace PozemiuRobotas;

public sealed class ToggleModuleCommand : ICommand
{
    private readonly int _moduleIndex;

    public ToggleModuleCommand(int moduleIndex) => _moduleIndex = moduleIndex;

    public void Execute(GameEngine engine) => engine.ToggleModule(_moduleIndex);
}
