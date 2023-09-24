namespace NodeSerializer.Nodes;

public class ValueDataNode : DataNode
{
    private object _value;

    public object Value
    {
        get => _value;
        set
        {
            if (value.GetType() != TypeOf)
                throw new ArgumentException($"Value must be of type {TypeOf}");
            _value = value;
        }
    }
    public override DataNodeType NodeType => DataNodeType.Value;

    public ValueDataNode(object value, string? name, DataNode? parent) : base(value.GetType(), name, parent)
    {
        ArgumentNullException.ThrowIfNull(value);
        var type = _value!.GetType();
        if (!type.IsPrimitive)
            throw new ArgumentException("Value must be a primitive.");
        _value = value;
    }

    public T GetAs<T>() => (T)_value;
    
    public override DataNode Clone()
    {
        return new ValueDataNode(_value, Name, null);
    }

    public override bool Equals(DataNode? other)
    {
        if (other is ValueDataNode otherValue)
        {
            return _value.Equals(otherValue._value);
        }
        return base.Equals(other);
    }

    public override string ToString(int indent)
    {
        return new string(IndentChar, indent) + $"Value({Name}: {_value})";
    }
}