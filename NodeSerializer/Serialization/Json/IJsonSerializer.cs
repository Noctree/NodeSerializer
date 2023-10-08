using NodeSerializer.Nodes;

namespace NodeSerializer.Serialization.Json;

public interface IJsonSerializer : ISerializer
{
    public DataNode Deserialize(byte[] raw);
    public byte[] SerializeToBytes(DataNode data);
}