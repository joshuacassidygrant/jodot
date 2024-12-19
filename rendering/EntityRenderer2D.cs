namespace Jodot.Rendering;

using Godot;
using System;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;
using System.Collections.Generic;

public partial class EntityRenderer2D : Node2D, IModelItemUpdateListener, IModelComponentUpdateListener, IEntityRenderer
{
	public int BoundModelItemIndex;
	public Model BoundModel;
	public Area2D Collider;
	public ILocationProvider LocationProvider;
	public Dictionary<int, ComponentRenderer2D> ComponentRenderersByType = new();

	public bool Valid => IsInstanceValid(this);

	// Services
#pragma warning disable CS0649
	[Inject("Events")] private IEventBus _events;
#pragma warning restore CS0649

	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void BindModelItem(int index, Func<int, ComponentRenderer2D> generateComponent, IEventBus events, Model m, ILocationProvider locationProvider, ModelRendererContainer modelRendererContainer)
	{
		_events = events;
		events.WatchModelItem(index, this);

 		LocationProvider = locationProvider;
		BoundModelItemIndex = index;
		BoundModel = m;
		if (locationProvider != null) {
			events.WatchModelComponent(locationProvider.GetComponentIndex, this);
			Vector3 pos = locationProvider.GetPosition();
			Position = new(pos.X, pos.Y);
			Visible = locationProvider.IsPositionValid();
		}

		foreach (Component component in m.GetComponentsBoundToEntity(index, (c) => c is IRenderableComponent2D)) {

			ComponentRenderer2D componentRenderer = generateComponent(component.ComponentType);
			AddChild(componentRenderer);
			componentRenderer.BindComponent(component, events);
			ComponentRenderersByType.Add(component.ComponentType, componentRenderer);
			componentRenderer.PostBind();
			modelRendererContainer.ComponentRenderers.Add(component.ComponentIndex, componentRenderer);
		}
		Name = $"{index}";
	}

	public void FreeComponentRenderer(int componentType) {
		if (!ComponentRenderersByType.ContainsKey(componentType)) return;
		ComponentRenderer2D renderer = ComponentRenderersByType[componentType];
		if (IsInstanceValid(renderer)) {
			renderer.QueueFree();
			_events.UnwatchModelComponent(renderer.ComponentIndex, renderer);
		}
		ComponentRenderersByType.Remove(componentType);

		if (ComponentRenderersByType.Count == 0) {
			_events.UnwatchModelItem(BoundModelItemIndex, this);
			QueueFree();
		}
	}

	public void FreeRenderer() {
		IEnumerable<int> keys = ComponentRenderersByType.Keys;
		foreach (int componentKey in keys) {
			FreeComponentRenderer(componentKey);
		}
		
		_events.UnwatchModelItem(BoundModelItemIndex, this);
		if (LocationProvider != null) {
			_events.UnwatchModelComponent(LocationProvider.GetComponentIndex, this);
		}
		QueueFree();
	}

	public void FaceX(Vector2 target) {
        float val = Scale.X;
        if (Position.X > target.X) {
            val = -1f;
        } else if (Position.X < target.X) {
            val = 1f;
        }
		Scale = new(val, 1f);

	}

	public virtual void Update()
	{
		if (LocationProvider != null)
		{
			Vector3 pos = LocationProvider.GetPosition();
			Position = new(pos.X, pos.Y);
			Visible = LocationProvider.IsPositionValid();
		}
	}

}
