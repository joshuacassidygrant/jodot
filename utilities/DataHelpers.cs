namespace Jodot.Utilities;

using Godot;
using Jodot.Serialization;
using System.Linq;

public static class DataHelpers
{
	
	public static Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> SerializeArray(ISerializable[] serializables) {
		Godot.Collections.Dictionary<string, Variant>[] dicts = serializables.Select(s => s.ExportData()).ToArray();
		return new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>(dicts);
	}


	public static ISerializable[] DeserializeArray(Variant data) {
		Godot.Collections.Array<Variant> av = (Godot.Collections.Array<Variant>)data;
		return av.Cast<ISerializable>().ToArray();
	}

	public static Vector3 VariantToVector3(Variant vInput) {
		string input = (string)vInput;
		// remove parens
		input = input[1..^1];
		string[] arr = input.Split(",");
		return new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
	}

	public static Vector2 VariantToVector2(Variant vInput) {
		string input = (string)vInput;
		// remove parens
		input = input[1..^1];
		string[] arr = input.Split(",");
		return new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
	}

	public static Vector2I VariantToVector2I(Variant vInput) {
		string input = (string)vInput;
		// remove parens
		input = input[1..^1];
		string[] arr = input.Split(",");
		return new Vector2I(int.Parse(arr[0]), int.Parse(arr[1]));
	}

}
