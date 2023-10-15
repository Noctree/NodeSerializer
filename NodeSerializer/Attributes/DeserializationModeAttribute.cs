using System.ComponentModel;

namespace NodeSerializer.Attributes;

public enum DeserializationMode
{
    /// <summary>
    /// Deserializes the object by creating it trough the constructor<br/>
    /// This assumes that all necessary data is passed in the constructor
    /// </summary>
    Constructor,
    /// <summary>
    /// Deserializes the object by first creating an instance trough a parameterless constructor (can be private)<br/>
    /// Then fills in the data by calling the property setters (can be private as well)
    /// </summary>
    Properties
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class DeserializationModeAttribute : Attribute
{
    public DeserializationMode DeserializationMode { get; set; }
    
    public DeserializationModeAttribute(DeserializationMode mode)
    {
        DeserializationMode = mode;
    }
}