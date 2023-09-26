﻿using System.Globalization;
using System.Runtime.InteropServices;

namespace NodeSerializer.Nodes;

public class NumberValueDataNode : TypedValueDataNode<NumericUnion>
{
    public NumberValueDataNode(NumericUnion value, string? name, DataNode? parent) : base(value, name, parent)
    {
    }
}

/// <summary>
/// A union of different numeric types, for internal use only
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct NumericUnion : IFormattable
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

    public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider) => Range switch
    {
        NumberRange.PositiveInteger => ULongValue.ToString(format, formatProvider),
        NumberRange.NegativeInteger => LongValue.ToString(format, formatProvider),
        NumberRange.SmallDecimal => DecimalValue.ToString(format, formatProvider),
        NumberRange.BigDecimal => DoubleValue.ToString(format, formatProvider),
        _ => nameof(NumberValueDataNode)
    };

    public static implicit operator NumericUnion(long value) => Create(value);
    public static implicit operator NumericUnion(ulong value) => Create(value);
    public static implicit operator NumericUnion(decimal value) => Create(value);
    public static implicit operator NumericUnion(double value) => Create(value);
    
    public static implicit operator long(NumericUnion value) => value.Range == NumberRange.NegativeInteger
        ? value.LongValue
        : (long)value.ToObject();
    public static implicit operator ulong(NumericUnion value) => value.Range == NumberRange.PositiveInteger
        ? value.ULongValue
        : (ulong)value.ToObject();
    public static implicit operator decimal(NumericUnion value) => value.Range == NumberRange.SmallDecimal
        ? value.DecimalValue
        : (decimal)value.ToObject();
    public static implicit operator double(NumericUnion value) => value.Range == NumberRange.BigDecimal
        ? value.DoubleValue
        : (double)value.ToObject();
}