namespace Jodot.Content.Resources;

using Godot;
using System.Collections.Generic;

public partial class ModelItemPrototype: Node, IContentModelItemResource
{	
	[Export]
	public ModelItemComponentResource[] Components = [];

	public string GetName() {
		return Name;
	}

	public Node3D Renderer => GetNode<Node3D>("renderer");
	public Node3D Collider => GetNode<Node3D>("collider");

	public virtual void Initialize() {
		if (Renderer != null) {
			Renderer.Visible = false;
		}
	}

    public List<ModelItemComponentResource> GetComponentResources()
    {
        return [.. Components];
    }

}