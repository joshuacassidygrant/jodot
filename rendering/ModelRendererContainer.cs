namespace Jodot.Rendering;

using Godot;
using System.Collections.Generic;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;

public partial class ModelRendererContainer : Node3D
{
	public Dictionary<int, ModelItemRenderer> Renderers = new Dictionary<int, ModelItemRenderer>();

	[Inject("Events")] public IEventBus _events;

	public ModelItemRenderer AddRenderer(int modelItemIndex, Model model) {
		ModelItemRenderer renderer = new();
		AddChild(renderer);
		renderer.Visible = true;
		renderer.BindModelItem(modelItemIndex, GenerateComponent, _events, model);
		Renderers.Add(modelItemIndex, renderer);
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

		Renderers = new Dictionary<int, ModelItemRenderer>();
	}

	public void GenerateRenderers(Model model)
	{
		// TODO: use comopnents
		/*foreach (ModelItem item in model.ModelItems)
		{
			if (item != null)
			{
				AddRenderer(item);
			}
		}*/
	}
}
