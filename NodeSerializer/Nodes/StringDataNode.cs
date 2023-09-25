﻿namespace NodeSerializer.Nodes;

public class StringDataNode : TypedValueDataNode<StringStruct>
{
    public StringDataNode(string value, string? name, DataNode? parent) : base(value, name, parent)
    {
    }
}

public readonly struct StringStruct
{
    public readonly string Value;

    public StringStruct(string value)
    {
        Value = value;
    }
    
    public static implicit operator string(StringStruct value) => value.Value;
    public static implicit operator StringStruct(string value) => new(value);
}