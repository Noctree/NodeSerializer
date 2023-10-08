namespace NodeSerializer.Nodes;

public partial class ObjectDataNode
{
    public static ObjectDataNode CreateUntypedEmpty() => new(null, null, null);
    public static ObjectDataNode CreateEmpty(Type type) => new(type, null, null);
    public static ObjectDataNode CreateEmpty(Type type, string name) => new(type, name, null);
}