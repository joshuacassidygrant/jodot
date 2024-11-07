namespace Jodot.Model;

using System;
using System.Reflection;
using Godot;
using Jodot.Serialization;

public static class Importer {

   public static Godot.Collections.Dictionary<string, Variant> Import(this ISerializable serializable) {
		Godot.Collections.Dictionary<string, Variant> data = new();

        try {
			foreach (FieldInfo fieldInfo in serializable.GetFieldsOfType<ModelProperty>()) {
				ModelProperty mp = fieldInfo.GetCustomAttribute<ModelProperty>(true);
				fieldInfo.SetValue(serializable, mp.Deserialize(data[fieldInfo.Name], fieldInfo.FieldType));
			}

			foreach (FieldInfo fieldInfo in serializable.GetFieldsOfType<ModelPropertyCollectionBase>()) {
				ModelPropertyCollectionBase mp = fieldInfo.GetCustomAttribute<ModelPropertyCollectionBase>(true);
				mp.DeserializeFrom(data[fieldInfo.Name], serializable, fieldInfo);
			}
		} catch (Exception e) {
			GD.PrintErr("Export error\n" + e);
		}

		return data;
   }

	public static FieldInfo GetFieldOfName(this ISerializable serializable, string name) {
		return serializable.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

}