using System.Reflection;
using NodeSerializer.Attributes;

namespace NodeSerializer.Serialization.Internal;

public enum SerializationMethod
{
    Value,
    Array,
    Dictionary,
    Object
}

public record PropertyMetadata(string Name, Type Type);

public record ConstructorMetadata(ConstructorInfo Constructor, IList<PropertyMetadata> Parameters); 

public record PropertiesMetadata(Type TargetType, IList<PropertyMetadata> Properties);

public record SerializationMetadata(Type TargetType, DeserializationMode DeserializationMode, SerializationMethod SerializationMethod);