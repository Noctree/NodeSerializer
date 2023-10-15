using NodeSerializer.Nodes;
using NodeSerializer.Serialization.Internal;

namespace NodeSerializer.Serialization;

public static class ObjectDeserializer
{
    public static object? Deserialize(DataNode node, Type type) => node switch
    {
        NullDataNode _ => default,
        BooleanDataNode booleanNode => booleanNode.Value,
        NumberDataNode numberNode => numberNode.TypedValue.ToObject(),
        StringDataNode stringNode => stringNode.Value,
        ValueDataNode valueDataNode => valueDataNode.Value,
        ObjectDataNode objectNode => DeserializeObject(objectNode, type),
        ArrayDataNode arrayNode => DeserializeArray(arrayNode, type),
        _ => throw new NotSupportedException($"Cannot deserialize {node} node.")
    };

    private static object DeserializeArray(ArrayDataNode arrayNode, Type type)
    {
        throw new NotImplementedException();
    }

    private static object DeserializeObject(ObjectDataNode objectNode, Type type)
    {
        var meta = SerializationMetadataCache.Get(type);
        
        //Create property map
        var props = 
    }
}