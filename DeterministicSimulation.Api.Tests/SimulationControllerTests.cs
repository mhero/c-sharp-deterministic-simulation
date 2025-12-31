using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DeterministicSimulation.Api.Controllers;
using DeterministicSimulation.Api.Dtos;
using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DeterministicSimulation.Api.Tests;

public sealed class SimulationControllerTests
{
  private static JsonElement Num(int v)
      => JsonDocument.Parse(v.ToString()).RootElement.Clone();

  [Fact]
  public void Run_SameInputs_ProduceSameFinalState()
  {
    var controller = new SimulationController();

    var initialState = new InitialStateDto
    {
      Tick = 0,
      Entities = new Dictionary<string, EntityStateDto>
      {
        ["E1"] = new EntityStateDto
        {
          Fields = new Dictionary<string, JsonElement>
          {
            ["x"] = Num(0),
            ["y"] = Num(0)
          }
        }
      }
    };

    var events = new SimEventDto[]
    {
            new MoveEntityDto { Tick = 1, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"x", Num(1000)}} },
            new MoveEntityDto { Tick = 2, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"y", Num(500)}} },
            new MoveEntityDto { Tick = 3, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"x", Num(750)}, {"y", Num(250)}} }
    };

    var request = new RunRequest
    {
      InitialState = initialState,
      TargetTick = 3,
      Events = events
    };

    // Properly unwrap ActionResult<RunResponse>
    var actionResult1 = controller.Run(request);
    var ok1 = Assert.IsType<OkObjectResult>(actionResult1.Result);
    var result1 = Assert.IsType<RunResponse>(ok1.Value);

    var actionResult2 = controller.Run(request);
    var ok2 = Assert.IsType<OkObjectResult>(actionResult2.Result);
    var result2 = Assert.IsType<RunResponse>(ok2.Value);

    Assert.Equal(Fingerprint(result1.FinalState), Fingerprint(result2.FinalState));
    Assert.Equal(3, result1.FinalState.Tick.Value);
  }

  [Fact]
  public void ReplayFromSnapshot_MatchesFullReplay()
  {
    var controller = new SimulationController();

    var initialState = new InitialStateDto
    {
      Tick = 0,
      Entities = new Dictionary<string, EntityStateDto>
      {
        ["E1"] = new EntityStateDto
        {
          Fields = new Dictionary<string, JsonElement> { { "x", Num(0) }, { "y", Num(0) } }
        }
      }
    };

    var events = new SimEventDto[]
    {
            new MoveEntityDto { Tick = 1, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"x", Num(1000)}} },
            new MoveEntityDto { Tick = 2, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"y", Num(500)}} },
            new MoveEntityDto { Tick = 3, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"x", Num(750)}, {"y", Num(250)}} },
            new MoveEntityDto { Tick = 4, EntityId = "E1", Fields = new Dictionary<string, JsonElement>{{"x", Num(850)}, {"y", Num(350)}} }
    };

    var runRequest = new RunRequest
    {
      InitialState = initialState,
      TargetTick = 4,
      Events = events
    };

    // Full run
    var fullAction = controller.Run(runRequest);
    var fullOk = Assert.IsType<OkObjectResult>(fullAction.Result);
    var fullResult = Assert.IsType<RunResponse>(fullOk.Value);

    // Snapshot at tick 2
    var snapshotRequest = new RunRequest
    {
      InitialState = initialState,
      TargetTick = 2,
      Events = events
    };
    var snapshotAction = controller.Run(snapshotRequest);
    var snapshotOk = Assert.IsType<OkObjectResult>(snapshotAction.Result);
    var snapshotResult = Assert.IsType<RunResponse>(snapshotOk.Value);
    var snapshot = new DeterministicSimulation.Core.Engine.Snapshot.SimulationSnapshot(snapshotResult.FinalState);

    // Replay from snapshot
    var replayRequest = new ReplayFromSnapshotRequest
    {
      Run = runRequest,
      Snapshot = snapshot
    };

    var replayAction = controller.ReplayFromSnapshot(replayRequest);
    var replayOk = Assert.IsType<OkObjectResult>(replayAction.Result);
    var replayResult = Assert.IsType<RunResponse>(replayOk.Value);

    Assert.Equal(Fingerprint(fullResult.FinalState), Fingerprint(replayResult.FinalState));
  }

  [Fact]
  public void RunRequest_SimulatesRealRequest()
  {
    var controller = new SimulationController();

    var request = new RunRequest
    {
      InitialState = new InitialStateDto
      {
        Tick = 0,
        Entities = new Dictionary<string, EntityStateDto>
        {
          ["E1"] = new EntityStateDto
          {
            Fields = new Dictionary<string, JsonElement>
            {
              ["x"] = JsonDocument.Parse("0").RootElement,
              ["y"] = JsonDocument.Parse("0").RootElement
            }
          }
        }
      },
      TargetTick = 1,
      Events = new SimEventDto[]
        {
            new MoveEntityDto { Tick = 1, EntityId = "E1", Fields = new Dictionary<string, JsonElement> { ["x"] = JsonDocument.Parse("100").RootElement } }
        }
    };

    var actionResult = controller.Run(request);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    var response = Assert.IsType<RunResponse>(okResult.Value);

    Assert.Equal(1, response.FinalState.Tick.Value);
    Assert.Equal(100, response.FinalState.Entities["E1"].Fields["x"].GetInt32());
  }

  private static string Fingerprint(SimulationState state)
  {
    return state.Tick.Value + "|" +
           string.Join("|",
               state.Entities
                   .OrderBy(e => e.Key)
                   .Select(e => e.Key + ":" +
                                string.Join(",", e.Value.Fields.Select(f => $"{f.Key}={f.Value}"))
                   ));
  }
}

