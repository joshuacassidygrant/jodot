namespace Jodot.Serialization;
using Godot;

public abstract class LocalModelData: ISerializable {
    public abstract Godot.Collections.Dictionary<string, Variant> ExportData();
    public abstract void ImportData(Godot.Collections.Dictionary<string, Variant> data); 
}