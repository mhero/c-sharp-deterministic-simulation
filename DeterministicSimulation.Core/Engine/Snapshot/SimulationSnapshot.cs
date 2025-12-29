using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Engine.Snapshot;

public sealed class SimulationSnapshot(SimulationState state)
{
  public Tick Tick { get; } = state.Tick;
  public SimulationState State { get; } = state.Clone();
}
