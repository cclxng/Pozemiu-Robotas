namespace PozemiuRobotas;

public sealed class EfficiencyModule : RobotModuleBase
{
    public EfficiencyModule() : base("Efektyvumo modulis") { }

    public override int ModifyMoveEnergyCost(int baseCost)
        => Enabled ? Math.Max(1, baseCost / 2) : baseCost;
}

