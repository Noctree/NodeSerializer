using System.Collections;
using System.Reflection;
using System.Text.Json;
using NodeSerializer.Attributes;
using NodeSerializer.Extensions;

namespace NodeSerializer.Serialization.Internal;

public static class SerializationMetadataCache
{
    private static readonly Dictionary<Type, SerializationMetadata> SerializationMetaCache = new();
    private static readonly Dictionary<Type, ConstructorMetadata> ConstructorMetadataCache = new();
    private static readonly Dictionary<Type, PropertyMetadata> PropertyMetadataCache = new();
    
    public static SerializationMetadata GetMetadata(Type type)
    {
        if (SerializationMetaCache.TryGetValue(type, out var metadata))
            return metadata;

        metadata = CreateMetadata(type);
        SerializationMetaCache.Add(type, metadata);
        return metadata;
    }

    public static ConstructorMetadata GetConstructorMetadata(Type type)
    {
        if (ConstructorMetadataCache.TryGetValue(type, out var metadata))
            return metadata;
        
        var serializationMetadata = GetMetadata(type);
        if (serializationMetadata.SerializationMethod == SerializationMethod.Value)
            throw new ArgumentException(
                $"{type} is a primitive type (or a nullable variant), ConstructorMetadata is not required");
        
        metadata = CreateConstructorMetadata(type, serializationMetadata.SerializationMethod);
    }

    private static SerializationMetadata CreateMetadata(Type type)
    {
        var deserializationMode = type.GetCustomAttribute<DeserializationModeAttribute>()?.DeserializationMode;
        deserializationMode ??= DeserializationMode.Constructor;

        var deserializationMethod = DetermineSerializationMethod(type);
        return new SerializationMetadata(type, deserializationMode.Value, deserializationMethod);
    }

    private static SerializationMethod DetermineSerializationMethod(Type type)
    {
        if (type.IsPrimitiveExtended() || type.IsNullablePrimitiveExtended())
            return SerializationMethod.Value;
        if (type.ImplementsInterface(typeof(IDictionary<,>)) || type.IsAssignableTo(typeof(IDictionary)))
            return SerializationMethod.Dictionary;
        if (type.ImplementsInterface(typeof(IEnumerable<>)) || type.IsAssignableTo(typeof(IEnumerable)))
            return SerializationMethod.Array;
        return SerializationMethod.Object;
    }

    private static ConstructorMetadata CreateConstructorMetadata(Type type, SerializationMethod method)
    {
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var constructor = constructors.FirstOrDefault(static c =>
            c.GetCustomAttribute<DeserializationConstructorAttribute>() is not null);
        if (constructor is null)
        {
            constructor = method switch
            {
                SerializationMethod.Array => FindArrayConstructor(type, constructors),
                SerializationMethod.Dictionary => FindDictionaryConstructor(type, constructors),
                SerializationMethod.Object => FindObjectConstructor(type, constructors),
            }
        }
        JsonSerializer.Serialize()
    }

    private static ConstructorInfo FindArrayConstructor(Type type, ConstructorInfo[] constructors)
    {
        ConstructorInfo? constructor;
        if (type.IsArray)
        {
            constructor = Array.Find(constructors, static c =>
            {
                var parameters = c.GetParameters();
                return parameters.Length == 1 &&
                       (parameters[0].ParameterType == typeof(int) || parameters[0].ParameterType == typeof(long));
            });
            
            //We are guaranteed to find this constructor for arrays
            return constructor!;
        }
        
        if (type.IsAssignableFrom(typeof(ArrayList))
            || (type.ImplementsInterface(typeof(ICollection<>), out var interfaceType)
                && type.IsAssignableFrom(typeof(List<>).MakeGenericType(interfaceType.GenericTypeArguments[0]))))
        {
            constructor = Array.Find(constructors, static c => c.GetParameters().Length == 0);
            if (constructor is not null)
                return constructor;
        }


        if (constructor is null)
            throw new MissingMemberException(
                $"{type} is serialized as an array type, " +
                "it must have a constructor with one parameter of type int or long " +
                "that pre-initializes it to the requested length");
        return constructor;
    }
}