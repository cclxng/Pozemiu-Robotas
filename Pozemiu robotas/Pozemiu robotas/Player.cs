namespace PozemiuRobotas;

public sealed class Player
{
    private readonly List<IRobotModule> _modules;

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Keys { get; private set; }
    public int Energy { get; private set; }
    public bool Alive { get; private set; }

    public IEnumerable<IRobotModule> Modules => _modules;

    public Player(int startX, int startY, int energy, IEnumerable<IRobotModule> modules)
    {
        X = startX;
        Y = startY;
        Energy = energy;
        Alive = true;
        _modules = modules.ToList();
    }

    public int VisionRadius => ApplyModules(GameConfig.BaseVisionRadius, (m, v) => m.ModifyVisionRadius(v));
    public int MoveEnergyCost => ApplyModules(GameConfig.BaseMoveEnergyCost, (m, c) => m.ModifyMoveEnergyCost(c));

    public void MoveTo(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }

    public void AddKey() => Keys++;

    public bool TryUseKey()
    {
        if (Keys <= 0) return false;
        Keys--;
        return true;
    }

    public void ConsumeEnergy()
    {
        Energy -= MoveEnergyCost;
        if (Energy <= 0)
        {
            Energy = 0;
            Alive = false;
        }
    }

    public void Kill() => Alive = false;

    public void ToggleModule(int moduleIndex)
    {
        if (moduleIndex < 0 || moduleIndex >= _modules.Count) return;
        _modules[moduleIndex].Toggle();
    }

    private int ApplyModules(int baseValue, Func<IRobotModule, int, int> apply)
    {
        int value = baseValue;
        foreach (var module in _modules) value = apply(module, value);
        return value;
    }
}
