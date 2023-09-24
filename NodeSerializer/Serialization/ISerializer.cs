using NodeSerializer.Nodes;

namespace NodeSerializer.Serialization;

public interface ISerializedLoader
{
    public DataNode Deserialize(string raw);
    public string Serialize(DataNode data);
}