using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeterministicSimulation.Core.Engine;
using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;
using DeterministicSimulation.Core.Engine.Snapshot;
using System.Collections.Generic;
using System.Linq;

namespace DeterministicSimulation.Tests;

[TestClass]
public sealed class DeterminismTests
{
    [TestMethod]
    public void SameInputs_ProduceSameFinalState()
    {
        // Arrange
        var initialEntities = new Dictionary<string, EntityState>
        {
            ["E1"] = new EntityState(0, 0)
        };

        var initialState = new SimulationState(
            Tick.Zero,
            initialEntities
        );

        var events = new SimEvent[]
        {
            new MoveEntity(new Tick(1), "E1", 1000, 0),
            new MoveEntity(new Tick(2), "E1", 0, 500),
            new MoveEntity(new Tick(3), "E1", -250, -250)
        };

        var schedule = new EventSchedule(events);
        var engine = new SimulationEngine();

        // Act
        var result1 = engine.Run(initialState, schedule, new Tick(3));
        var result2 = engine.Run(initialState, schedule, new Tick(3));

        // Assert
        Assert.AreEqual(Fingerprint(result1), Fingerprint(result2));
        Assert.AreEqual(new Tick(3), result1.Tick);

        var entity = result1.Entities["E1"];
        Assert.AreEqual(750, entity.X);
        Assert.AreEqual(250, entity.Y);

    }

    [TestMethod]
    public void ReplayFromSnapshot_MatchesFullReplay()
    {
        var initialEntities = new Dictionary<string, EntityState>
        {
            ["E1"] = new EntityState(0, 0)
        };

        var initialState = new SimulationState(Tick.Zero, initialEntities);

        var events = new SimEvent[]
        {
        new MoveEntity(new Tick(1), "E1", 1000, 0),
        new MoveEntity(new Tick(2), "E1", 0, 500),
        new MoveEntity(new Tick(3), "E1", -250, -250),
        new MoveEntity(new Tick(4), "E1", 100, 100)
        };

        var schedule = new EventSchedule(events);
        var engine = new SimulationEngine();

        // Full replay
        var full = engine.Run(initialState, schedule, new Tick(4));

        // Snapshot at tick 2
        var snapshotState = engine.Run(initialState, schedule, new Tick(2));
        var store = new SnapshotStore();
        store.Save(new SimulationSnapshot(snapshotState));

        // Replay from snapshot
        var fromSnapshot = engine.RunFromSnapshot(
            store,
            initialState,
            schedule,
            new Tick(4)
        );

        Assert.AreEqual(Fingerprint(full), Fingerprint(fromSnapshot));
    }


    private static string Fingerprint(SimulationState state)
    {
        return state.Tick.Value + "|" +
            string.Join(
                "|",
                state.Entities
                    .OrderBy(e => e.Key)
                    .Select(e => $"{e.Key}:{e.Value.X},{e.Value.Y}")
            );
    }
}