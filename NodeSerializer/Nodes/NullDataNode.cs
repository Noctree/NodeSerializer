namespace NodeSerializer.Nodes;

public sealed partial class NullDataNode : DataNode
{
    public override DataNodeType NodeType => DataNodeType.Null;
    
    internal NullDataNode(string? name, DataNode? parent) : base(null, name, parent)
    {
    }

    public override DataNode Clone() => Create();

    protected override string ToString(byte indent)
    {
        return Indent("null", indent);
    }

    public override bool Equals(DataNode? other)
    {
        return other?.NodeType == DataNodeType.Null || base.Equals(other);
    }
}