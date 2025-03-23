namespace Jodot.Content.Libraries;

using Godot;
using System;
using System.Collections.Generic;

public partial class ResourceLibrary<C, T>  : IContentLibrary
	where T : Resource
	where C: Enum 
{
	private Dictionary<C, T> directory = new();
	private string _name;

	// Called when the node enters the scene tree for the first time.
	public ResourceLibrary(string name, string folder, string root="res://scenes/content")
	{
		_name = name;

		for (int i = 1; i < Enum.GetValues(typeof(C)).Length; i++) { // Skipping 0 as this is null
			C code = (C)(object)i; // Will fail if cast fails
			T res = ResourceLoader.Load<T>($"{root}/{folder}/{code}.tres");
			directory.Add(code, res as T);
		}
	}

	public ResourceLibrary() {} // For mocks

	public dynamic Get(int i) {
		return Get((C)(object)i);
	}

	public T Get(C code) {
		if (!directory.ContainsKey(code)) {
			GD.PushError($"Missing key in  {_name} prototype library {code.ToString()}");
			return null;
		}

		return directory[code];
	}

	public List<string> List() {
		return directory.Keys.Select(c => c.ToString()).ToList();
	}

}
