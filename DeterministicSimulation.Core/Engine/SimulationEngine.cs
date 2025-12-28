using DeterministicSimulation.Core.Events;
using DeterministicSimulation.Core.State;
using DeterministicSimulation.Core.Time;

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
}
