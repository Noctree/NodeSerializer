using FluentAssertions;
using NodeSerializer.Nodes;
using NodeSerializer.Serialization;

namespace NodeSerializer.Tests;

public class ObjectSerializerTests_Serialize_EdgeCases
{
    [Serializable]
    public class SimpleObject
    {
        public int IntVariable { get; set; }
        public string StringVariable { get; set; }
        public bool BooleanVariable { get; set; }
    }

    [Serializable]
    public class ContainerObject
    {
        public SimpleObject NestedObject { get; set; }
    }
    
    [Fact]
    public void ShouldSerializeNestedDictionary()
    {
        // Arrange
        var input = new Dictionary<string, Dictionary<string, int>>
        {
            { "outer1", new Dictionary<string, int> { { "inner1", 1 }, { "inner2", 2 } } },
            { "outer2", new Dictionary<string, int> { { "inner3", 3 }, { "inner4", 4 } } }
        };

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result.Should().BeOfType<ObjectDataNode>();
        var obj = result.AsObject();
        obj.Keys.Should().Contain("outer1");
        obj.Keys.Should().Contain("outer2");
        
        obj["outer1"].AsObject()["inner1"].AsNumber().TypedValue.AsInt().Should().Be(1);
        obj["outer1"].AsObject()["inner2"].AsNumber().TypedValue.AsInt().Should().Be(2);
        
        obj["outer2"].AsObject()["inner3"].AsNumber().TypedValue.AsInt().Should().Be(3);
        obj["outer2"].AsObject()["inner4"].AsNumber().TypedValue.AsInt().Should().Be(4);
    }

    [Fact]
    public void ShouldSerializeEmptyArray()
    {
        // Arrange
        var input = new int[0];

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result.Should().BeOfType<ArrayDataNode>();
        result.AsArray().Should().BeEmpty();
    }

    [Fact]
    public void ShouldSerializeObjectWithNestedObject()
    {
        // Arrange
        var obj = new SimpleObject
        {
            IntVariable = 42,
            StringVariable = "nested",
            BooleanVariable = true
        };
        var container = new ContainerObject() { NestedObject = obj };

        // Act
        var result = ObjectSerializer.Serialize(container);

        // Assert
        result.Should().BeOfType<ObjectDataNode>();
        result.AsObject().Keys.Should().Contain("NestedObject");
        var nestedObj = result.AsObject()["NestedObject"].AsObject();
        nestedObj.Keys.Should().Contain("IntVariable");
        nestedObj.Keys.Should().Contain("StringVariable");
        nestedObj.Keys.Should().Contain("BooleanVariable");
        nestedObj["IntVariable"].AsNumber().TypedValue.AsInt().Should().Be(42);
        nestedObj["StringVariable"].AsString().TypedValue.AsString().Should().Be("nested");
        nestedObj["BooleanVariable"].AsBoolean().TypedValue.Should().BeTrue();
    }

    [Fact]
    public void ShouldThrowOnUnsupportedKeyType()
    {
        // Arrange
        var input = new Dictionary<SimpleObject, int>
        {
            { new SimpleObject { IntVariable = 42 }, 1 }
        };

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => ObjectSerializer.Serialize(input));
    }

    [Fact]
    public void ShouldSerializeListOfNulls()
    {
        // Arrange
        var input = new List<string?> { null, null, null };

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result.Should().BeOfType<ArrayDataNode>();
        var array = result.AsArray();
        array.Should().HaveCount(3);
        foreach (var node in array)
        {
            node.Should().BeOfType<NullDataNode>();
        }
    }
}