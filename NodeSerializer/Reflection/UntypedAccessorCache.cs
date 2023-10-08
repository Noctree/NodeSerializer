using System.Reflection;

namespace NodeSerializer.Reflection;

public class UntypedAccessorCache
{
    private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                              System.Reflection.BindingFlags.Public |
                                              System.Reflection.BindingFlags.NonPublic;
    private static readonly Dictionary<Type, Dictionary<string, Func<object, object>>> InternalGetterCache = new();
    private static readonly Dictionary<Type, Dictionary<string, Action<object, object>>> InternalSetterCache = new();

    public static IReadOnlyDictionary<string, Func<object, object>> GetPropertyGettersFor(Type objectType)
    {
        if (InternalGetterCache.TryGetValue(objectType, out var getters))
            return getters;
        
        var properties = objectType.GetProperties(BindingFlags);
        getters = new(properties.Length);
        foreach (var prop in properties)
        {
            var getter = AccessorLambdaGenerator.CreateUntypedGetter(objectType, prop.Name);
            getters.Add(prop.Name, getter);
        }
        InternalGetterCache.Add(objectType, getters);
        return getters;
    }
    
    public static IReadOnlyDictionary<string, Action<object, object>> GetPropertySettersFor(Type objectType)
    {
        if (InternalSetterCache.TryGetValue(objectType, out var setters))
            return setters;
        
        var properties = objectType.GetProperties(BindingFlags);
        setters = new(properties.Length);
        foreach (var prop in properties)
        {
            var setter = AccessorLambdaGenerator.CreateUntypedSetter(objectType, prop.Name);
            setters.Add(prop.Name, setter);
        }
        InternalSetterCache.Add(objectType, setters);
        return setters;
    }
    
    public static Func<object, object> GetGetter(Type objectType, string name)
    {
        Func<object, object> getter;
        if (InternalGetterCache.TryGetValue(objectType, out var propertyCache))
        {
            if (propertyCache.TryGetValue(name, out getter))
                return getter;
            getter = AccessorLambdaGenerator.CreateUntypedGetter(objectType, name);
            propertyCache.Add(name, getter);
            return getter;
        }

        propertyCache = new(1);
        getter = AccessorLambdaGenerator.CreateUntypedGetter(objectType, name);
        propertyCache.Add(name, getter);
        InternalGetterCache.Add(objectType, propertyCache);
        return getter;
    }
    
    public static Action<object, object> GetSetter(Type objectType, string name)
    {
        Action<object, object> setter;
        if (InternalSetterCache.TryGetValue(objectType, out var propertyCache))
        {
            if (propertyCache.TryGetValue(name, out setter))
                return setter;
            setter = AccessorLambdaGenerator.CreateUntypedSetter(objectType, name);
            propertyCache.Add(name, setter);
            return setter;
        }
        
        propertyCache = new(1);
        setter = AccessorLambdaGenerator.CreateUntypedSetter(objectType, name);
        propertyCache.Add(name, setter);
        InternalSetterCache.Add(objectType, propertyCache);
        return setter;
    }
}