using System.Collections.Generic;
using System.Text.Json;
using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Api.Dtos;

public sealed record MoveEntityDto : SimEventDto
{
    public string EntityId { get; init; } = "";

    public Dictionary<string, JsonElement> Fields { get; init; } = new();

    public override SimEvent ToDomain() =>
        new MoveEntity(
            new Tick(Tick),
            EntityId,
            Fields
        );
}
