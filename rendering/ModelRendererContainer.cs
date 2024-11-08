namespace Jodot.Rendering;

using Godot;
using System.Collections.Generic;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;
using System.Linq;

public partial class ModelRendererContainer : Node3D, IInjectSubject
{
	public Dictionary<int, ModelItemRenderer> Renderers = new Dictionary<int, ModelItemRenderer>();

	public Model Model;

	[Inject("Events")] public IEventBus _events;
	[Inject("ModelRunner")] public ModelRunner _modelRunner;
	
	public ModelItemRenderer AddRenderer(int modelItemIndex, Model model, ILocationProvider locationProvider) {
		ModelItemRenderer renderer = new();
		AddChild(renderer);
		renderer.Visible = true;
		renderer.BindModelItem(modelItemIndex, GenerateComponent, _events, model, locationProvider);
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
		for (int i = 1; i < model.NextEntityPointer; i++)
		{
			if (!model.FreedEntities.Contains(i))
			{
				ILocationProvider locationProvider = (ILocationProvider)model.GetComponentsBoundToEntity(i, typeof(ILocationProvider)).FirstOrDefault();
				AddRenderer(i, model, locationProvider);
			}
		}
	}

    public void OnPostInject()
    {
        _events.ConnectTo("RequestGenerateAllRenderers", Callable.From(() => {
			GenerateRenderers(_modelRunner.Model);
		}));

        _events.ConnectTo("RequestDestroyAllRenderers", Callable.From(() => {
			ClearRenderers();
		}));

		_events.ConnectTo("OnComponentFreed", Callable.From((int componentIndex) => {
			Component component = _modelRunner.Model.Components[componentIndex];
			if (component == null) return;
			int entityIndex = component.EntityIndex;

			if (!Renderers.ContainsKey(entityIndex)) return;
			ModelItemRenderer renderer = Renderers[entityIndex];

			renderer.FreeComponentRenderer(component.ComponentType);

		}));


		_events.ConnectTo("OnEntityFreed", Callable.From((int entityIndex) => {
			if (!Renderers.ContainsKey(entityIndex)) return;
			ModelItemRenderer renderer = Renderers[entityIndex];

			renderer.FreeRenderer();

			Renderers.Remove(entityIndex);

		}));

    }
}
