using System.Collections.Generic;
using System.Linq;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.State;

public sealed class SimulationState
{
    public Tick Tick { get; }
    public IReadOnlyDictionary<string, EntityState> Entities { get; }

    public SimulationState(
        Tick tick,
        IReadOnlyDictionary<string, EntityState> entities)
    {
        Tick = tick;

        Entities = new SortedDictionary<string, EntityState>(
            entities is Dictionary<string, EntityState> d
                ? d
                : new Dictionary<string, EntityState>(entities)
        );
    }

    public SimulationState Clone()
    {
        var copy = new Dictionary<string, EntityState>(Entities.Count);

        foreach (var (key, value) in Entities)
            copy[key] = value.Clone();

        return new SimulationState(Tick, copy);
    }
}
