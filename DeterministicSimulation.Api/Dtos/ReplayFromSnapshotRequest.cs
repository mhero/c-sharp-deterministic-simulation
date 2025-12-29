using DeterministicSimulation.Core.Engine.Snapshot;

namespace DeterministicSimulation.Api.Dtos;

public sealed class ReplayFromSnapshotRequest
{
    public RunRequest Run { get; init; } = default!;
    public SimulationSnapshot Snapshot { get; init; } = default!;
}
