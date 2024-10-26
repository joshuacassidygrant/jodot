namespace Jodot.Attributes.Injection;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class Inject: System.Attribute {
    public string Name;

    public Inject(string name) {
        Name = name;
    }
}