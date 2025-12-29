using System;
using System.Collections.Generic;
using DeterministicSimulation.Core.Events;

namespace DeterministicSimulation.Core.State;

public static class StateUpdater
{
    public static SimulationState Apply(
        SimulationState previous,
        SimEvent ev)
    {
        return ev switch
        {
            MoveEntity move => ApplyMove(previous, move),
            _ => throw new InvalidOperationException(
                $"Unhandled event type: {ev.GetType().Name}")
        };
    }

    private static SimulationState ApplyMove(
        SimulationState prev,
        MoveEntity move)
    {
        if (!prev.Entities.TryGetValue(move.EntityId, out var entity))
            throw new InvalidOperationException(
                $"Entity '{move.EntityId}' does not exist.");

        var updatedEntity = new EntityState(
            entity.X + move.Dx,
            entity.Y + move.Dy
        );

        var newEntities = new Dictionary<string, EntityState>(prev.Entities)
        {
            [move.EntityId] = updatedEntity
        };

        return new SimulationState(
            move.Tick,
            newEntities
        );
    }
}
