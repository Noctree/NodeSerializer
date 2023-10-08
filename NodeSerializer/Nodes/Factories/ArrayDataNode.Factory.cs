namespace NodeSerializer.Nodes;

public partial class ArrayDataNode
{
    public static ArrayDataNode CreateUntypedEmpty() => new(null, null, null, null);
    
    public static ArrayDataNode CreateEmptyWithUntypedElements(Type implementationType) => new(implementationType, null, null, null);
    public static ArrayDataNode CreateEmptyWithUntypedImplementation(Type elementType) => new(null, elementType, null, null);
    public static ArrayDataNode CreateEmptyWithUntypedImplementation(Type elementType, string name) => new(null, elementType, name, null);
    public static ArrayDataNode CreateEmpty(Type type, Type elementType) => new(type, elementType, null, null);
    public static ArrayDataNode CreateEmpty(Type type, Type elementType, string name) => new(type, elementType, name, null);
}