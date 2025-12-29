using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DeterministicSimulation.Core.Engine;
using DeterministicSimulation.Core.Engine.Snapshot;
using DeterministicSimulation.Core.Time;
using DeterministicSimulation.Api.Dtos;

namespace DeterministicSimulation.Api.Controllers;

[ApiController]
[Route("simulation")]
public sealed class SimulationController : ControllerBase
{
    private readonly SimulationEngine _engine = new();

    [HttpPost("run")]
    public ActionResult<RunResponse> Run([FromBody] RunRequest request)
    {
        var schedule = new EventSchedule(
            request.Events.Select(e => e.ToDomain())
        );

        var result = _engine.Run(
            request.InitialState.ToDomain(),
            schedule,
            new Tick(request.TargetTick)
        );

        return Ok(new RunResponse(result));
    }

    [HttpPost("replay-from-snapshot")]
    public ActionResult<RunResponse> ReplayFromSnapshot(
        [FromBody] ReplayFromSnapshotRequest request)
    {
        var store = new SnapshotStore(request.Snapshot);

        var schedule = new EventSchedule(
            request.Run.Events.Select(e => e.ToDomain())
        );

        var result = _engine.RunFromSnapshot(
            store,
            request.Run.InitialState.ToDomain(),
            schedule,
            new Tick(request.Run.TargetTick)
        );

        return Ok(new RunResponse(result));
    }
}
