using System.ComponentModel.DataAnnotations;
using DeterministicSimulation.Core.State;

namespace DeterministicSimulation.Api.Dtos;

public sealed class EntityStateDto
{
  [Required]
  public int X { get; init; }

  [Required]
  public int Y { get; init; }

  public EntityState ToDomain() => new(X, Y);
}
