namespace Jodot.Attributes.Linked;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LinkedModelItemComponent: System.Attribute
{
	public string ModelIndexField;

	public LinkedModelItemComponent(string modelIndexField) {
		ModelIndexField = modelIndexField;
	}
}