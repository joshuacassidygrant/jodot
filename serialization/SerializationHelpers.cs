namespace Jodot.Serialization;

using System;
using System.Linq;
using System.Reflection;
using Godot;
using Jodot.Content.Libraries;
using Jodot.Injection;

public static class SerializationHelpers {
	public static FieldInfo[] GetFieldsOfType<T>(this ISerializable serializable) {
		return serializable.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(p => p.GetCustomAttributes(typeof(T), true).Any()).ToArray();
	}

	public static FieldInfo GetFieldOfName(this ISerializable serializable, string name) {
		return serializable.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

	public static void ResolveBoundResources(this ISerializable serializable, IServiceContext s) {
		// Resolve bound resources
		foreach (FieldInfo fieldInfo in serializable.GetFieldsOfType<LinkedResource>()) {
			LinkedResource mp = fieldInfo.GetCustomAttribute<LinkedResource>(true);
			int index = (int)serializable.GetFieldOfName(mp.ResourceKeyField).GetValue(serializable);
			IContentLibrary library = s.GetService(mp.ResourceLibraryKey);
			fieldInfo.SetValue(serializable, library.Get(index));	
		}
	}
}