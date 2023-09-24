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
    
    public DataNode Deserialize(string raw, Type targetType)
    {
        //If the type is primitive, we should be able to just parse the whole string immediately
        if (targetType.IsPrimitive)
        {
            return new ValueDataNode(PrimitiveSerializer.Deserialize(raw, targetType), null, null);
        }
        
        //Else we need to deserialize the entire JSON
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(raw), ReaderOptions);
        DataNode output = targetType.IsArray
            ? new ArrayDataNode(targetType, null, null)
            : new ObjectDataNode(targetType, null, null);
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Comment:
                    continue;
                case JsonTokenType.StartObject:
            }
        }
    }

    public string Serialize(DataNode data)
    {
        throw new NotImplementedException();
    }
}