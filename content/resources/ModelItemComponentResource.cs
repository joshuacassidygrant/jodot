namespace Jodot.Content.Resources;

using Godot;
using Jodot.Injection;
using Jodot.Model;

[GlobalClass]
public partial class ModelItemComponentResource : Resource
{
	[Export]
    public string ReferenceName;

    [Export]
    public bool DefaultActive = true;

    public virtual Component GenerateComponent(IServiceContext serviceDirectory) {
        return null;
    }

    public virtual bool Validate(Vector2I placement, ModelItemPrototype proto, Model model) {
        return true;
    }

}
