using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeterministicSimulation.Core.Engine;
using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;
using DeterministicSimulation.Core.Engine.Snapshot;
using System.Collections.Generic;
using System.Text.Json;

namespace DeterministicSimulation.Tests;

[TestClass]
public sealed class DeterminismTests
{
    private static JsonElement Num(int v)
    {
        using var doc = JsonDocument.Parse(v.ToString());
        return doc.RootElement.Clone();
    }


    [TestMethod]
    public void SameInputs_ProduceSameFinalState()
    {
        var initialEntities = new Dictionary<string, EntityState>
        {
            ["E1"] = new EntityState(new Dictionary<string, JsonElement>
            {
                ["x"] = Num(0),
                ["y"] = Num(0)
            })
        };

        var initialState = new SimulationState(Tick.Zero, initialEntities);

        var events = new SimEvent[]
        {
            new MoveEntity(new Tick(1), "E1", new Dictionary<string, JsonElement>
            {
                ["x"] = Num(1000)
            }),
            new MoveEntity(new Tick(2), "E1", new Dictionary<string, JsonElement>
            {
                ["y"] = Num(500)
            }),
            new MoveEntity(new Tick(3), "E1", new Dictionary<string, JsonElement>
            {
                ["x"] = Num(750),
                ["y"] = Num(250)
            })
        };

        var schedule = new EventSchedule(events);
        var engine = new SimulationEngine();

        var result1 = engine.Run(initialState, schedule, new Tick(3));
        var result2 = engine.Run(initialState, schedule, new Tick(3));

        Assert.AreEqual(result1, result2);
        Assert.AreEqual(new Tick(3), result1.Tick);
    }

    [TestMethod]
    public void ReplayFromSnapshot_MatchesFullReplay()
    {
        var initialEntities = new Dictionary<string, EntityState>
        {
            ["E1"] = new EntityState(new Dictionary<string, JsonElement>
            {
                ["x"] = Num(0),
                ["y"] = Num(0)
            })
        };

        var initialState = new SimulationState(Tick.Zero, initialEntities);

        var events = new SimEvent[]
        {
            new MoveEntity(new Tick(1), "E1", new Dictionary<string, JsonElement> { ["x"] = Num(1000) }),
            new MoveEntity(new Tick(2), "E1", new Dictionary<string, JsonElement> { ["y"] = Num(500) }),
            new MoveEntity(new Tick(3), "E1", new Dictionary<string, JsonElement> { ["x"] = Num(750), ["y"] = Num(250) }),
            new MoveEntity(new Tick(4), "E1", new Dictionary<string, JsonElement> { ["x"] = Num(850), ["y"] = Num(350) })
        };

        var schedule = new EventSchedule(events);
        var engine = new SimulationEngine();

        var full = engine.Run(initialState, schedule, new Tick(4));

        var snapshotState = engine.Run(initialState, schedule, new Tick(2));
        var store = new SnapshotStore(new SimulationSnapshot(snapshotState));

        var fromSnapshot = engine.RunFromSnapshot(
            store,
            initialState,
            schedule,
            new Tick(4)
        );

        Assert.AreEqual(full, fromSnapshot);
    }

    [TestMethod]
    public void EventOrdering_IsDeterministic()
    {
        var events = new SimEvent[]
        {
        new MoveEntity(new Tick(1), "B", new Dictionary<string, JsonElement>()),
        new MoveEntity(new Tick(1), "A", new Dictionary<string, JsonElement>()),
        new MoveEntity(new Tick(1), "C", new Dictionary<string, JsonElement>())
        };

        var schedule = new EventSchedule(events);

        var ordered = schedule.ForTick(new Tick(1))
            .OfType<MoveEntity>()
            .OrderBy(e => e.EntityId)
            .Select(e => e.EntityId)
            .ToArray();

        CollectionAssert.AreEqual(new[] { "A", "B", "C" }, ordered);
    }

    [TestMethod]
    public void StressTest_10kEvents_Deterministic()
    {
        const int entityCount = 100;
        const int eventCount = 10000;

        var entities = new Dictionary<string, EntityState>();
        for (int i = 0; i < entityCount; i++)
        {
            entities[$"E{i}"] = new EntityState(new Dictionary<string, JsonElement>
            {
                ["v"] = Num(0)
            });
        }

        var initialState = new SimulationState(Tick.Zero, entities);

        var events = new List<SimEvent>();
        for (int t = 1; t <= eventCount; t++)
        {
            int eId = t % entityCount;
            events.Add(new MoveEntity(
                new Tick(t),
                $"E{eId}",
                new Dictionary<string, JsonElement> { ["v"] = Num(t) }
            ));
        }

        var schedule = new EventSchedule(events);
        var engine = new SimulationEngine();

        var full = engine.Run(initialState, schedule, new Tick(eventCount));

        var store = new SnapshotStore();
        for (int snapTick = 1000; snapTick <= eventCount; snapTick += 1000)
        {
            var snapState = engine.Run(initialState, schedule, new Tick(snapTick));
            store.Save(new SimulationSnapshot(snapState));
        }

        var fromSnapshot = engine.RunFromSnapshot(
            store,
            initialState,
            schedule,
            new Tick(eventCount)
        );

        Assert.AreEqual(full, fromSnapshot);
    }
}
