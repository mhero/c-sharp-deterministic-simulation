using System.Collections.Generic;

namespace DeterministicSimulation.Api.Dtos;

public sealed record RunRequest
{
    public InitialStateDto InitialState { get; init; } = default!;
    public int TargetTick { get; init; }
    public IReadOnlyList<SimEventDto> Events { get; init; } = [];
}
