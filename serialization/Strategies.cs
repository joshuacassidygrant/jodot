namespace Jodot.Serialization;

using Godot;
using Jodot.Model;
using Jodot.Utilities;
using System;

public static class SerializationStrategies {

    public static Variant Serialize(object o, SerializationStrategy strategy) {
        switch (strategy) {
			case SerializationStrategy.INT:
				return (int)o;
			case SerializationStrategy.STRING:
				return (string)o;
			case SerializationStrategy.ENUM:
				return (int)o;
			case SerializationStrategy.BOOLEAN:
				return (Boolean)o;
			case SerializationStrategy.FLOAT:
				return (float)o;
			case SerializationStrategy.V2I:
				return (Vector2I)o;
			case SerializationStrategy.V3:
				return (Vector3)o;
			case SerializationStrategy.INT_STAT_MODIFIER:
				LocalModelData id = (LocalModelData)o;
				return id.ExportData();
			case SerializationStrategy.FLOAT_STAT_MODIFIER:
				LocalModelData fd = (LocalModelData)o;
				return fd.ExportData();
			case SerializationStrategy.LOCAL_DATA:
				LocalModelData rd = (LocalModelData)o;
				return rd.ExportData();
			default:
				return 0;
		}
    }

    public static dynamic Deserialize(Variant o, Type type, SerializationStrategy strategy) {
		switch (strategy) {
			case SerializationStrategy.INT:
				return (int)o;
			case SerializationStrategy.STRING:
				return (string)o;
			case SerializationStrategy.BOOLEAN:
				return (Boolean)o;
			case SerializationStrategy.FLOAT:
				return (float)o;
			case SerializationStrategy.ENUM:
				return Enum.ToObject(type, (int)o);
			case SerializationStrategy.V2I:
				return DataHelpers.VariantToVector2I(o);
			case SerializationStrategy.V3:
				return DataHelpers.VariantToVector3(o);
			case SerializationStrategy.INT_STAT_MODIFIER:
				ModelIntStatModifier id = new();
				Godot.Collections.Dictionary<string, Variant> idata = (Godot.Collections.Dictionary<string, Variant>)o;
				id.ImportData(idata);
				return id;
			case SerializationStrategy.FLOAT_STAT_MODIFIER:
				ModelFloatStatModifier fd = new();
				Godot.Collections.Dictionary<string, Variant> fdata = (Godot.Collections.Dictionary<string, Variant>)o;
				fd.ImportData(fdata);
				return fd;
			case SerializationStrategy.LOCAL_DATA:
				LocalModelData mr = new();
				Godot.Collections.Dictionary<string, Variant> rdata = (Godot.Collections.Dictionary<string, Variant>)o;
				mr.ImportData(rdata);
				return mr;
			default:
				return o;
			
		}
	}

}

public enum SerializationStrategy {
	BOOLEAN,
	INT,
	FLOAT,
	STRING,
	V2I,
	V3,
	ENUM,
	LOCAL_DATA,
	INT_STAT_MODIFIER,
	FLOAT_STAT_MODIFIER,
	
}