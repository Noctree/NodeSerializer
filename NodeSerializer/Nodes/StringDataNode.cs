namespace NodeSerializer.Nodes;

public partial class StringDataNode : TypedValueDataNode<StringStruct>
{
    internal StringDataNode(string value, string? name, DataNode? parent) : base(value, typeof(string), name, parent)
    {
    }

    public override DataNode Clone()
    {
        return new StringDataNode(TypedValue, null, null);
    }
}

public readonly struct StringStruct : IFormattable, IComparable<string>, IComparable<StringStruct>
{
    public readonly string Value;

    public StringStruct(string value)
    {
        Value = value;
    }
    
    public static implicit operator string(StringStruct value) => value.Value;
    public static implicit operator StringStruct(string value) => new(value);
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value;
    }
    
    public string AsString() => Value;
    public int CompareTo(string? other)
    {
        return string.Compare(Value, other, StringComparison.Ordinal);
    }

    public int CompareTo(StringStruct other)
    {
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }
}