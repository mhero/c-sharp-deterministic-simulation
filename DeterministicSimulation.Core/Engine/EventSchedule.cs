using System.Collections.Generic;
using System.Linq;
using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Engine;

public sealed class EventSchedule
{
    private readonly IReadOnlyList<SimEvent> _events;

    public EventSchedule(IEnumerable<SimEvent> events)
    {
        _events = [.. events
            .OrderBy(e => e.Tick)
            .ThenBy(e => e.GetType().FullName)];
    }

    public IEnumerable<SimEvent> ForTick(Tick tick) =>
        _events.Where(e => e.Tick == tick);
}
