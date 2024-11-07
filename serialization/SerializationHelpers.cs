namespace Jodot.Serialization;

using System.Linq;
using System.Reflection;

public static class SerializationHelpers {
	public static FieldInfo[] GetFieldsOfType<T>(this ISerializable serializable) {
		return serializable.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(p => p.GetCustomAttributes(typeof(T), true).Any()).ToArray();
	}

	public static FieldInfo GetFieldOfName(this ISerializable serializable, string name) {
		return serializable.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

}