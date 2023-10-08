namespace NodeSerializer.Nodes;

public sealed partial class BooleanDataNode : TypedValueDataNode<bool>, IEquatable<BooleanDataNode>
{
    private const string TRUE_STRING = "true";
    private const string FALSE_STRING = "false";
    
    public string ValueAsString => TypedValue ? TRUE_STRING : FALSE_STRING;
    
    public BooleanDataNode(bool value, string? name, DataNode? parent) : base(value, typeof(bool), name, parent)
    {
    }

    public override DataNode Clone()
    {
        return new BooleanDataNode(TypedValue, null, null);
    }

    public bool Equals(BooleanDataNode? other)
    {
        return Value == other?.Value;
    }

    public override bool Equals(DataNode? other)
    {
        if (other is BooleanDataNode booleanDataNode)
        {
            return Value == booleanDataNode.Value;
        }
        return base.Equals(other);
    }
}