using System;
using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;
using DeterministicSimulation.Core.Engine.Snapshot;

namespace DeterministicSimulation.Core.Engine;

public sealed class SimulationEngine
{
    public SimulationState Run(
        SimulationState initialState,
        EventSchedule schedule,
        Tick targetTick)
    {
        if (targetTick < initialState.Tick)
            throw new InvalidOperationException(
                "Cannot run simulation backwards.");

        var state = initialState;

        while (state.Tick < targetTick)
        {
            var nextTick = state.Tick.Next();

            var events = schedule.ForTick(nextTick);

            var nextState = state;

            foreach (var ev in events)
            {
                nextState = StateUpdater.Apply(nextState, ev);
            }

            // Advance tick even if no events occurred
            if (nextState.Tick < nextTick)
            {
                nextState = new SimulationState(
                    nextTick,
                    nextState.Entities
                );
            }

            state = nextState;
        }

        return state;
    }

    public SimulationState RunFromSnapshot(
    SnapshotStore snapshots,
    SimulationState initialState,
    EventSchedule schedule,
    Tick targetTick)
    {
        var snapshot = snapshots.GetLatestAtOrBefore(targetTick);

        if (snapshot != null && snapshot.Tick < initialState.Tick)
            throw new InvalidOperationException(
                "Snapshot predates initial state.");

        var startState = snapshot is null
            ? initialState
            : snapshot.State;

        return Run(startState, schedule, targetTick);
    }

}
