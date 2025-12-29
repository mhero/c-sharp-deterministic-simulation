namespace DeterministicSimulation.Core.State;

public sealed class EntityState
{
    public int X { get; }
    public int Y { get; }

    public EntityState(int x, int y)
    {
        X = x;
        Y = y;
    }

    public EntityState Clone() => new EntityState(X, Y);
}
