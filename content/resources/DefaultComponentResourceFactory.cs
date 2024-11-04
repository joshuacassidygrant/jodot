namespace Jodot.Content.Resources;

using Godot;
using Jodot.Model;
using System;

public partial class DefaultComponentResourceFactory
{
	public IModelInfo ModelInfo;
	public ModelItemComponentResource[] DefaultResources;

	public DefaultComponentResourceFactory(IModelInfo modelInfo, string root = "res://scenes/content/component-defaults") {
		ModelInfo = modelInfo;
		DefaultResources = new ModelItemComponentResource[ModelInfo.ComponentTypeCount];
		for (int i = 1; i < ModelInfo.ComponentTypeCount; i++) { // Skipping 0 as this is null
			string type = ModelInfo.ComponentTypeNames[i];
			string path = $"{root}/{type}.tres";
			if (ResourceLoader.Exists(path)) {
				ModelItemComponentResource res = ResourceLoader.Load<ModelItemComponentResource>(path);
				DefaultResources[i] = res;
			}
		}
	}


	public virtual ModelItemComponentResource GetDefaultResource(int type) {
	 	ModelItemComponentResource resource = DefaultResources[type];
		if (resource == null) {
			GD.PrintErr($"Tried to access default component for {type}");
		}
		return resource;		
	}

}
