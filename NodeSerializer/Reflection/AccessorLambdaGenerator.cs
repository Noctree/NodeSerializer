using System.Linq.Expressions;
using System.Reflection;

namespace NodeSerializer.Reflection;

public static class AccessorLambdaGenerator
{
    private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                              System.Reflection.BindingFlags.Public |
                                              System.Reflection.BindingFlags.NonPublic;

    public static Func<object, object> CreateUntypedGetter(Type objectType, string propertyName)
    {
        var property  = objectType.GetRequiredProperty(propertyName, BindingFlags);
        return CreateUntypedGetter(objectType, property);
    }
    
    public static Func<object, object> CreateUntypedGetter(Type objectType, PropertyInfo property)
    {
        var getter = property.GetRequiredGetter();
        var parameter = Expression.Parameter(typeof(object), "instance");
        var body = Expression.Convert(
                                    Expression.Call(
                                        Expression.Convert(parameter, objectType),
                                        getter),
                                    typeof(object));
        return Expression.Lambda<Func<object, object>>(body, parameter).Compile();
    }

    public static Action<object, object> CreateUntypedSetter(Type objectType, string propertyName)
    {
        var prop = objectType.GetRequiredProperty(propertyName, BindingFlags);
        return CreateUntypedSetter(objectType, prop);
    }
    
    public static Action<object, object> CreateUntypedSetter(Type objectType, PropertyInfo property)
    {
        var setter = property.GetRequiredSetter(BindingFlags);
        
        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");
        var body = Expression.Call(
            Expression.Convert(instanceParam, objectType),
            setter,
            Expression.Convert(valueParam, property.PropertyType));
        return Expression.Lambda<Action<object, object>>(body, instanceParam, valueParam).Compile();
    }

    public static Func<TObject, TValue> CreateGetter<TObject, TValue>(string propertyName)
    {
        var getter = typeof(TObject).GetRequiredGetter(propertyName, BindingFlags);
        
        var instanceParam = Expression.Parameter(typeof(TObject), "instance");
        var body = Expression.Call(instanceParam, getter);
        return Expression.Lambda<Func<TObject, TValue>>(body, instanceParam).Compile();
    }
    
    public static Action<TObject, TValue> CreateSetter<TObject, TValue>(string propertyName)
    {
        var setter = typeof(TObject).GetRequiredSetter(propertyName, BindingFlags);
        
        var instanceParam = Expression.Parameter(typeof(TObject), "instance");
        var valueParam = Expression.Parameter(typeof(TValue), "value");
        var body = Expression.Call(instanceParam, setter, valueParam);
        return Expression.Lambda<Action<TObject, TValue>>(body, instanceParam, valueParam).Compile();
    }
}