using System.Collections.Generic;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Events;

public sealed class SimEventComparer : IComparer<SimEvent>
{
    public static readonly SimEventComparer Instance = new();

    private SimEventComparer() { }

    public int Compare(SimEvent? x, SimEvent? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var tickCompare = x.Tick.CompareTo(y.Tick);
        if (tickCompare != 0)
            return tickCompare;

        // Tie-breaker: stable, deterministic ordering
        return string.CompareOrdinal(
            x.GetType().FullName,
            y.GetType().FullName
        );
    }
}
