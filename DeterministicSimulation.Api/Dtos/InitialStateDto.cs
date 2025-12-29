using System.ComponentModel.DataAnnotations;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Api.Dtos;

public sealed class InitialStateDto
{
    [Required]
    public int Tick { get; init; }

    [Required]
    public Dictionary<string, EntityStateDto> Entities { get; init; } = [];

    public SimulationState ToDomain()
    {
        var entities = Entities.ToDictionary(
            e => e.Key,
            e => e.Value.ToDomain()
        );

        return new SimulationState(new Tick(Tick), entities);
    }
}
