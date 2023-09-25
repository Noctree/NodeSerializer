namespace NodeSerializer.Nodes;

public class BooleanDataNode : TypedValueDataNode<bool>
{
    public BooleanDataNode(bool value, string? name, DataNode? parent) : base(value, name, parent)
    {
    }
}