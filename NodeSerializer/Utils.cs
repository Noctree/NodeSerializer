﻿using System.Globalization;

namespace NodeSerializer;

public static class Utils
{
    internal static readonly object SharedDummyObjectValue = (object)0;
    internal static readonly string DecimalMax = decimal.MaxValue.ToString(CultureInfo.InvariantCulture);
    internal static readonly string DecimalMin = decimal.MinValue.ToString(CultureInfo.InvariantCulture);
    public static int CompareNumbersAsString(ReadOnlySpan<byte> a, string b)
    {
        if (a[0] == '-' && b[0] != '-')
        {
            return -1;
        }
        if (a[0] != '-' && b[0] == '-')
        {
            return 1;
        }
        
        if (a.Length != b.Length)
        {
            return a.Length.CompareTo(b.Length);
        }
        
        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                return a[i].CompareTo((byte)b[i]);
            }
        }
        return 0;
    }

    public static UniqueName ToUniqueName(this string str) => new UniqueName(str);
}