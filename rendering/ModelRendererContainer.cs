namespace Jodot.Rendering;

using Godot;
using System.Collections.Generic;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;

public partial class ModelRendererContainer : Node3D
{
	public Dictionary<ModelItem, ModelItemRenderer> Renderers = new Dictionary<ModelItem, ModelItemRenderer>();

	[Inject("Events")] public IEventBus _events;

	public ModelItemRenderer AddRenderer(ModelItem item)
	{
		if (item == null) return null;
		ModelItemRenderer renderer = new ModelItemRenderer();
		AddChild(renderer);
		renderer.Visible = true;
		renderer.BindModelItem(item, GenerateComponent, _events);
		Renderers.Add(item, renderer);
		return renderer;
	}

	public virtual ComponentRenderer GenerateComponent(int type) {
		return new ComponentRenderer();
	}

	public void ClearRenderers()
	{
		foreach (ModelItemRenderer renderer in Renderers.Values)
		{
			if (renderer != null && IsInstanceValid(renderer))
			{
				renderer.QueueFree();
			}
		}

		Renderers = new Dictionary<ModelItem, ModelItemRenderer>();
	}

	public void GenerateRenderers(Model model)
	{
		foreach (ModelItem item in model.ModelItems)
		{
			if (item != null)
			{
				AddRenderer(item);
			}
		}
	}
}
