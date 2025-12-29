using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Engine.Snapshot;

public sealed class SimulationSnapshot
{
    public Tick Tick { get; }
    public SimulationState State { get; }

    public SimulationSnapshot(SimulationState state)
    {
        Tick = state.Tick;
        State = state.Clone();
    }
}
