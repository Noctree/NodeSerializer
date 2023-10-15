using FluentAssertions;
using NodeSerializer.Nodes;
using NodeSerializer.Serialization;

namespace NodeSerializer.Tests;

public class ObjectSerializerTests_Serialize
{
    [Serializable]
    public class SimpleObject
    {
        public int IntVariable { get; set; }
        public string StringVariable { get; set; }
        public bool BooleanVariable { get; set; }
    }
   
    [Fact]
    public void ShouldSerializeNull()
    {
        // Act
        var result = ObjectSerializer.Serialize(null);

        // Assert
        result.Should().BeOfType<NullDataNode>();
    }

    [Fact]
    public void ShouldSerializeInt()
    {
        // Arrange
        var input = 42;

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result
            .Should()
            .BeOfType<NumberDataNode>();
        result.AsNumber().TypedValue.AsInt().Should().Be(input);
    }

    [Fact]
    public void ShouldSerializeString()
    {
        // Arrange
        var input = "hello";

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result.Should().BeOfType<StringDataNode>();
        result.AsString().TypedValue.AsString().Should().Be(input);
    }

    [Fact]
    public void ShouldSerializeArray()
    {
        // Arrange
        var input = new int[] { 1, 2, 3 };

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result.Should().BeOfType<ArrayDataNode>();
        var arr = result.AsArray();
        arr[0].AsNumber().TypedValue.AsInt().Should().Be(input[0]);
        arr[1].AsNumber().TypedValue.AsInt().Should().Be(input[1]);
        arr[2].AsNumber().TypedValue.AsInt().Should().Be(input[2]);
    }

    [Fact]
    public void ShouldSerializeDictionary()
    {
        // Arrange
        var input = new Dictionary<string, int>
        {
            {"one", 1},
            {"two", 2},
            {"three", 3}
        };

        // Act
        var result = ObjectSerializer.Serialize(input);

        // Assert
        result.Should().BeOfType<ObjectDataNode>();
        var obj = result.AsObject();
        obj.Keys.Should().Contain("one");
        obj.Keys.Should().Contain("two");
        obj.Keys.Should().Contain("three");
        obj["one"].AsNumber().TypedValue.AsInt().Should().Be(input["one"]);
        obj["two"].AsNumber().TypedValue.AsInt().Should().Be(input["two"]);
        obj["three"].AsNumber().TypedValue.AsInt().Should().Be(input["three"]);
    }

    [Fact]
    public void ShouldSerializeObject()
    {
        //Arrange
        var obj = new SimpleObject()
        {
            BooleanVariable = true,
            IntVariable = 69,
            StringVariable = "Hello!"
        };
        
        //Act
        var result = ObjectSerializer.Serialize(obj);
        
        //Assert
        result.Should().BeOfType<ObjectDataNode>();
        var objData = result.AsObject();
        objData.Keys.Should().Contain("BooleanVariable");
        objData.Keys.Should().Contain("IntVariable");
        objData.Keys.Should().Contain("StringVariable");
        objData["BooleanVariable"].AsBoolean().TypedValue.Should().BeTrue();
        objData["IntVariable"].AsNumber().TypedValue.AsInt().Should().Be(69);
        objData["StringVariable"].AsString().TypedValue.AsString().Should().Be("Hello!");
    }
}