namespace NodeSerializer.Nodes;

public partial class NumberValueDataNode
{
    public static NumberValueDataNode Create(byte value) => Create(value, null!);
    public static NumberValueDataNode Create(byte value, string name) => new(value, name, null);
    public static NumberValueDataNode Create(sbyte value) => Create(value, null!);
    public static NumberValueDataNode Create(sbyte value, string name) => new(value, name, null);
    
    public static NumberValueDataNode Create(short value) => Create(value, null!);
    public static NumberValueDataNode Create(short value, string name) => new(value, name, null);
    
    public static NumberValueDataNode Create(ushort value) => Create(value, null!);
    public static NumberValueDataNode Create(ushort value, string name) => new(value, name, null);
    public static NumberValueDataNode Create(int value) => Create(value, null!);

    public static NumberValueDataNode Create(int value, string name) => new(value, name, null);
    public static NumberValueDataNode Create(long value) => Create(value, null!);

    public static NumberValueDataNode Create(long value, string name) => new(value, name, null);
    public static NumberValueDataNode Create(ulong value) => Create(value, null!);

    public static NumberValueDataNode Create(ulong value, string name) => new(value, name, null);
    public static NumberValueDataNode Create(decimal value) => Create(value, null!);

    public static NumberValueDataNode Create(decimal value, string name) => new(value, name, null);
    public static NumberValueDataNode Create(double value) => Create(value, null!);

    public static NumberValueDataNode Create(double value, string name) => new(value, name, null);
}