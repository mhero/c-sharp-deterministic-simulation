using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.Time;

namespace DeterministicSimulation.Api.Dtos;

public sealed record MoveEntityDto : SimEventDto
{
    public string EntityId { get; init; } = "";
    public int Dx { get; init; }
    public int Dy { get; init; }

    public override SimEvent ToDomain() =>
        new MoveEntity(
            new Tick(Tick),
            EntityId,
            Dx,
            Dy
        );
}
