using System.Reflection;

namespace NodeSerializer.Extensions;

public static class ReflectionExtensions
{
    public static bool CanBeNull(this Type type) =>
        type.IsClass || type.IsInterface || Nullable.GetUnderlyingType(type) is not null;
}