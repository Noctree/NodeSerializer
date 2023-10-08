using System.Collections;
using System.Text;
using NodeSerializer.Extensions;

namespace NodeSerializer.Nodes;

public partial class ArrayDataNode : DataNode, ICollection<DataNode>
{
    private readonly List<DataNode> _values = new();
    public Type? ElementType { get; private set; }
    public bool AreValuesNullable { get; private set; }
    public override DataNodeType NodeType => DataNodeType.Array;
    public int Count => _values.Count;
    public bool IsReadOnly => false;

    public ArrayDataNode(Type? type, Type? elementType, string? name, DataNode? parent) : base(type, name, parent)
    {
        if (type is not null && !type.IsAssignableTo(typeof(IEnumerable)))
            throw new ArgumentException($"{nameof(type)} must be an enumerable.", nameof(type));
        ElementType = elementType;
        AreValuesNullable = ElementType?.CanBeNull() ?? true;
    }

    public DataNode this[int index]
    {
        get => _values[index];
        set
        {
            CheckNode(value);
            _values[index] = value;
        }
    }

    public void Add(DataNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        CheckNode(node);

        node.Name = null;
        node.Parent = this;
        _values.Add(node);
    }

    private void CheckNode(DataNode node)
    {
        if (ElementType is not null && node.TypeOf?.IsAssignableTo(ElementType) == false)
        {
            throw new ArgumentException($"Value node of type {node.TypeOf} must be assignable to {ElementType}", nameof(node));
        }

        if (node is NullDataNode && !AreValuesNullable)
        {
            throw new ArgumentException($"Null value nodes are not allowed.", nameof(node));
        }
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
        var clone = new ArrayDataNode(TypeOf, ElementType, null, null);
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

    protected override string ToString(byte indent)
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