using NodeSerializer.Nodes;

namespace NodeSerializer.Serialization;

public interface ISerializer
{
    public DataNode Deserialize(string raw);
    public string Serialize(DataNode data);
}