using FluentAssertions;
using NodeSerializer.Nodes;
using NodeSerializer.Serialization.Json;

namespace NodeSerializer.Tests;

public class JsonSerializationTests
{
    private readonly JsonDataNodeSerializer _serializer = new();
    
    [Fact]
    public void SimpleUntypedObject_Serialization_ShouldSucceed()
    {
        var obj = ObjectDataNode.CreateUntypedEmpty();
        var intProp = NumberValueDataNode.Create(32);
        var doubleProp = NumberValueDataNode.Create(420.69);
        var boolProp = BooleanDataNode.Create(true);
        var stringProp = StringDataNode.Create("Hello");
        var nullProp = NullDataNode.Create();
        
        obj.Add("Number", intProp);
        obj.Add("Boolean", boolProp);
        obj.Add("String", stringProp);
        obj.Add("NullValue", nullProp);
        obj.Add("Double", doubleProp);
        
        var json = _serializer.Serialize(obj);
        json.Should().Be("""{"Number":32,"Boolean":true,"String":"Hello","NullValue":null,"Double":420.69}""");
    }
    
    [Fact]
    public void SimpleUntypedArray_Serialization_ShouldSucceed()
    {
        var array = ArrayDataNode.CreateUntypedEmpty();
        var intProp = NumberValueDataNode.Create(32);
        var doubleProp = NumberValueDataNode.Create(420.69);
        var boolProp = BooleanDataNode.Create(true);
        var stringProp = StringDataNode.Create("Hello");
        var nullProp = NullDataNode.Create();
        
        array.Add(intProp);
        array.Add(boolProp);
        array.Add(stringProp);
        array.Add(nullProp);
        array.Add(doubleProp);
        
        var json = _serializer.Serialize(array);
        json.Should().Be("""[32,true,"Hello",null,420.69]""");
    }
    
    [Fact]
        public void NestedUntypedObject_Serialization_ShouldSucceed()
        {
            var obj = ObjectDataNode.CreateUntypedEmpty();
            var nestedObj = ObjectDataNode.CreateUntypedEmpty();

            nestedObj.Add("NestedNumber", NumberValueDataNode.Create(42));
            nestedObj.Add("NestedBoolean", BooleanDataNode.Create(false));
            
            obj.Add("Number", NumberValueDataNode.Create(32));
            obj.Add("NestedObject", nestedObj);

            var json = _serializer.Serialize(obj);
            json.Should().Be("""{"Number":32,"NestedObject":{"NestedNumber":42,"NestedBoolean":false}}""");
        }

        [Fact]
        public void NestedUntypedArray_Serialization_ShouldSucceed()
        {
            var array = ArrayDataNode.CreateUntypedEmpty();
            var nestedArray = ArrayDataNode.CreateUntypedEmpty();

            nestedArray.Add(NumberValueDataNode.Create(42));
            nestedArray.Add(BooleanDataNode.Create(false));

            array.Add(NumberValueDataNode.Create(32));
            array.Add(nestedArray);

            var json = _serializer.Serialize(array);
            json.Should().Be("[32,[42,false]]");
        }

        [Fact]
        public void EmptyObject_Serialization_ShouldReturnEmptyJsonObject()
        {
            var obj = ObjectDataNode.CreateUntypedEmpty();
            var json = _serializer.Serialize(obj);
            json.Should().Be("{}");
        }

        [Fact]
        public void EmptyArray_Serialization_ShouldReturnEmptyJsonArray()
        {
            var array = ArrayDataNode.CreateUntypedEmpty();
            var json = _serializer.Serialize(array);
            json.Should().Be("[]");
        }
        
        [Fact]
        public void ObjectWithSpecialCharacters_Serialization_ShouldEscape()
        {
            var obj = ObjectDataNode.CreateUntypedEmpty();
            obj.Add("StringWithSpecialChars", StringDataNode.Create("\"Hello\nWorld\""));

            var json = _serializer.Serialize(obj);
            json.Should().Be("""{"StringWithSpecialChars":"\u0022Hello\nWorld\u0022"}""");
        }
}