namespace PozemiuRobotas;

public abstract class RobotModuleBase : IRobotModule
{
    public string Name { get; }
    public bool Enabled { get; private set; }

    protected RobotModuleBase(string name) => Name = name;

    public void Toggle() => Enabled = !Enabled;

    public virtual int ModifyVisionRadius(int baseRadius) => baseRadius;
    public virtual int ModifyMoveEnergyCost(int baseCost) => baseCost;
}
