using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Engine.Snapshot;

public sealed record SimulationSnapshot(
    Tick Tick,
    SimulationState State
);
