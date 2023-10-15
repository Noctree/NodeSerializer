namespace NodeSerializer.Nodes;

public partial class NumberDataNode
{
    public static NumberDataNode Create(byte value) => Create(value, null!);
    public static NumberDataNode Create(byte value, string name) => new(value, name, null);
    public static NumberDataNode Create(sbyte value) => Create(value, null!);
    public static NumberDataNode Create(sbyte value, string name) => new(value, name, null);
    
    public static NumberDataNode Create(short value) => Create(value, null!);
    public static NumberDataNode Create(short value, string name) => new(value, name, null);
    
    public static NumberDataNode Create(ushort value) => Create(value, null!);
    public static NumberDataNode Create(ushort value, string name) => new(value, name, null);
    public static NumberDataNode Create(int value) => Create(value, null!);

    public static NumberDataNode Create(int value, string name) => new(value, name, null);
    public static NumberDataNode Create(long value) => Create(value, null!);

    public static NumberDataNode Create(long value, string name) => new(value, name, null);
    public static NumberDataNode Create(ulong value) => Create(value, null!);

    public static NumberDataNode Create(ulong value, string name) => new(value, name, null);
    public static NumberDataNode Create(decimal value) => Create(value, null!);

    public static NumberDataNode Create(decimal value, string name) => new(value, name, null);
    public static NumberDataNode Create(double value) => Create(value, null!);

    public static NumberDataNode Create(double value, string name) => new(value, name, null);
}