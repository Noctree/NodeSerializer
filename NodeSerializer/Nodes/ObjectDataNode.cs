using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NodeSerializer.Nodes;

public class ObjectDataNode : DataNode, IDictionary<string, DataNode>
{
    private readonly Dictionary<string, int> _propertyIndices = new();
    private readonly List<DataNode> _properties = new();

    public int Count => _properties.Count;
    public bool IsReadOnly => false;
    public ICollection<string> Keys => _propertyIndices.Keys;
    public ICollection<DataNode> Values => _properties;
    public override DataNodeType NodeType => DataNodeType.Object;
    public ObjectDataNode(Type? type, string? name, DataNode? parent) : base(type, name, parent)
    {
    }

    public override void ChangeType(Type newType)
    {
        if (newType.IsPrimitive)
            throw new NotSupportedException("Object cannot be a primitive type");
        base.ChangeType(newType);
    }

    public void Add(DataNode property)
    {
        ArgumentNullException.ThrowIfNull(property);
        if (property.Name is null)
            throw new ArgumentNullException( nameof(property), "Property of an object must have a name.");
        CheckNameValid(property.Name);
        AddPropertyInternal(property);
    }

    public void Add(string name, DataNode property)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(name);
        CheckNameValid(name);
        property.Name = name;
        AddPropertyInternal(property);
    }
    
    public void Add(KeyValuePair<string, DataNode> item)
    {
        Add(item.Key, item.Value);
    }
    
    internal void AddPropertyInternal(DataNode property)
    {
        _propertyIndices.Add(property.Name!, _properties.Count);
        _properties.Add(property);
    }

    public bool ContainsKey(string key) => HasProperty(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out DataNode value)
    {
        value = null;
        if (_propertyIndices.TryGetValue(key, out var index))
        {
            value = _properties[index];
            return true;
        }
        return false;
    }
    
    public bool Remove(string name)
    {
        if (!_propertyIndices.TryGetValue(name, out var index))
            return false;
        var old = _properties[index];
        old.Parent = null;
        _propertyIndices.Remove(name);
        _properties.RemoveAt(index);
        UpdateIndexLookup(index);
        return true;
    }
    
    public bool Remove(DataNode item)
    {
        var index = _properties.IndexOf(item);
        if (index < 0)
            return false;
        var old = _properties[index];
        old.Parent = null;
        _properties.RemoveAt(index);
        return true;
    }

    public bool Remove(KeyValuePair<string, DataNode> item)
    {
        var old = FindByKeyValuePair(item.Key, item.Value);
        if (old is null)
            return false;

        var index = _propertyIndices[old.Name!];
        old.Parent = null;
        _propertyIndices.Remove(item.Key);
        _properties.RemoveAt(index);
        return true;
    }

    public bool HasProperty(string name) => _propertyIndices.ContainsKey(name);

    public void Clear()
    {
        foreach (var property in _properties)
        {
            property.Parent = null;
        }
        _properties.Clear();
        _propertyIndices.Clear();
    }

    private DataNode? FindByKeyValuePair(string name, DataNode item)
    {
        if (!_propertyIndices.TryGetValue(name, out var index))
            return null;
        var old = _properties[index];
        return old == item ? old : null;
    }
    
    public bool Contains(DataNode value) => _properties.Contains(value);
    public bool Contains(KeyValuePair<string, DataNode> item) => FindByKeyValuePair(item.Key, item.Value) is not null;
    
    public void CopyTo(KeyValuePair<string, DataNode>[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("Not enough space in the array.");
        for (var i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = new KeyValuePair<string, DataNode>(_properties[i].Name!, _properties[i]);
        }
    }

    public void CopyTo(DataNode[] array, int arrayIndex)
    {
        _properties.CopyTo(array, arrayIndex);
    }

    public DataNode this[int index]
    {
        get => _properties[index];
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (index < 0 || index >= _properties.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            IndexerSetInternal(value, index, _properties[index].Name!);
        }
    }
    
    public DataNode this[string name]
    {
        get => _propertyIndices.TryGetValue(name, out var index)
            ? _properties[index]
            : throw new KeyNotFoundException($"No property with the name {name} exists.");
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (!_propertyIndices.TryGetValue(name, out var index))
                throw new KeyNotFoundException($"No property with the name {name} exists.");
            
            IndexerSetInternal(value, index, name);
        }
    }

    private void IndexerSetInternal(DataNode value, int index, string name)
    {
        var expectedName = _properties[index].Name;

        if (value.Name is not null && expectedName != value.Name)
            throw new ArgumentException($"Property must have the name {name}, or the name must be null.");
        if (value.Parent is not null && value.Parent != this)
            throw new ArgumentException("New value cannot have a parent.");
        value.Name ??= name;
        var old = _properties[index];
        _properties[index] = value;
        value.Parent = this;
        old.Parent = null;
    }
    
    public override DataNode Clone()
    {
        var clone = new ObjectDataNode(TypeOf, Name, null);
        foreach (var property in _properties)
        {
            clone.AddPropertyInternal(property.Clone());
        }
        return clone;
    }

    public override bool Equals(DataNode? other)
    {
        if (other is ObjectDataNode otherNode)
        {
            return PropertiesEqual(this, otherNode);
        }
        return base.Equals(other);
    }

    private static bool PropertiesEqual(ObjectDataNode a, ObjectDataNode b)
    {
        foreach (var propertyName in a._propertyIndices.Keys)
        {
            if (!b._propertyIndices.ContainsKey(propertyName))
                return false;
            var objA = a[propertyName];
            var objB = b[propertyName];
            if (objA != objB)
                return false;
        }

        return true;
    }

    private void UpdateIndexLookup(int startIndex)
    {
        for (var i = startIndex; i < _properties.Count; i++)
        {
            _propertyIndices[_properties[i].Name!] = i;
        }
    }

    private void CheckNameValid(string name)
    {
        if (_propertyIndices.ContainsKey(name))
            throw new InvalidOperationException($"A property with the name {name} already exists.");
    }

    public override string ToString(int indent)
    {
        var sb = new StringBuilder();
        var tab = GetIndent(indent);
        sb.Append(tab + "Object(");
        sb.Append(Name);
        sb.Append(", {\n");
        foreach (var property in _properties)
        {
            sb.Append(tab);
            sb.Append(property.Name);
            sb.Append(":\n");
            sb.Append(property.ToString(indent + 1));
            sb.Append(",\n");
        }
        sb.Append(tab + "})");
        return sb.ToString();
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<KeyValuePair<string, DataNode>> IEnumerable<KeyValuePair<string, DataNode>>.GetEnumerator() =>
        GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<KeyValuePair<string, DataNode>>
    {
        private Dictionary<string, int>.Enumerator _indexEnumerator;
        private List<DataNode>.Enumerator _propertyEnumerator;
        public KeyValuePair<string, DataNode> Current => new(_indexEnumerator.Current.Key, _propertyEnumerator.Current);
        object IEnumerator.Current => Current;
        
        internal Enumerator(ObjectDataNode node)
        {
            _indexEnumerator = node._propertyIndices.GetEnumerator();
            _propertyEnumerator = node._properties.GetEnumerator();
        }
        
        public void Dispose()
        {
            _indexEnumerator.Dispose();
            _propertyEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return _indexEnumerator.MoveNext() && _propertyEnumerator.MoveNext();
        }

        public void Reset()
        {
            ((IEnumerator)_indexEnumerator).Reset();
            ((IEnumerator)_propertyEnumerator).Reset();
        }
    }
}