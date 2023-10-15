using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NodeSerializer.Extensions;

public static class ReflectionExtensions
{
    public static bool CanBeNull(this Type type) =>
        type.IsClass || type.IsInterface || Nullable.GetUnderlyingType(type) is not null;

    public static bool IsNullableValueType(this Type type) => Nullable.GetUnderlyingType(type) is not null;

    public static bool IsNullableValueType(this Type type, [NotNullWhen(true)] out Type? underlyingType)
    {
        underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType is not null;
    }

    public static Type? GetNullableUnderlyingType(this Type type) => Nullable.GetUnderlyingType(type);

    public static bool IsPrimitiveExtended(this Type type) =>
        type.IsPrimitive || type == typeof(string) || type == typeof(decimal);

    public static bool IsNullablePrimitiveExtended(this Type type) =>
        type == typeof(string)
        || (type.IsNullableValueType(out var underlyingType)
            && underlyingType.IsPrimitiveExtended());

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
    
    public static T GetRequiredAttribute<T>(this Type type) where T : Attribute => type.GetCustomAttribute<T>()
        ?? throw new MissingMemberException($"{type} does not have a required attribute of type {typeof(T)}");

    public static bool ShouldSerializeAsArray(this Type type) =>
        type.IsAssignableTo(typeof(IEnumerable)) || type.ImplementsInterface(typeof(IEnumerable<>));
}