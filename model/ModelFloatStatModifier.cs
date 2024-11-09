namespace Jodot.Model;

using Godot;
using Godot.Collections;
using Jodot.Serialization;
using System;

public partial class ModelFloatStatModifier: LocalModelData, IModelStatData
{
    public int StatCode;
    public float Value;

    public override Dictionary<string, Variant> ExportData()
    {
        return new Dictionary<string, Variant>
        {
			{"StatCode", (int)StatCode},
			{"Value", Value},
		};
    }

    public override void ImportData(Dictionary<string, Variant> data)
    {
        try {
			StatCode = (int)data["StatCode"];
			Value = (float)data["Value"];
		} catch (Exception e) {
			GD.PrintErr(e);
		}
    }

    public ModelFloatStatModifier() { }

    public ModelFloatStatModifier(Godot.Collections.Dictionary<string, Variant> data) {
		ImportData(data);
	}
}