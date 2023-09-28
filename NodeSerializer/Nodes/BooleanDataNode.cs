namespace NodeSerializer.Nodes;

public class BooleanDataNode : TypedValueDataNode<bool>
{
    private const string TRUE_STRING = "true";
    private const string FALSE_STRING = "false";
    
    public string ValueAsString => TypedValue ? TRUE_STRING : FALSE_STRING;
    
    public BooleanDataNode(bool value, string? name, DataNode? parent) : base(value, typeof(bool), name, parent)
    {
    }

    protected override string ToString(byte indent)
    {
        return Indent($"Boolean({Name}: {ValueAsString})", indent);
    }
}