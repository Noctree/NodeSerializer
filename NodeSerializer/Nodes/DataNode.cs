﻿namespace NodeSerializer.Nodes;

public enum DataNodeType
{
    Null,
    Value,
    Array,
    Object,
}

public abstract class DataNode : IEquatable<DataNode>
{
    private static readonly Dictionary<byte, string> IndentCache = new();
    protected const char INDENT_CHAR = ' ';
    public abstract DataNodeType NodeType { get; }
    public DataNode? Parent { get; internal set; }
    public Type? TypeOf { get; private set; }
    public string? Name { get; internal set; }

    protected DataNode(Type? type, string? name, DataNode? parent)
    {
        IsValidParent(parent);
        if (name?.Length == 0)
            throw new ArgumentException("Name cannot be empty, use null instead.", nameof(name));
        TypeOf = type;
        Parent = parent;
        Name = name;
    }

    private static void IsValidParent(DataNode? parent)
    {
        if (parent is null)
            return;
        if (parent.NodeType == DataNodeType.Value)
        {
            throw new NotSupportedException($"Cannot have a value as a {nameof(parent)}.");
        }

        if (parent.NodeType == DataNodeType.Null)
        {
            throw new NotSupportedException($"Cannot have a null as a {nameof(parent)}.");
        }
    }

    public abstract DataNode Clone();

    public virtual void ChangeType(Type newType)
    {
        TypeOf = newType;
    }

    public bool IsNull() => NodeType == DataNodeType.Null;
    
    public ValueDataNode AsValue() => this as ValueDataNode ?? throw new InvalidOperationException("This node is not a value.");
    public NumberDataNode AsNumber() => this as NumberDataNode ?? throw new InvalidOperationException("This node is not a number.");
    public StringDataNode AsString() => this as StringDataNode ?? throw new InvalidOperationException("This node is not a string.");
    public BooleanDataNode AsBoolean() => this as BooleanDataNode ?? throw new InvalidOperationException("This node is not a boolean.");
    public ArrayDataNode AsArray() => this as ArrayDataNode ?? throw new InvalidOperationException("This node is not an array.");
    public ObjectDataNode AsObject() => this as ObjectDataNode ?? throw new InvalidOperationException("This node is not an object.");

    public virtual bool Equals(DataNode? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return NodeType == other.NodeType
               && Equals(Parent, other.Parent)
               && TypeOf == other.TypeOf
               && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is DataNode other)
            return Equals(other);
        return false;
    }

    public string ToString(int indent)
    {
        if (indent is < 0 or > byte.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(indent));
        return ToString((byte)indent);
    }

    protected abstract string ToString(byte indent);

    public override string ToString()
    {
        return ToString(0);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)NodeType, Parent, TypeOf, Name);
    }

    public static bool operator ==(DataNode? left, DataNode? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        return left?.Equals(right) ?? false;
    }

    public static bool operator !=(DataNode? left, DataNode? right)
    {
        return !(left == right);
    }

    protected static string Indent(string str, byte amount)
    {
        var indentStr = GetIndent(amount);
        return indentStr + str;
    }

    protected static string GetIndent(byte indent)
    {
        if (indent == 0)
            return string.Empty;
        if (IndentCache.TryGetValue(indent, out var indentStr))
            return indentStr;
        indentStr = new string(INDENT_CHAR, indent);
        IndentCache.Add(indent, indentStr);
        return indentStr;
    }
}