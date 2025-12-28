using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Engine;

public sealed class EventSchedule
{
    private readonly IReadOnlyList<SimEvent> _events;

    public EventSchedule(IEnumerable<SimEvent> events)
    {
        var list = new List<SimEvent>(events);
        list.Sort(SimEventComparer.Instance);
        _events = list;
    }

    public IEnumerable<SimEvent> ForTick(Tick tick)
    {
        foreach (var ev in _events)
        {
            if (ev.Tick < tick)
                continue;

            if (ev.Tick > tick)
                yield break;

            yield return ev;
        }
    }
}
