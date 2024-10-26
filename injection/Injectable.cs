namespace Jodot.Injection;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class Injectable: System.Attribute {
    public string Name;

    public Injectable(string name) {
        Name = name;
    }
}