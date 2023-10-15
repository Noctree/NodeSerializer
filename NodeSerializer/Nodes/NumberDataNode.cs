using System.Globalization;
using System.Runtime.InteropServices;

namespace NodeSerializer.Nodes;

public partial class NumberDataNode : TypedValueDataNode<NumericUnion>
{
    internal NumberDataNode(byte value, string? name, DataNode? parent) : base((ulong)value, typeof(byte), name, parent)
    {
    }
    
    internal NumberDataNode(sbyte value, string? name, DataNode? parent) : base(value, typeof(sbyte), name, parent)
    {
    }
    
    internal NumberDataNode(short value, string? name, DataNode? parent) : base(value, typeof(short), name, parent)
    {
    }
    
    internal NumberDataNode(ushort value, string? name, DataNode? parent) : base((ulong)value, typeof(ushort), name, parent)
    {
    }
    
    internal NumberDataNode(int value, string? name, DataNode? parent) : base(value, typeof(int), name, parent)
    {
    }
    
    internal NumberDataNode(long value, string? name, DataNode? parent) : base(value, typeof(long), name, parent)
    {
    }
    
    internal NumberDataNode(ulong value, string? name, DataNode? parent) : base(value, typeof(ulong), name, parent)
    {
    }
    
    internal NumberDataNode(decimal value, string? name, DataNode? parent) : base(value, typeof(decimal), name, parent)
    {
    }
    
    internal NumberDataNode(double value, string? name, DataNode? parent) : base(value, typeof(double), name, parent)
    {
    }

    private NumberDataNode(NumericUnion value, Type type) : base(value, type, null, null)
    {
    }

    public override DataNode Clone()
    {
        return new NumberDataNode(TypedValue, TypeOf!);
    }

    protected override string ToString(byte indent)
    {
        return Indent($"{TypeOf?.Name ?? "Number"}({Name}: {TypedValue})", indent);
    }
}

/// <summary>
/// A union of different numeric types, for internal use only
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct NumericUnion : IFormattable, IComparable<NumericUnion>
{
    public enum NumberRange : byte
    {
        PositiveInteger,
        NegativeInteger,
        SmallDecimal,
        BigDecimal
    }
    [FieldOffset(0)]
    public long LongValue;
    [FieldOffset(0)]
    public ulong ULongValue;
    [FieldOffset(0)]
    public decimal DecimalValue;
    [FieldOffset(0)]
    public double DoubleValue;
    [FieldOffset(16)]
    public NumberRange Range;
    
    public static NumericUnion Create(long value) => new() { LongValue = value, Range = NumberRange.NegativeInteger };
    public static NumericUnion Create(ulong value) => new() { ULongValue = value, Range = NumberRange.PositiveInteger };
    public static NumericUnion Create(decimal value) => new() { DecimalValue = value, Range = NumberRange.SmallDecimal };
    public static NumericUnion Create(double value) => new() { DoubleValue = value, Range = NumberRange.BigDecimal };
    
    public object ToObject() => Range switch
    {
        NumberRange.PositiveInteger => ULongValue,
        NumberRange.NegativeInteger => LongValue,
        NumberRange.SmallDecimal => DecimalValue,
        NumberRange.BigDecimal => DoubleValue,
        _ => throw new ArgumentOutOfRangeException($"Range {Range} is not supported")
    };
    
    public readonly int AsInt() => Range switch
    {
        NumberRange.PositiveInteger => (int)ULongValue,
        NumberRange.NegativeInteger => (int)LongValue,
        NumberRange.SmallDecimal => (int)DecimalValue,
        NumberRange.BigDecimal => (int)DoubleValue,
        _ => throw new ArgumentOutOfRangeException($"Range {Range} is not supported")
    };
    
    public readonly long AsLong() => Range switch
    {
        NumberRange.PositiveInteger => (long)ULongValue,
        NumberRange.NegativeInteger => LongValue,
        NumberRange.SmallDecimal => (long)DecimalValue,
        NumberRange.BigDecimal => (long)DoubleValue,
        _ => throw new ArgumentOutOfRangeException($"Range {Range} is not supported")
    };
    
    public readonly ulong AsULong() => Range switch
    {
        NumberRange.PositiveInteger => ULongValue,
        NumberRange.NegativeInteger => (ulong)LongValue,
        NumberRange.SmallDecimal => (ulong)DecimalValue,
        NumberRange.BigDecimal => (ulong)DoubleValue,
        _ => throw new ArgumentOutOfRangeException($"Range {Range} is not supported")
    };
    
    public readonly decimal AsDecimal() => Range switch
    {
        NumberRange.PositiveInteger => ULongValue,
        NumberRange.NegativeInteger => LongValue,
        NumberRange.SmallDecimal => DecimalValue,
        NumberRange.BigDecimal => (decimal)DoubleValue,
        _ => throw new ArgumentOutOfRangeException($"Range {Range} is not supported")
    };

    public readonly double AsDouble() => Range switch
    {
        NumberRange.PositiveInteger => ULongValue,
        NumberRange.NegativeInteger => LongValue,
        NumberRange.SmallDecimal => (double)DecimalValue,
        NumberRange.BigDecimal => DoubleValue,
        _ => throw new ArgumentOutOfRangeException($"Range {Range} is not supported")
    };
    public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider) => Range switch
    {
        NumberRange.PositiveInteger => ULongValue.ToString(format, formatProvider),
        NumberRange.NegativeInteger => LongValue.ToString(format, formatProvider),
        NumberRange.SmallDecimal => DecimalValue.ToString(format, formatProvider),
        NumberRange.BigDecimal => DoubleValue.ToString(format, formatProvider),
        _ => nameof(NumberDataNode)
    };
    
    public int CompareTo(NumericUnion other)
    {
        if (Range == NumberRange.BigDecimal || other.Range == NumberRange.BigDecimal)
        {
            // If either is a double, compare as doubles
            return AsDouble().CompareTo(other.AsDouble());
        }

        // For all other cases, compare as decimals for more precision
        return AsDecimal().CompareTo(other.AsDecimal());
    }

    public int CompareTo(long other)
    {
        return Range == NumberRange.BigDecimal
            ? AsDouble().CompareTo((double)other)
            : AsDecimal().CompareTo(other);
    }

    public int CompareTo(ulong other)
    {
        return Range == NumberRange.BigDecimal
            ? AsDouble().CompareTo((double)other)
            : AsDecimal().CompareTo(other);
    }

    public int CompareTo(decimal other)
    {
        return Range == NumberRange.BigDecimal
            ? AsDouble().CompareTo((double)other)
            : AsDecimal().CompareTo(other);
    }

    public int CompareTo(double other)
    {
        return AsDouble().CompareTo(other);
    }

    public static implicit operator NumericUnion(long value) => Create(value);
    public static implicit operator NumericUnion(ulong value) => Create(value);
    public static implicit operator NumericUnion(decimal value) => Create(value);
    public static implicit operator NumericUnion(double value) => Create(value);
    public static implicit operator NumericUnion(int value) => Create(value);

    public static implicit operator int(NumericUnion value) => value.AsInt();

    public static implicit operator long(NumericUnion value) => value.AsLong();
    public static implicit operator ulong(NumericUnion value) => value.AsULong();
    public static implicit operator decimal(NumericUnion value) => value.AsDecimal();
    public static implicit operator double(NumericUnion value) => value.AsDouble();
}