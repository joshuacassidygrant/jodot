namespace Jodot.Attributes.Properties;

using Godot;
using Jodot.Attributes.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Field)]
public abstract class ModelPropertyCollectionBase: System.Attribute {
	public abstract void SerializeTo(System.Collections.IEnumerable vals, string fieldName, ref Godot.Collections.Dictionary<string, Variant> data);
	public abstract void DeserializeFrom(Variant o, object owner, FieldInfo field);

}


[System.AttributeUsage(System.AttributeTargets.Field)]
public class ModelPropertyCollection<C, T>: ModelPropertyCollectionBase
	where C: System.Collections.IList, new()
{
	public SerializationStrategy Strategy;

	public ModelPropertyCollection(SerializationStrategy strat) {
		Strategy = strat;
	}
	public override void SerializeTo(System.Collections.IEnumerable vals, string fieldName, ref Godot.Collections.Dictionary<string, Variant> data) {
		Godot.Collections.Array<Variant> valsArray = new Godot.Collections.Array<Variant>();
		foreach (object val in vals) {
			valsArray.Add(SerializationStrategies.Serialize(val, Strategy));
		}
		data.Add(fieldName, valsArray);
	}

	public override void DeserializeFrom(Variant o, object owner, FieldInfo field) {
		Godot.Collections.Array<Variant> arr = (Godot.Collections.Array<Variant>) o;

		C collection = new();
		foreach (Variant val in arr) {
			collection.Add(SerializationStrategies.Deserialize(val, typeof(T), Strategy));
		}
		field.SetValue(owner, collection);
	}
}
