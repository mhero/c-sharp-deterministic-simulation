using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Events;

public abstract record SimEvent
{
    public Tick Tick { get; }

    protected SimEvent(Tick tick)
    {
        Tick = tick;
    }
}
