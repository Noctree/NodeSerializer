using System.Globalization;

namespace NodeSerializer.Serialization;

public static class PrimitiveSerializer
{
    public static string Serialize<T>(T value) => value?.ToString() ?? string.Empty;

    public static object Deserialize(string raw, Type targetType)
    {
        return Convert.ChangeType(raw, targetType, CultureInfo.InvariantCulture);
    }
}