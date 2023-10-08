using System.Reflection;

namespace NodeSerializer.Reflection;

public static class TypeMemberGetExtensions
{
    public static PropertyInfo GetRequiredProperty(this Type objectType, string propertyName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        var prop = objectType.GetProperty(propertyName, bindingFlags);
        return prop ?? throw new MissingMemberException(objectType.Name, propertyName);
    }

    public static MethodInfo GetRequiredGetter(this Type objectType, string propertyName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        var prop = GetRequiredProperty(objectType, propertyName, bindingFlags);
        return GetRequiredGetter(prop, bindingFlags);
    }
    
    public static MethodInfo GetRequiredGetter(this PropertyInfo property, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) =>
        property.GetGetMethod((bindingFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
            ?? throw new MissingMemberException(property.DeclaringType?.Name, property.Name);
    
    public static MethodInfo GetRequiredSetter(this Type objectType, string propertyName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        var prop = GetRequiredProperty(objectType, propertyName, bindingFlags);
        return GetRequiredSetter(prop, bindingFlags);
    }
    
    public static MethodInfo GetRequiredSetter(this PropertyInfo property, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) =>
        property.GetSetMethod((bindingFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
        ?? throw new MissingMemberException(property.DeclaringType?.Name, property.Name);

    public static MethodInfo GetRequiredMethod(this Type type, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) =>
        type.GetMethod(methodName, bindingFlags)
        ?? throw new MissingMemberException(type.Name, methodName);
}