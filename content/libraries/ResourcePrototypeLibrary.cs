namespace Jodot.Content.Libraries;

using Godot;
using Jodot.Content.Resources;
using System;
using System.Collections.Generic;

public partial class ResourcePrototypeLibrary<C, T>  : Node,  IContentLibrary
	where T : ModelItemPrototype
	where C: Enum 
{
	private Dictionary<C, T> directory = new();

	// Called when the node enters the scene tree for the first time.
	public ResourcePrototypeLibrary(string name, string folder, string root="res://scenes/content")
	{
		Name = name;

		for (int i = 1; i < Enum.GetValues(typeof(C)).Length; i++) { // Skipping 0 as this is null
			C code = (C)(object)i; // Will fail if cast fails
			PackedScene scene = ResourceLoader.Load<PackedScene>($"{root}/{folder}/{code}.tscn");
			ModelItemPrototype proto = scene.Instantiate<ModelItemPrototype>();
			directory.Add(code, proto as T);
			AddChild(proto);
			proto.Initialize();
		}
	}

	public dynamic Get(int i) {
		return Get((C)(object)i);
	}

	public T Get(C code) {
		if (!directory.ContainsKey(code)) {
			GD.PushError("Missing key in prototype library " + code.ToString());
			return null;
		}

		return directory[code];
	}
}
