namespace NodeSerializer.Nodes;

public partial class NullDataNode
{
    public static NullDataNode Create() => new NullDataNode(null, null);
}