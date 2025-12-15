namespace PozemiuRobotas;

public interface IRobotModule
{
    string Name { get; }
    bool Enabled { get; }
    void Toggle();

    int ModifyVisionRadius(int baseRadius);
    int ModifyMoveEnergyCost(int baseCost);
}

