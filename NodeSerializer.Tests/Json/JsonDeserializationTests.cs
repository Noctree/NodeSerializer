using System.Text.Json;
using FluentAssertions;
using NodeSerializer.Nodes;
using NodeSerializer.Serialization.Json;

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

    private class NestedJsonObject
    {
        public string String { get; set; }
        public SimpleJsonObject ObjectA { get; set; }
        public SimpleJsonObject ObjectB { get; set; }
    }

    private class ArrayJsonObject
    {
        public bool Boolean { get; set; }
        public int[] Integers { get; set; }
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
        integerData.Should().BeOfType<NumberDataNode>();
        var integerDataNode = integerData.AsNumber();
        integerDataNode.TypedValue.AsInt().Should().Be(69);
        
        var booleanData = objData[nameof(SimpleJsonObject.Boolean)];
        booleanData.Should().BeOfType<BooleanDataNode>();
        var booleanDataNode = booleanData.AsBoolean();
        booleanDataNode.TypedValue.Should().BeTrue();
        
        var decimalData = objData[nameof(SimpleJsonObject.Decimal)];
        decimalData.Should().BeOfType<NumberDataNode>();
        var decimalDataNode = decimalData.AsNumber();
        decimalDataNode.TypedValue.AsDecimal().Should().Be(420.1337m);
        
        var stringData = objData[nameof(SimpleJsonObject.String)];
        stringData.Should().BeOfType<StringDataNode>();
        stringData.AsString().TypedValue.AsString().Should().Be("Hello World");
    }

    [Fact]
    public void NestedJsonObjectDeserialization_Valid_ShouldSucceed()
    {
        var obj = new NestedJsonObject()
        {
            String = "Hello World",
            ObjectA = new SimpleJsonObject()
            {
                Integer = 69,
                Boolean = true,
                Decimal = 420.1337m,
                String = "How are you"
            },
            ObjectB = new SimpleJsonObject()
            {
                Integer = 420,
                Boolean = false,
                Decimal = 420.1337m,
                String = "Today?"
            }
        };
        var json = JsonSerializer.Serialize(obj);
        var serializer = new JsonDataNodeSerializer();
        var data = serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        
        var objData = data.AsObject();
        var validNames = new HashSet<string>()
        {
            nameof(NestedJsonObject.String),
            nameof(NestedJsonObject.ObjectA),
            nameof(NestedJsonObject.ObjectB)
        };
        objData
            .Keys
            .Should()
            .OnlyContain(n => validNames.Contains(n));
        objData[nameof(NestedJsonObject.String)].AsString().TypedValue.AsString().Should().Be("Hello World");

        var obj1 = objData[nameof(NestedJsonObject.ObjectA)].AsObject();
        var integerData1 = obj1[nameof(SimpleJsonObject.Integer)];
        integerData1.Should().BeOfType<NumberDataNode>();
        integerData1.AsNumber().TypedValue.AsInt().Should().Be(69);
        
        var booleanData1 = obj1[nameof(SimpleJsonObject.Boolean)];
        booleanData1.Should().BeOfType<BooleanDataNode>();
        booleanData1.AsBoolean().TypedValue.Should().BeTrue();
        
        var decimalData1 = obj1[nameof(SimpleJsonObject.Decimal)];
        decimalData1.Should().BeOfType<NumberDataNode>();
        decimalData1.AsNumber().TypedValue.AsDecimal().Should().Be(420.1337m);
        
        var stringData1 = obj1[nameof(SimpleJsonObject.String)];
        stringData1.Should().BeOfType<StringDataNode>();
        stringData1.AsString().TypedValue.AsString().Should().Be("How are you");
        
        var obj2 = objData[nameof(NestedJsonObject.ObjectB)].AsObject();
        var integerData2 = obj2[nameof(SimpleJsonObject.Integer)];
        integerData2.Should().BeOfType<NumberDataNode>();
        integerData2.AsNumber().TypedValue.AsInt().Should().Be(420);
        
        var booleanData2 = obj2[nameof(SimpleJsonObject.Boolean)];
        booleanData2.Should().BeOfType<BooleanDataNode>();
        booleanData2.AsBoolean().TypedValue.Should().BeFalse();
        
        var decimalData2 = obj2[nameof(SimpleJsonObject.Decimal)];
        decimalData2.Should().BeOfType<NumberDataNode>();
        decimalData2.AsNumber().TypedValue.AsDecimal().Should().Be(420.1337m);
        
        var stringData2 = obj2[nameof(SimpleJsonObject.String)];
        stringData2.Should().BeOfType<StringDataNode>();
        stringData2.AsString().TypedValue.AsString().Should().Be("Today?");
    }

    [Fact]
    public void ArrayJsonObjectDeserialization_Valid_ShouldSucceed()
    {
        var obj = new ArrayJsonObject()
        {
            Boolean = false,
            Integers = new int[]
            {
                3,
                4,
                5,
                6
            }
        };
        
        var json = JsonSerializer.Serialize(obj);
        var serializer = new JsonDataNodeSerializer();
        var data = serializer.Deserialize(json);
        data.Should().BeOfType<ObjectDataNode>();
        
        var objData = data.AsObject();
        var validNames = new HashSet<string>()
        {
            nameof(ArrayJsonObject.Boolean),
            nameof(ArrayJsonObject.Integers)
        };
        objData
            .Keys
            .Should()
            .OnlyContain(n => validNames.Contains(n));
        objData[nameof(ArrayJsonObject.Boolean)].AsBoolean().TypedValue.Should().BeFalse();
        objData[nameof(ArrayJsonObject.Integers)]
            .AsArray()
            .Count
            .Should()
            .Be(4);
        var array = objData[nameof(ArrayJsonObject.Integers)].AsArray();
        array[0].TypeOf.Should().Be<ulong>();
        array[1].TypeOf.Should().Be<ulong>();
        array[2].TypeOf.Should().Be<ulong>();
        array[3].TypeOf.Should().Be<ulong>();
        array[0].AsNumber().TypedValue.AsInt().Should().Be(3);
        array[1].AsNumber().TypedValue.AsInt().Should().Be(4);
        array[2].AsNumber().TypedValue.AsInt().Should().Be(5);
        array[3].AsNumber().TypedValue.AsInt().Should().Be(6);
    }

    [Fact]
    public void ArrayOfJsonObjects_Valid_ShouldSucceed()
    {
        var array = new SimpleJsonObject[]
        {
            new SimpleJsonObject()
            {
                Integer = 2,
                Boolean = true,
                Decimal = -32.1337m,
                String = "ABC"
            },
            new SimpleJsonObject()
            {
                Integer = -6,
                Boolean = false,
                Decimal = 420.1337m,
                String = "Today?"
            },
            new SimpleJsonObject()
            {
                Integer = 0,
                Boolean = true,
                Decimal = 0.1337m,
                String = "How are you"
            }
        };

        var json = JsonSerializer.Serialize(array);
        var serializer = new JsonDataNodeSerializer();
        var data = serializer.Deserialize(json);
        data.NodeType.Should().Be(DataNodeType.Array);
        var arrayNode = data.AsArray();

        foreach (var (orig, node) in array.Zip(arrayNode))
        {
            node.NodeType.Should().Be(DataNodeType.Object);
            var obj = node.AsObject();
            obj[nameof(SimpleJsonObject.Boolean)].AsBoolean().TypedValue.Should().Be(orig.Boolean);
            obj[nameof(SimpleJsonObject.Decimal)].AsNumber().TypedValue.AsDecimal().Should().Be(orig.Decimal);
            obj[nameof(SimpleJsonObject.Integer)].AsNumber().TypedValue.AsInt().Should().Be(orig.Integer);
            obj[nameof(SimpleJsonObject.String)].AsString().TypedValue.AsString().Should().Be(orig.String);
        }
    }
}