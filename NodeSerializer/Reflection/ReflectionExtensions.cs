using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NodeSerializer.Extensions;

public static class ReflectionExtensions
{
    public static bool CanBeNull(this Type type) =>
        type.IsClass || type.IsInterface || Nullable.GetUnderlyingType(type) is not null;

    public static bool IsNullableValueType(this Type type) => Nullable.GetUnderlyingType(type) is not null;
    
    public static Type? GetNullableUnderlyingType(this Type type) => Nullable.GetUnderlyingType(type);
    
    public static bool IsPrimitiveExtended(this Type type) =>
        type.IsPrimitive || type == typeof(string) || type == typeof(decimal);

    public static bool ImplementsInterface(this Type type, Type interfaceType) =>
        ImplementsInterface(type, interfaceType, out _);

    public static bool ImplementsInterface(this Type type, Type interfaceType, [NotNullWhen(true)] out Type? genericInterface)
    {
        if (interfaceType is { IsGenericType: true, ContainsGenericParameters: true })
        {
            var interfaces = type.GetInterfaces();
            genericInterface = Array.Find(interfaces, i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
            return genericInterface is not null;
        }

        genericInterface = null;
        return type.IsAssignableTo(interfaceType);
    }
}