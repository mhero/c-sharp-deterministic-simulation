using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using DeterministicSimulation.Core.Time;
using System;

namespace DeterministicSimulation.Core.Events;

public sealed record MoveEntity : SimEvent
{
    public string EntityId { get; }
    public ImmutableSortedDictionary<string, JsonElement> Fields { get; }

    public MoveEntity(
        Tick tick,
        string entityId,
        IDictionary<string, JsonElement> fields
    ) : base(tick)
    {
        EntityId = entityId;
        Fields = fields.ToImmutableSortedDictionary(StringComparer.Ordinal);
    }
}
