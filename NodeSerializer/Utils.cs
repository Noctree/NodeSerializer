using System.Globalization;

namespace NodeSerializer;

public static class Utils
{
    internal static readonly object SHARED_DUMMY_OBJECT_VALUE = (object)0;
    internal static readonly string DECIMAL_MAX = decimal.MaxValue.ToString(CultureInfo.InvariantCulture);
    internal static readonly string DECIMAL_MIN = decimal.MinValue.ToString(CultureInfo.InvariantCulture);
    public static int CompareNumbersAsString(string a, string b)
    {
        if (a.Length != b.Length)
        {
            return a.Length.CompareTo(b.Length);
        }
        
        if (a[0] == '-' && b[0] != '-')
        {
            return -1;
        }
        if (a[0] != '-' && b[0] == '-')
        {
            return 1;
        }
        
        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                return a[i].CompareTo(b[i]);
            }
        }
        return 0;
    }
}