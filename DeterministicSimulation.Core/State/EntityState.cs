using System.Collections.Immutable;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace DeterministicSimulation.Core.State;

public sealed class EntityState(IDictionary<string, JsonElement> fields)
{
  public ImmutableSortedDictionary<string, JsonElement> Fields { get; } = fields.ToImmutableSortedDictionary(
          StringComparer.Ordinal
      );

  public JsonElement this[string key] => Fields[key];

    public EntityState With(string key, JsonElement value)
        => new(Fields.SetItem(key, value.Clone()));

    public EntityState Clone() => this;
}
