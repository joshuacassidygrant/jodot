namespace Jodot.Serialization;

public interface ISerializable {
    public Godot.Collections.Dictionary<string, Godot.Variant> ExportData();
    public void ImportData( Godot.Collections.Dictionary<string, Godot.Variant> data);
}