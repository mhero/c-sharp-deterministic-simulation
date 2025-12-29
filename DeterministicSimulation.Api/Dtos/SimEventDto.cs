using System.Text.Json.Serialization;
using DeterministicSimulation.Core.Events;

namespace DeterministicSimulation.Api.Dtos;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(MoveEntityDto), "MoveEntity")]
public abstract record SimEventDto
{
    public int Tick { get; init; }
    public abstract SimEvent ToDomain();
}
