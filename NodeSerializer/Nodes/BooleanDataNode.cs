namespace NodeSerializer.Nodes;

public class BooleanDataNode : TypedValueDataNode<BooleanDataNode.FormattableBoolean>
{
    private const string TRUE_STRING = "true";
    private const string FALSE_STRING = "false";
    
    public string ValueAsString => TypedValue ? TRUE_STRING : FALSE_STRING;
    
    public BooleanDataNode(bool value, string? name, DataNode? parent) : base(value, name, parent)
    {
    }
    
    public readonly struct FormattableBoolean : IFormattable
    {
        public readonly bool Value;

        public FormattableBoolean(bool value)
        {
            Value = value;
        }
    
        public string ToString(string? format, IFormatProvider? formatProvider) => Value ? TRUE_STRING : FALSE_STRING;

        public bool AsBool() => Value;
        
        public static implicit operator FormattableBoolean(bool value) => new(value);
        public static implicit operator bool(FormattableBoolean value) => value.Value;
    }
}