using System;
using System.Text.Json;
using FluentAssertions;
using NodeSerializer.Nodes;
using NodeSerializer.Serialization.Json;
using Xunit;

namespace NodeSerializer.Tests;

public class JsonDeserializationTestsEdgeCases
{
    private readonly JsonDataNodeSerializer _serializer = new();

    [Fact]
    public void NullValues_Deserialization_ShouldBeOfTypeNullDataNode()
    {
        var json = JsonSerializer.Serialize((object)null);
        var data = _serializer.Deserialize(json);
        data.Should().BeOfType<NullDataNode>();
    }

    [Fact]
    public void EmptyObject_Deserialization_ShouldBeEmpty()
    {
        var json = "{}";
        var data = _serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        data.AsObject().Should().BeEmpty();
    }

    [Fact]
    public void EmptyArray_Deserialization_ShouldBeEmpty()
    {
        var json = "[]";
        var data = _serializer.Deserialize(json);
        data.Should().BeOfType<ArrayDataNode>();
        data.AsArray().Should().BeEmpty();
    }

    [Fact]
    public void ObjectWithNullValue_Deserialization_ShouldHaveNullDataNode()
    {
        var json = "{\"key\": null}";
        var data = _serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        data.AsObject()["key"].Should().BeOfType<NullDataNode>();
    }

    [Fact]
    public void ArrayWithNullValue_Deserialization_ShouldHaveNullDataNode()
    {
        var json = "[null]";
        var data = _serializer.Deserialize(json);
        data.Should().BeOfType<ArrayDataNode>();
        data.AsArray()[0].Should().BeOfType<NullDataNode>();
    }

    [Fact]
    public void MixedTypes_Deserialization_ShouldSucceed()
    {
        var obj = new
        {
            Integer = 69,
            String = "Hello World",
            Array = new int[] { 1, 2, 3 },
            Object = new { Nested = "NestedValue" }
        };

        var json = JsonSerializer.Serialize(obj);
        var data = _serializer.Deserialize(json);

        data.Should().BeOfType<ObjectDataNode>();

        var objData = data.AsObject();

        objData[nameof(obj.Integer)].AsNumber().TypedValue.AsInt().Should().Be(69);
        objData[nameof(obj.String)].AsString().TypedValue.AsString().Should().Be("Hello World");
        objData[nameof(obj.Array)].AsArray().Count.Should().Be(3);
        objData[nameof(obj.Object)].AsObject()[nameof(obj.Object.Nested)].AsString().TypedValue.AsString().Should()
            .Be("NestedValue");
    }

    [Fact]
    public void LargeNumbers_Deserialization_ShouldSucceed()
    {
        var largeNumber = Int64.MaxValue;
        var json = JsonSerializer.Serialize(new { LargeNumber = largeNumber });
        var data = _serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        data.AsObject()["LargeNumber"].AsNumber().TypedValue.AsLong().Should().Be(largeNumber);
    }

    [Fact]
    public void Deserialization_InvalidJson_ShouldThrowException()
    {
        var invalidJson = "not a json string";
        Assert.ThrowsAny<JsonException>(() => _serializer.Deserialize(invalidJson));
    }

    [Fact]
    public void Deserialization_NullValue_ShouldReturnNullNode()
    {
        var json = "null";
        var serializer = new JsonDataNodeSerializer();
        var data = serializer.Deserialize(json);

        data.Should().BeOfType<NullDataNode>();
    }

    [Fact]
    public void Deserialization_UnsupportedType_ShouldThrowException()
    {
        var json = "{\"unsupportedType\": function(){}}";
        var serializer = new JsonDataNodeSerializer();

        Assert.ThrowsAny<JsonException>(() => serializer.Deserialize(json));
    }

    [Fact]
    public void ComplexObject_InvalidInnerTypes_ShouldNotThrowException()
    {
        var json = "{\"String\": \"valid\", \"IntList\": null, \"StringDict\": [1, 2]}";
        var serializer = new JsonDataNodeSerializer();
        
        var data = serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();

        var objData = data.AsObject();
        objData["String"].AsString().TypedValue.AsString().Should().Be("valid");
        objData["IntList"].Should().BeOfType<NullDataNode>();
        objData["StringDict"].Should().BeOfType<ArrayDataNode>();
        objData["StringDict"].AsArray().Should().ContainInConsecutiveOrder(
            NumberDataNode.Create(1),
            NumberDataNode.Create(2));
    }

    [Fact]
    public void ComplexObject_MismatchedArrayTypes_ShouldNotThrowException()
    {
        var json = "{\"String\": \"valid\", \"IntList\": [1, \"string\", 3]}";
        var serializer = new JsonDataNodeSerializer();
        
        var data = serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        
        var objData = data.AsObject();
        objData["String"].AsString().TypedValue.AsString().Should().Be("valid");
        objData["IntList"].Should().BeOfType<ArrayDataNode>();
        objData["IntList"].AsArray()[0].AsNumber().TypedValue.AsInt().Should().Be(1);
        objData["IntList"].AsArray()[1].AsString().TypedValue.AsString().Should().Be("string");
        objData["IntList"].AsArray()[2].AsNumber().TypedValue.AsInt().Should().Be(3);
    }

    [Fact]
    public void ComplexObject_NestedDictionary_MissingKey_ShouldNotThrowException()
    {
        var json = "{\"String\": \"valid\", \"StringDict\": [{\"key\": \"key1\"}]}";
        var serializer = new JsonDataNodeSerializer();

        var data = serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        
        var objData = data.AsObject();
        objData["String"].AsString().TypedValue.AsString().Should().Be("valid");
        objData["StringDict"].Should().BeOfType<ArrayDataNode>();
        objData["StringDict"].AsArray()[0].AsObject()["key"].AsString().TypedValue.AsString().Should().Be("key1");
    }
}