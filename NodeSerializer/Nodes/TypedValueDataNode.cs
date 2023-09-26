using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NodeSerializer.Nodes;

public class TypedValueDataNode<T> : ValueDataNode where T : struct, IFormattable
{
    public override object Value
    {
        [SuppressMessage("Sonar Consistency Issue", "S4275", Justification = "Base Object value is ignored for performance reasons")]
        get => TypedValue;
        [SuppressMessage("Sonar Consistency Issue", "S4275", Justification = "Base Object value is ignored for performance reasons")]
        set => TypedValue = (T)value;
    }

    public T TypedValue { get; set; }

    public TypedValueDataNode(T value, string? name, DataNode? parent) : base(Utils.SHARED_DUMMY_OBJECT_VALUE, name, parent)
    {
        TypedValue = value;
    }

    public override string? SerializeToString()
    {
        return TypedValue.ToString(null, CultureInfo.InvariantCulture);
    }
}