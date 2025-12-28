using System.Collections.Generic;
using System.Linq;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Engine.Snapshot;

public sealed class SnapshotStore
{
    private readonly Dictionary<long, SimulationSnapshot> _snapshots = new();

    public void Save(SimulationSnapshot snapshot)
    {
        _snapshots[snapshot.Tick.Value] = snapshot;
    }

    public SimulationSnapshot? GetLatestAtOrBefore(Tick tick)
    {
        SimulationSnapshot? best = null;

        foreach (var kv in _snapshots)
        {
            if (kv.Key > tick.Value)
                continue;

            if (best is null || kv.Key > best.Tick.Value)
                best = kv.Value;
        }

        return best;
    }
}
