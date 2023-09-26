using NodeSerializer.Nodes;

namespace NodeSerializer.Serialization;

public interface IJsonSerializer : ISerializer
{
    public DataNode DeserializeFromBytes(byte[] raw);
    public byte[] SerializeToBytes(DataNode data);
}