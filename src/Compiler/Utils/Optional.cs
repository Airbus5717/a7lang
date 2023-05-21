namespace A7.Utils;

public struct Optional<T>
{
    public bool HasValue { get; private set; }
    private T value;
    public T Value
    {
        get
        {
            return value;
        }
    }

    public Optional(T value)
    {
        this.value = value;
        HasValue = true;
    }

    public static explicit operator T(Optional<T> optional)
    {
        return optional.Value;
    }

    public static implicit operator Optional<T>(T value)
    {
        return new Optional<T>(value);
    }

    public override bool Equals(object obj)
    {
        if (obj is Optional<T>) return this.Equals((Optional<T>)obj);

        return false;
    }

    public bool Equals(Optional<T> other)
    {
        if (HasValue && other.HasValue)
            return object.Equals(value, other.value);

        return HasValue == other.HasValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HasValue, value, Value);
    }
}
