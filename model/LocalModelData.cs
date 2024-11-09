namespace Jodot.Serialization;
using Godot;

public class LocalModelData: ISerializable {
    public virtual Godot.Collections.Dictionary<string, Variant> ExportData() {
        return new Godot.Collections.Dictionary<string, Variant>();
    }
    public virtual void ImportData(Godot.Collections.Dictionary<string, Variant> data) {}
}