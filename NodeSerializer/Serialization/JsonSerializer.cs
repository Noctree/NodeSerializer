using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using NodeSerializer.Extensions;
using NodeSerializer.Nodes;

namespace NodeSerializer.Serialization;

public class JsonLoader : IJsonSerializer
{
    private static readonly JsonReaderOptions ReaderOptions = new()
    {
        AllowTrailingCommas = true
    };
    
    public DataNode DeserializeFromBytes(byte[] raw)
    {
        var reader = new Utf8JsonReader(raw, ReaderOptions);
        return DeserializeInternal(reader);
    }

    public DataNode Deserialize(string raw)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(raw), ReaderOptions);
        return DeserializeInternal(reader);
    }
    
    private DataNode DeserializeInternal(Utf8JsonReader reader)
    {
        //Skip any comments at start
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.Comment)
                break;
        }

        //Figure out what type the root node is
        switch (reader.TokenType)
        {
            case JsonTokenType.False:
                return new BooleanDataNode(false, null, null);
            case JsonTokenType.True:
                return new BooleanDataNode(true, null, null);
            case JsonTokenType.Number:
                return ParseNumber(reader, null);
            case JsonTokenType.Null:
                return new NullDataNode(null, null);
            case JsonTokenType.String:
                return ParseString(reader, null);
            case JsonTokenType.StartObject:
                return ParseObjectRecursively(reader, null!);
            case JsonTokenType.StartArray:
                return ParseArrayRecursively(reader, null!);
            default:
                throw new JsonException($"Unexpected token at {reader.Position} ({reader.TokenType})");
        }
    }
    
    private static DataNode ParseNumber(Utf8JsonReader reader, string? name)
    {
        var rawValue = reader.ValueSpan.ToString();
        if (rawValue.Length == 0)
            return new NumberValueDataNode(0L, null, null);
        var split = rawValue.Split('.');
        var requiresNegative = rawValue[0] == '-';
        if (split.Length == 1)
        {
            //We try to use the largest type to prevent any accidental data loss
            //Using signed longs only if the number is negative
            if (requiresNegative)
                return new NumberValueDataNode(long.Parse(rawValue, CultureInfo.InvariantCulture), name, null);
            else
                return new NumberValueDataNode(
                    ulong.Parse(rawValue, CultureInfo.InvariantCulture), name, null);
        }
        else
        {
            //Here we use heuristics to guess the most appropriate type for decimal values
            //If the number fits in the decimal range, we use decimal, otherwise double
            if (Utils.CompareNumbersAsString(split[0], requiresNegative ? Utils.DECIMAL_MIN : Utils.DECIMAL_MAX) <= 0)
                return new NumberValueDataNode(decimal.Parse(rawValue, CultureInfo.InvariantCulture), name, null);
            else
                return new NumberValueDataNode(double.Parse(rawValue, CultureInfo.InvariantCulture), name, null);
        }
    }

    private static DataNode ParseString(Utf8JsonReader reader, string? name) =>
        new StringDataNode(reader.ValueSpan.ToString(), name, null);

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
                    if (newName is null)
                        ThrowFormatJsonException<int>(reader, "Object property declared inside object without a property name");
                    result = ParseObjectRecursively(reader, newName);
                    instance.Add(result);
                    break;
                case JsonTokenType.StartArray:
                    if (newName is null)
                        ThrowFormatJsonException<int>(reader, "Array property declared inside object without a property name");
                    result = ParseArrayRecursively(reader, newName);
                    instance.Add(result);
                    break;
                case JsonTokenType.EndObject:
                    return instance;
                default:
                    return ThrowFormatJsonException<DataNode>(reader);
            }
        }
        return ThrowUnexpectedEndJsonException<DataNode>(reader, "Unexpected end of JSON object");
    }

    private DataNode ParseArrayRecursively(Utf8JsonReader reader, string name)
    {
        var instance = new ArrayDataNode(null, name, null);
        while (reader.Read())
        {
            DataNode result;
            switch (reader.TokenType)
            {
                case JsonTokenType.Comment:
                    continue;
                case JsonTokenType.PropertyName:
                    return ThrowFormatJsonException<DataNode>(reader, "Arrays cannot contain property names");
                case JsonTokenType.StartArray:
                    result = ParseArrayRecursively(reader, name);
                    instance.Add(result);
                    break;
                case JsonTokenType.StartObject:
                    result = ParseObjectRecursively(reader, name);
                    instance.Add(result);
                    break;
                case JsonTokenType.EndArray:
                    return instance;
                default:
                    return ThrowFormatJsonException<DataNode>(reader);
            }
        }
        return ThrowUnexpectedEndJsonException<DataNode>(reader, "Unexpected end of JSON array");
    }

    public byte[] SerializeToBytes(DataNode data)
    {
        using var memoryStream = new MemoryStream(256);
        using var writer = new Utf8JsonWriter(memoryStream);
        switch (data)
        {
            case ObjectDataNode obj:
                SerializeNode(obj, writer, false);
                break;
            case ArrayDataNode array:
                SerializeNode(array, writer, true);
                break;
            default:
                throw new NotSupportedException($"{nameof(DataNode)}s of type {data.GetType()} cannot be root of a JSON document");
        }
        
        writer.Flush();
        return memoryStream.ToArray();
    }

    public string Serialize(DataNode data) => Encoding.UTF8.GetString(SerializeToBytes(data));

    private static void SerializeNode(DataNode node, Utf8JsonWriter writer, bool inArray)
    {
        switch (node)
        {
            case BooleanDataNode boolean:
                SerializeBooleanNode(writer, inArray, boolean);
                break;
            case NumberValueDataNode number:
                SerializeNumberNode(writer, inArray, number);
                break;
            case StringDataNode stringData:
                SerializeStringNode(writer, inArray, stringData);
                break;
            case ObjectDataNode objectData:
                SerializeObjectNode(writer, inArray, objectData);
                break;
            case ArrayDataNode array:
                SerializeArrayNode(writer, inArray, array);
                break;
            case NullDataNode nullValue:
                SerializeNullNode(writer, inArray, nullValue);
                break;
            default:
                throw new NotSupportedException($"{nameof(DataNode)}s of type {node.GetType()} are not supported");
        }
    }

    private static void SerializeBooleanNode(Utf8JsonWriter writer, bool inArray, BooleanDataNode boolean)
    {
        if (inArray)
            writer.WriteBooleanValue(boolean.TypedValue);
        else
            writer.WriteBoolean(boolean.Name.EnsureNotNull(), boolean.TypedValue);
    }

    private static void SerializeNullNode(Utf8JsonWriter writer, bool inArray, NullDataNode nullValue)
    {
        if (inArray)
            writer.WriteNullValue();
        else
            writer.WriteNull(nullValue.Name.EnsureNotNull());
    }

    private static void SerializeArrayNode(Utf8JsonWriter writer, bool inArray, ArrayDataNode array)
    {
        if (!inArray && array.Name is not null)
            writer.WriteStartArray(array.Name);
        else
            writer.WriteStartArray();

        foreach (var property in array)
        {
            SerializeNode(property, writer, true);
        }

        writer.WriteEndArray();
    }

    private static void SerializeObjectNode(Utf8JsonWriter writer, bool inArray, ObjectDataNode objectData)
    {
        if (!inArray && objectData.Name is not null)
            writer.WriteStartObject(objectData.Name);
        else
            writer.WriteStartObject();
        foreach (var property in objectData.Values)
        {
            SerializeNode(property, writer, false);
        }

        writer.WriteEndObject();
    }

    private static void SerializeStringNode(Utf8JsonWriter writer, bool inArray, StringDataNode stringData)
    {
        if (inArray)
            writer.WriteStringValue(stringData.TypedValue);
        else
            writer.WriteString(stringData.Name.EnsureNotNull(), stringData.TypedValue);
    }

    private static void SerializeNumberNode(Utf8JsonWriter writer, bool inArray, NumberValueDataNode number)
    {
        var value = number.SerializeToString();
        if (value is null)
        {
            if (inArray)
                writer.WriteNullValue();
            else
                writer.WriteNull(number.Name.EnsureNotNull());
        }
        else
        {
            if (inArray)
                writer.WriteRawValue(value);
            else
            {
                writer.WritePropertyName(number.Name.EnsureNotNull());
                writer.WriteRawValue(value);
            }
        }
    }

    [DoesNotReturn]
    private T ThrowFormatJsonException<T>(Utf8JsonReader reader, string? explanation = null) =>
        throw new JsonException($"Unexpected token of type <{reader.TokenType}> at {reader.Position}"
                                + (explanation is null ? string.Empty : $" ({explanation})"));
    
    [DoesNotReturn]
    private T ThrowUnexpectedEndJsonException<T>(Utf8JsonReader reader, string? explanation = null) =>
        throw new JsonException("Unexpected end of JSON"
                                + (explanation is null ? string.Empty : $" ({explanation})"));
}