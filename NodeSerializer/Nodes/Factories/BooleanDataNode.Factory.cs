namespace NodeSerializer.Nodes;

public partial class BooleanDataNode
{
    public static BooleanDataNode Create(bool value) => new(value, null, null);
    public static BooleanDataNode Create(bool value, string name) => new(value, name, null);
}