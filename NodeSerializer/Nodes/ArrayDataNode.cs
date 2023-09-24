using System.Collections;
using System.Text;
using NodeSerializer.Extensions;

namespace NodeSerializer.Nodes;

public class ArrayDataNode : DataNode, ICollection<DataNode>
{
    private readonly List<DataNode> _values = new();
    public Type ValueType { get; private set; }
    public bool AreValuesNullable { get; private set; }
    public override DataNodeType NodeType => DataNodeType.Array;
    public int Count => _values.Count;
    public bool IsReadOnly => false;

    public ArrayDataNode(Type type, string? name, DataNode? parent) : base(type, name, parent)
    {
        if (!type.IsArray)
            throw new ArgumentException($"{nameof(type)} must be a type of an array.", nameof(type));
        ValueType = type.GetElementType()!;
        AreValuesNullable = ValueType.CanBeNull();
    }

    public void Add(DataNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!node.TypeOf.IsAssignableTo(ValueType))
        {
            throw new ArgumentException($"Value must be assignable to {ValueType}", nameof(node));
        }

        if (node is NullDataNode && !AreValuesNullable)
        {
            throw new ArgumentException($"Null values are not allowed.", nameof(node));
        }

        node.Name = null;
        node.Parent = this;
        _values.Add(node);
    }

    public void Clear()
    {
        foreach (var value in _values)
        {
            value.Parent = null;
        }
        _values.Clear();
    }
    
    public bool Remove(DataNode item)
    {
        var index = _values.IndexOf(item);
        if (index < 0)
            return false;
        
        var old = _values[index];
        old.Parent = null;
        _values.RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        var node = _values[index];
        node.Parent = null;
        _values.RemoveAt(index);
    }
    
    public bool Contains(DataNode node)
    {
        return _values.Contains(node);
    }

    public void CopyTo(DataNode[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    public override DataNode Clone()
    {
        var clone = new ArrayDataNode(TypeOf, Name, null);
        foreach (var node in _values)
        {
            clone.Add(node.Clone());
        }
        return clone;
    }

    public override bool Equals(DataNode? other)
    {
        if (other is ArrayDataNode otherNode)
        {
            return ElementsEqual(this, otherNode);
        }
        return base.Equals(other);
    }

    private static bool ElementsEqual(ArrayDataNode a, ArrayDataNode b)
    {
        if (a._values.Count != b._values.Count)
            return false;
        
        for (var i = 0; i < a._values.Count; i++)
        {
            if (!a._values[i].Equals(b._values[i]))
                return false;
        }

        return true;
    }

    public override string ToString(int indent)
    {
        var tab = GetIndent(indent);
        var sb = new StringBuilder();
        sb.Append(tab + '[');
        foreach (var value in _values)
        {
            sb.Append(value.ToString(indent + 1));
            sb.Append(",\n");
        }
        sb.Append(tab + ']');
        return sb.ToString();
    }

    public List<DataNode>.Enumerator GetEnumerator() => _values.GetEnumerator();
    
    IEnumerator<DataNode> IEnumerable<DataNode>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}