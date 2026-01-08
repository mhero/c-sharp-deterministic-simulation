using System.Collections.Immutable;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace DeterministicSimulation.Core.State;

public sealed class EntityState(IDictionary<string, JsonElement> fields)
{
    public ImmutableSortedDictionary<string, JsonElement> Fields { get; } = fields.ToImmutableSortedDictionary(StringComparer.Ordinal);

    public JsonElement this[string key] => Fields[key];

    public EntityState With(string key, JsonElement value)
        => new(Fields.SetItem(key, value.Clone()));

    public EntityState Clone() => this;

    public override bool Equals(object? obj)
    {
        if (obj is not EntityState other) return false;
        if (Fields.Count != other.Fields.Count) return false;

        foreach (var kv in Fields)
        {
            if (!other.Fields.TryGetValue(kv.Key, out var oVal)) return false;
            if (kv.Value.GetRawText() != oVal.GetRawText()) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var kv in Fields)
        {
            hash = hash * 31 + kv.Key.GetHashCode();
            hash = hash * 31 + kv.Value.GetRawText().GetHashCode();
        }
        return hash;
    }
}
