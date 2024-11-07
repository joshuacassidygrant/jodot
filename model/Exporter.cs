namespace Jodot.Model;

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Godot;
using Jodot.Serialization;

public static class Exporter {

	public static Godot.Collections.Dictionary<string, Variant> ExportComponent(this Component component) {
		Godot.Collections.Dictionary<string, Variant> data = component.Export();
		data.Add("ComponentType", component.ComponentType);

		return data;
	}

   public static Godot.Collections.Dictionary<string, Variant> Export(this ISerializable serializable) {
		Godot.Collections.Dictionary<string, Variant> data = new();

        try {
			foreach (FieldInfo fieldInfo in serializable.GetFieldsOfType<ModelProperty>()) {
				ModelProperty mp = fieldInfo.GetCustomAttribute<ModelProperty>(true);
				data.Add(fieldInfo.Name, mp.Serialize(fieldInfo.GetValue(serializable), fieldInfo.FieldType));
			}

			foreach (FieldInfo fieldInfo in serializable.GetFieldsOfType<ModelPropertyCollectionBase>()) {
				ModelPropertyCollectionBase mp = fieldInfo.GetCustomAttribute<ModelPropertyCollectionBase>(true);
				IEnumerable val = fieldInfo.GetValue(serializable) as IEnumerable;
				mp.SerializeTo(val, fieldInfo.Name, ref data);
			}
		} catch (Exception e) {
			GD.PrintErr("Export error\n" + e);
		}

		return data;
   }

	public static FieldInfo[] GetFieldsOfType<T>(this ISerializable serializable) {
		return serializable.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(p => p.GetCustomAttributes(typeof(T), true).Any()).ToArray();
	}

	public static FieldInfo GetFieldOfName(this ISerializable serializable, string name) {
		return serializable.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

}