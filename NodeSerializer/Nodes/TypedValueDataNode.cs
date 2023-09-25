namespace NodeSerializer.Nodes;

public class TypedValueDataNode<T> : ValueDataNode where T : struct
{
    private static readonly object DummyBaseValue = (object)0;

    public override object Value
    {
        get => TypedValue;
        set => TypedValue = (T)value;
    }

    public T TypedValue { get; set; }

    public TypedValueDataNode(T value, string? name, DataNode? parent) : base(DummyBaseValue, name, parent)
    {
    }
}