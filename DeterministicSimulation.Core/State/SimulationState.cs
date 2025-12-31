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
}
