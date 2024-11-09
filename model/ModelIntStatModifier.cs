namespace Jodot.Model;

using Godot;
using Godot.Collections;
using Jodot.Serialization;
using System;

public partial class ModelIntStatModifier: LocalModelData, IModelStatData
{
  public IntStatCode StatCode;
  public int Value;

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
    StatCode = (IntStatCode)(int)data["StatCode"];
    Value = (int)data["Value"];
  } catch (Exception e) {
    GD.PrintErr(e);
  }
  }

  public ModelIntStatModifier() { }

  public ModelIntStatModifier(Godot.Collections.Dictionary<string, Variant> data) {
    ImportData(data);
  }
}