namespace Jodot.Attributes.Properties;

using System;
using Godot;
using Jodot.Attributes.Serialization;


[System.AttributeUsage(System.AttributeTargets.Field)]
public class ModelProperty: System.Attribute
{
	public SerializationStrategy Strategy;

	public ModelProperty(SerializationStrategy strat) {
		Strategy = strat;
	}

	public Variant Serialize(object o, Type type) {
		return SerializationStrategies.Serialize(o, Strategy);
	}

	public dynamic Deserialize(Variant o, Type type) {
		return SerializationStrategies.Deserialize(o, type, Strategy);
	}
}




