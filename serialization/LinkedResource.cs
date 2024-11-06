namespace Jodot.Serialization;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LinkedResource: System.Attribute
{
    public string ResourceLibraryKey;
    public string ResourceKeyField;

    public LinkedResource(string resourceLibraryKey, string resourceKeyField) {
        ResourceLibraryKey = resourceLibraryKey;
        ResourceKeyField = resourceKeyField;
    }
    
}