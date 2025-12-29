using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Api.Dtos;

public sealed record RunRequest(
    SimulationState InitialState,
    SimEvent[] Events,
    int TargetTick
);
