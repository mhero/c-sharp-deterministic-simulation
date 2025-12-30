using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using DeterministicSimulation.Core.State;

namespace DeterministicSimulation.Api.Dtos;

public sealed class EntityStateDto
{
    [Required]
    public Dictionary<string, JsonElement> Fields { get; init; } = [];

    public EntityState ToDomain() => new(Fields);
}
