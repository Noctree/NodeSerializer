using System.Text.Json;
using FluentAssertions;
using NodeSerializer.Nodes;
using NodeSerializer.Serialization;

namespace NodeSerializer.Tests;

public class JsonDeserializationTests
{
    private class SimpleJsonObject
    {
        public int Integer { get; set; }
        public string String { get; set; }
        public bool Boolean { get; set; }
        public decimal Decimal { get; set; }
    }
    
    [Fact]
    public void SimpleJsonObjectDeserialization_Valid_ShouldSucceed()
    {
        var obj = new SimpleJsonObject()
        {
            Integer = 69,
            String = "Hello World",
            Boolean = true,
            Decimal = 420.1337m
        };
        var json = JsonSerializer.Serialize(obj);
        var serializer = new JsonDataNodeSerializer();
        var data = serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();

        var objData = data.AsObject();
        var validNames = new HashSet<string>()
        {
            nameof(SimpleJsonObject.Integer),
            nameof(SimpleJsonObject.Boolean),
            nameof(SimpleJsonObject.Decimal),
            nameof(SimpleJsonObject.String)
        };
        objData
            .Keys
            .Should()
            .OnlyContain(n => validNames.Contains(n));
        var integerData = objData[nameof(SimpleJsonObject.Integer)];
        integerData.Should().BeOfType<NumberValueDataNode>();
        var integerDataNode = integerData.AsNumber();
        integerDataNode.TypedValue.AsInt().Should().Be(69);
        
        var booleanData = objData[nameof(SimpleJsonObject.Boolean)];
        booleanData.Should().BeOfType<BooleanDataNode>();
        var booleanDataNode = booleanData.AsBoolean();
        booleanDataNode.TypedValue.AsBool().Should().BeTrue();
        
        var decimalData = objData[nameof(SimpleJsonObject.Decimal)];
        decimalData.Should().BeOfType<NumberValueDataNode>();
        var decimalDataNode = decimalData.AsNumber();
        decimalDataNode.TypedValue.AsDecimal().Should().Be(420.1337m);
        
        var stringData = objData[nameof(SimpleJsonObject.String)];
        stringData.Should().BeOfType<StringDataNode>();
        stringData.AsString().TypedValue.AsString().Should().Be("Hello World");
    }
}