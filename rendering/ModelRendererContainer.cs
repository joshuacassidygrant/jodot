namespace Jodot.Rendering;

using Godot;
using System.Collections.Generic;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;
using System.Linq;

public partial class ModelRendererContainer : Node, IInjectSubject
{
	public Dictionary<int, IEntityRenderer> Renderers = new Dictionary<int, IEntityRenderer>();
	public Dictionary<int, IComponentRenderer> ComponentRenderers = new Dictionary<int, IComponentRenderer>();
	public virtual bool Is3D => true;

	public Model Model;

	[Inject("Events")] public IEventBus _events;
	[Inject("ModelRunner")] public ModelRunner _modelRunner;
	
	public IEntityRenderer AddRenderer(int modelItemIndex, Model model, ILocationProvider locationProvider) {
		if (Is3D) {
			EntityRenderer renderer = new EntityRenderer();
			AddChild(renderer);
			renderer.Visible = true;
			renderer.BindModelItem(modelItemIndex, GenerateComponent, _events, model, locationProvider, this);
			Renderers.Add(modelItemIndex, renderer);
			return renderer;
		} else {
			EntityRenderer2D renderer2D = new();
			AddChild(renderer2D);
			renderer2D.Visible = true;
			renderer2D.BindModelItem(modelItemIndex, GenerateComponent2D, _events, model, locationProvider, this);
			Renderers.Add(modelItemIndex, renderer2D);
			return renderer2D;
		}		
	}

	public void RemoveRenderer(int modelItemIndex) {
		IEntityRenderer renderer = GetEntityRenderer(modelItemIndex);
		if (renderer == null) return;
		renderer.FreeRenderer();
		foreach (int componentIndex in _modelRunner.Model.ComponentsByEntity[modelItemIndex]) {
			ComponentRenderers.Remove(componentIndex);
		}
		Renderers.Remove(modelItemIndex);
	}

	public IEntityRenderer GetEntityRenderer(int key) {
		if (!Renderers.ContainsKey(key)) return null;

		return Renderers[key];
	}
	 
	public IComponentRenderer GetComponentRenderer(int key) {
		if (!ComponentRenderers.ContainsKey(key)) return null;

		return ComponentRenderers[key];
	} 

	public virtual ComponentRenderer GenerateComponent(int type) {
		return new ComponentRenderer();
	}

	public virtual ComponentRenderer2D GenerateComponent2D(int type) {
		return new ComponentRenderer2D();
	}

	public void ClearRenderers()
	{
		if (Is3D) {
			foreach (EntityRenderer renderer in Renderers.Values)
			{
				if (renderer != null && IsInstanceValid(renderer))
				{
					renderer.FreeRenderer();
				}
			}
		} else {
			foreach (EntityRenderer2D renderer in Renderers.Values)
			{
				if (renderer != null && IsInstanceValid(renderer))
				{
					renderer.FreeRenderer();
				}
			}

		}
		ComponentRenderers = [];
		Renderers = [];
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
			IEntityRenderer renderer = Renderers[entityIndex];

			renderer.FreeComponentRenderer(component.ComponentType);

		}));


		_events.ConnectTo("OnEntityFreed", Callable.From((int entityIndex) => {
			if (!Renderers.ContainsKey(entityIndex)) return;
			IEntityRenderer renderer = Renderers[entityIndex];

			renderer.FreeRenderer();

			Renderers.Remove(entityIndex);

		}));

    }
}
