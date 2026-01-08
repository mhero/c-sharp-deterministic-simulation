using System.Collections.Generic;
using System.Linq;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.State;

public sealed class SimulationState(
    Tick tick,
    IReadOnlyDictionary<string, EntityState> entities)
{
    public Tick Tick { get; } = tick;
    public IReadOnlyDictionary<string, EntityState> Entities { get; } = new SortedDictionary<string, EntityState>(
        entities is Dictionary<string, EntityState> d
            ? d
            : new Dictionary<string, EntityState>(entities)
    );

    public SimulationState Clone()
    {
        var copy = new Dictionary<string, EntityState>(Entities.Count);

        foreach (var (key, value) in Entities)
            copy[key] = value.Clone();

        return new SimulationState(Tick, copy);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not SimulationState other) return false;
        if (!Tick.Equals(other.Tick)) return false;
        if (Entities.Count != other.Entities.Count) return false;

        foreach (var (key, value) in Entities)
        {
            if (!other.Entities.TryGetValue(key, out var oValue)) return false;
            if (!value.Equals(oValue)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = Tick.GetHashCode();
        foreach (var kv in Entities)
        {
            hash = hash * 31 + kv.Key.GetHashCode();
            hash = hash * 31 + kv.Value.GetHashCode();
        }
        return hash;
    }
}
