using System.Collections;
using System.Globalization;
using System.Reflection;
using NodeSerializer.Extensions;
using NodeSerializer.Nodes;
using NodeSerializer.Reflection;

namespace NodeSerializer.Serialization;

public static class ObjectSerializer
{
    [ThreadStatic] private static object[]? _singleObjParamArray;
    [ThreadStatic] private static Type[]? _arrayGenericParamsArray;
    [ThreadStatic] private static Type[]? _dictionaryGenericParamsArray;
    [ThreadStatic] private static MethodInfo? _dictionarySerializeGenericMethodInfo;
    [ThreadStatic] private static MethodInfo? _arraySerializeGenericMethodInfo;
    
    /// <summary>
    /// Serialize an object into a DataNode structure
    /// </summary>
    /// <param name="value">object to serialize</param>
    /// <returns>serialized DataNode</returns>
    public static DataNode Serialize(object? value)
    {
        if (value is null)
            return NullDataNode.Create();

        CheckIsSupported(value);
        var type = value.GetType();

        if (type.IsPrimitiveExtended())
            return SerializePrimitive(value);
        
        if (value.GetType().ImplementsInterface(typeof(IDictionary<,>), out var dictionaryInterface))
        {
            _dictionarySerializeGenericMethodInfo ??= typeof(ObjectSerializer)
                .GetRequiredMethod(nameof(SerializeDictionaryGeneric), BindingFlags.Static | BindingFlags.NonPublic);
            _dictionaryGenericParamsArray ??= new Type[2];
            _dictionaryGenericParamsArray[0] = dictionaryInterface.GenericTypeArguments[0];
            _dictionaryGenericParamsArray[1] = dictionaryInterface.GenericTypeArguments[1];
            var genericMethod = _dictionarySerializeGenericMethodInfo
                .MakeGenericMethod(_dictionaryGenericParamsArray);

            _singleObjParamArray ??= new object[1];
            _singleObjParamArray[0] = value;
            try
            {
                return (DataNode)genericMethod.Invoke(null, _singleObjParamArray)!;
            }
            catch (TargetInvocationException ex)
            {
                //Unwrap exception
                throw ex.InnerException ?? throw ex;
            }
        }

        if (value.GetType().ImplementsInterface(typeof(IEnumerable<>), out var enumerableInterface))
        {
            _arraySerializeGenericMethodInfo ??= typeof(ObjectSerializer)
                .GetRequiredMethod(nameof(SerializeArrayGeneric), BindingFlags.Static | BindingFlags.NonPublic);
            _arrayGenericParamsArray ??= new Type[1];
            _arrayGenericParamsArray[0] = enumerableInterface.GenericTypeArguments[0];
            var genericMethod = _arraySerializeGenericMethodInfo
                .MakeGenericMethod(_arrayGenericParamsArray);

            _singleObjParamArray ??= new object[1];
            _singleObjParamArray[0] = value;
            try
            {
                return (DataNode)genericMethod.Invoke(null, _singleObjParamArray)!;
            }
            catch (TargetInvocationException ex)
            {
                //Unwrap exception
                throw ex.InnerException ?? throw ex;
            }
        }
        
        //IDictionary<TKey, TValue> does not implement IDictionary, so we also check for the non-generic version
        if (value is IDictionary dictionary)
        {
            return SerializeDictionary(dictionary);
        }

        if (value is IEnumerable enumerable)
        {
            return SerializeArray(enumerable);
        }

        return SerializeObject(value);
    }
    
    private static bool CheckIsSupported(object obj) => obj switch
    {
        // ReSharper disable once PossibleMistakenCallToGetType.2
        Type _ => throw new NotSupportedException($"Objects of type {nameof(Type)} are not supported."),
        Exception _ => throw new NotSupportedException($"Serialization of exceptions is not supported."),
        _ => obj.GetType().GetCustomAttribute<SerializableAttribute>() is not null || (obj.GetType().IsArray
            ? true
            : throw new NotSupportedException($"Object of type {obj.GetType().Name} is not serializable. (Must be marked with the {nameof(SerializableAttribute)}"))
    };

    private static DataNode SerializeObject(object value)
    {
        var type = value.GetType();
        var properties = UntypedAccessorCache.GetPropertyGettersFor(type);
        var result = ObjectDataNode.CreateEmpty(type);
        foreach (var property in properties)
        {
            result.Add(property.Key, Serialize(property.Value(value)));
        }

        return result;
    }

    private static DataNode SerializeDictionary(IDictionary dictionary)
    {
        var node = ObjectDataNode.CreateEmpty(dictionary.GetType());
        foreach (var key in dictionary.Keys)
        {
            var serializedKey = SerializeKey(key); // Assumes TKey can be meaningfully transformed to string
            var value = dictionary[key];
            node.Add(serializedKey, Serialize(value));
        }

        return node;
    }

    private static ObjectDataNode SerializeDictionaryGeneric<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        var objNode = ObjectDataNode.CreateUntypedEmpty();

        foreach (var kvp in dict)
        {
            var key = SerializeKey(kvp.Key); // Assumes TKey can be meaningfully transformed to string
            var valueNode = Serialize(kvp.Value);
            objNode.Add(key, valueNode);
        }

        return objNode;
    }

    private static string SerializeKey(object? key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (!key.GetType().IsPrimitiveExtended())
            throw new NotSupportedException($"{key.GetType()} objects cannot be keys in a dictionary, only primitive types, decimal and strings are supported.");

        string result;
        if (key is IFormattable formattable)
            result = formattable.ToString(null, CultureInfo.InvariantCulture);
        else
            result = key.ToString() ?? throw new NotSupportedException("Dictionary keys converted to string via calling ToString cannot return null");
        
        if (string.IsNullOrEmpty(result))
            throw new NotSupportedException("Dictionary keys converted to string via calling ToString cannot result in null or an empty string");
        return result;
    }

    private static DataNode SerializeArrayGeneric<T>(IEnumerable<T> enumerable)
    {
        var arrayNode = ArrayDataNode.CreateEmpty(enumerable.GetType(), typeof(T));
        foreach (var value in enumerable)
        {
            arrayNode.Add(Serialize(value));
        }

        return arrayNode;
    }

    private static DataNode SerializeArray(IEnumerable? enumerable)
    {
        if (enumerable is null)
            return NullDataNode.Create();

        var node = ArrayDataNode.CreateEmptyWithUntypedElements(enumerable.GetType());
        foreach (var value in enumerable)
        {
            node.Add(Serialize(value));
        }

        return node;
    }

    private static DataNode SerializePrimitive(object value) =>
        value switch
        {
            bool b => BooleanDataNode.Create(b),
            string s => StringDataNode.Create(s),
            decimal d => NumberValueDataNode.Create(d),
            byte b => NumberValueDataNode.Create(b),
            short s => NumberValueDataNode.Create(s),
            int i => NumberValueDataNode.Create(i),
            long l => NumberValueDataNode.Create(l),
            float f => NumberValueDataNode.Create(f),
            double d => NumberValueDataNode.Create(d),
            sbyte b => NumberValueDataNode.Create(b),
            ushort s => NumberValueDataNode.Create(s),
            uint i => NumberValueDataNode.Create(i),
            ulong l => NumberValueDataNode.Create(l),
            _ => throw new NotSupportedException("Only primitive types are supported")
        };
}