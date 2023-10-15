namespace NodeSerializer;

public class UniqueName : IEquatable<UniqueName>, IEquatable<string>
{
    public string Value { get; }
    
    public UniqueName(string value) => Value = value;

    public bool Equals(UniqueName? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public bool Equals(string? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        return StringComparer.OrdinalIgnoreCase.Compare(Value, other) == 0;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj switch
        {
            UniqueName name => Equals(name),
            string name => Equals(name),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }

    public static bool Equals(UniqueName? uniqueName, string? str)
    {
        if (ReferenceEquals(null, uniqueName)) return false;
        if (ReferenceEquals(null, str)) return false;
        return uniqueName.Equals(str);
    }

    public static bool operator ==(UniqueName? uniqueName, UniqueName? right)
    {
        return Equals(uniqueName, right);
    }

    public static bool operator !=(UniqueName? uniqueName, UniqueName? right)
    {
        return !Equals(uniqueName, right);
    }
    
    public static bool operator ==(UniqueName? uniqueName, string? str)
    {
        return Equals(uniqueName, str);
    }

    public static bool operator !=(UniqueName? uniqueName, string? str)
    {
        return !Equals(uniqueName, str);
    }
    
    public static bool operator ==(string? str, UniqueName? uniqueName)
    {
        return Equals(uniqueName, str);
    }

    public static bool operator !=(string? str, UniqueName? uniqueName)
    {
        return !Equals(uniqueName, str);
    }
}