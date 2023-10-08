using NodeSerializer.Extensions;

namespace NodeSerializer.Nodes;

public class ValueDataNode : DataNode
{
    private object _value;

    public virtual object Value
    {
        get => _value;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.GetType() != TypeOf)
                throw new ArgumentException($"Value must be of type {TypeOf}");
            _value = value;
        }
    }
    
    public override DataNodeType NodeType => DataNodeType.Value;

    public ValueDataNode(object value, Type type, string? name, DataNode? parent) : base(type, name, parent)
    {
        if (!value.GetType().IsPrimitive)
            ArgumentNullException.ThrowIfNull(value);
        if (!type.IsPrimitiveExtended())
            throw new ArgumentException("Value must be a primitive or string or decimal.");
        _value = value;
    }
    
    public override DataNode Clone()
    {
        return new ValueDataNode(_value, TypeOf!, null, null);
    }

    public override bool Equals(DataNode? other)
    {
        if (other is ValueDataNode otherValue)
        {
            return _value.Equals(otherValue._value);
        }
        return base.Equals(other);
    }

    protected override string ToString(byte indent)
    {
        return Indent($"Value({Name}: {Value})", indent);
    }

    public virtual string? SerializeToString()
    {
        return Value.ToString();
    }
}