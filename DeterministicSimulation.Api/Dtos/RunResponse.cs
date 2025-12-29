using DeterministicSimulation.Core.State;

namespace DeterministicSimulation.Api.Dtos;

public sealed record RunResponse(SimulationState FinalState);
