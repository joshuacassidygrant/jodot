namespace Jodot.Content.Resources;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ModelItemResource: Resource, IContentModelItemResource
{

	public virtual string GetName() {
		return "RESOURCE";
	}

	[Export]
	public ModelItemComponentResource[] Components = Array.Empty<ModelItemComponentResource>();

	public List<ModelItemComponentResource> GetComponentResources()
    {
        return Components.ToList();
    }
}