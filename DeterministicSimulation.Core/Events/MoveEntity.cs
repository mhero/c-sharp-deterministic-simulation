using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Core.Events;

public sealed record MoveEntity(
    Tick Tick,
    string EntityId,
    int Dx,
    int Dy
) : SimEvent(Tick);
