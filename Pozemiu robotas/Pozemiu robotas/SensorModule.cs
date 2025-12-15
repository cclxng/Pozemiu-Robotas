namespace PozemiuRobotas;

public sealed class SensorModule : RobotModuleBase
{
    private const int VisionBonus = 2;

    public SensorModule() : base("Jutiklių modulis") { }

    public override int ModifyVisionRadius(int baseRadius)
        => Enabled ? baseRadius + VisionBonus : baseRadius;
}
