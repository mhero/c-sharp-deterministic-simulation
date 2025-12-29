namespace DeterministicSimulation.Core.State;

public sealed class EntityState(int x, int y)
{
  public int X { get; } = x;
  public int Y { get; } = y;

  public EntityState Clone() => new(X, Y);
}
