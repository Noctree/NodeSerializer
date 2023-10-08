namespace NodeSerializer.Nodes;

public partial class StringDataNode
{
    public static StringDataNode Create(string value) => new(value, null, null);
    public static StringDataNode Create(string value, string name) => new(value, name, null);
}