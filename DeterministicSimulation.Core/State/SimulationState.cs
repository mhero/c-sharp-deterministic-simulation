using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.State;

public sealed record SimulationState
{
    public Tick Tick { get; }
    public IReadOnlyDictionary<string, EntityState> Entities { get; }

    public SimulationState(
        Tick tick,
        IReadOnlyDictionary<string, EntityState> entities)
    {
        Tick = tick;
        Entities = entities;
    }

    public static SimulationState Initial()
        => new SimulationState(
            Tick.Zero,
            new Dictionary<string, EntityState>()
        );
}
