namespace Jodot.Attributes.Linked;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LinkedModelItem: System.Attribute
{
	public string ModelIndexField;

	public LinkedModelItem(string modelIndexField) {
		ModelIndexField = modelIndexField;
	}
}