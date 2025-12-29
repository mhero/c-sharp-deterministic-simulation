using System.Collections.Generic;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Api.Dtos;

public sealed record InitialStateDto
{
    public int Tick { get; init; }
    public Dictionary<string, EntityState> Entities { get; init; } = [];

    public SimulationState ToDomain() =>
        new(new Tick(Tick), Entities);
}
