using System.Text;
using System.Text.Json;
using NodeSerializer.Nodes;

namespace NodeSerializer.Serialization;

public class JsonLoader : ISerializedLoader
{
    private static readonly JsonReaderOptions ReaderOptions = new()
    {
        AllowTrailingCommas = true
    };
    
    public DataNode Deserialize(string raw, IFormatProvider? formatProvider = null)
    {
        //If the type is primitive, we should be able to just parse the whole string immediately
        if (targetType.IsPrimitive)
        {
            return new ValueDataNode(PrimitiveSerializer.Deserialize(raw, targetType), null, null);
        }
        
        //Else we need to deserialize the entire JSON
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(raw), ReaderOptions);
        DataNode output = null;
        while (reader.Read() && reader.TokenType == JsonTokenType.Comment) {}

        switch (reader.TokenType)
        {
            case JsonTokenType.False:
                return new BooleanDataNode(false, null, null);
            case JsonTokenType.True:
                return new BooleanDataNode(true, null, null);
            case JsonTokenType.Number:
        }
    }
    
    private DataNode ParseNumber(Utf8JsonReader reader, string? name, IFormatProvider? formatProvider)
    {
        var rawValue = reader.ValueSpan.ToString();
        if (rawValue.Length == 0)
            return new TypedValueDataNode<int>(0, null, null);
    }

    private DataNode ParseObjectRecursively(Utf8JsonReader reader, string name)
    {
        var instance = new ObjectDataNode(typeof(object), name, null);
        var newName = (string)null;
        while (reader.Read())
        {
            DataNode result;
            switch (reader.TokenType)
            {
                case JsonTokenType.Comment:
                    continue;
                case JsonTokenType.PropertyName:
                    newName = reader.ValueSpan.ToString();
                    break;
                case JsonTokenType.StartObject:
                    CheckPropertyNamePresent();
                    result = ParseObjectRecursively(reader, newName!);
                    instance.Add(result);
                    break;
                case JsonTokenType.StartArray:
                    CheckPropertyNamePresent();
                    result = ParseArrayRecursively(reader, newName!, instance);
                    instance.Add(result);
                    break;
                case JsonTokenType.EndObject:
                    return instance;
                default:
                    throw new JsonException($"Unexpected token at {reader.Position}");
            }
        }
        throw new JsonException("Unexpected end of object");

        void CheckPropertyNamePresent()
        {
            if (newName is null)
                throw new JsonException("New object declared inside another object without a property name");
        }
    }

    private DataNode ParseArrayRecursively(Utf8JsonReader reader, string name, DataNode parent)
    {
        throw new NotImplementedException();
    }

    public string Serialize(DataNode data)
    {
        throw new NotImplementedException();
    }
}