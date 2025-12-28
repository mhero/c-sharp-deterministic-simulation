namespace DeterministicSimulation.Core.Time;

public readonly struct Tick : IComparable<Tick>, IEquatable<Tick>
{
    public long Value { get; }

    public Tick(long value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Tick cannot be negative.");

        Value = value;
    }

    public static Tick Zero => new Tick(0);

    public Tick Next() => new Tick(Value + 1);

    public static Tick operator +(Tick tick, long delta)
    {
        if (delta < 0)
            throw new ArgumentOutOfRangeException(nameof(delta));

        return new Tick(tick.Value + delta);
    }

    public static bool operator <(Tick a, Tick b) => a.Value < b.Value;
    public static bool operator >(Tick a, Tick b) => a.Value > b.Value;
    public static bool operator <=(Tick a, Tick b) => a.Value <= b.Value;
    public static bool operator >=(Tick a, Tick b) => a.Value >= b.Value;
    public static bool operator ==(Tick a, Tick b) => a.Value == b.Value;
    public static bool operator !=(Tick a, Tick b) => a.Value != b.Value;

    public int CompareTo(Tick other) => Value.CompareTo(other.Value);

    public bool Equals(Tick other) => Value == other.Value;

    public override bool Equals(object? obj)
        => obj is Tick other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
