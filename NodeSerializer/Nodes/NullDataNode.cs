namespace NodeSerializer.Nodes;

public class NullDataNode : DataNode
{
    public override DataNodeType NodeType => DataNodeType.Null;
    
    public NullDataNode(string? name, DataNode? parent) : base(typeof(void), name, parent)
    {
    }

    public override DataNode Clone()
    {
        return new NullDataNode(Name, null);
    }

    public override string ToString(int indent)
    {
        return Indent("null", indent);
    }

    public override bool Equals(object? obj)
    {
        return obj is DataNode { NodeType: DataNodeType.Null } || base.Equals(obj);
    }

    public override bool Equals(DataNode? other)
    {
        return other?.NodeType == DataNodeType.Null || base.Equals(other);
    }

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(NullDataNode? left, NullDataNode? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(NullDataNode? left, NullDataNode? right)
    {
        return !Equals(left, right);
    }
}