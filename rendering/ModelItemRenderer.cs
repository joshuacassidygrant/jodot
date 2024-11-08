namespace Jodot.Rendering;

using Godot;
using System;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;
using System.Collections.Generic;

public partial class ModelItemRenderer : Node3D, IModelItemUpdateListener, IModelComponentUpdateListener
{
	public int BoundModelItemIndex;
	public Model BoundModel;
	public Vector3 NextPosition;
	public Vector3 LastPosition;
	public Area3D Collider;
	public ILocationProvider LocationProvider;
	public Dictionary<int, ComponentRenderer> ComponentRenderersByType = new();

	public bool Valid => IsInstanceValid(this);

	// Services
#pragma warning disable CS0649
	[Inject("Events")] private IEventBus _events;
#pragma warning restore CS0649

	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void BindModelItem(int index, Func<int, ComponentRenderer> generateComponent, IEventBus events, Model m, ILocationProvider locationProvider)
	{
		_events = events;
		events.WatchModelItem(index, this);

 		LocationProvider = locationProvider;
		BoundModelItemIndex = index;
		BoundModel = m;
		if (locationProvider != null) {
			events.WatchModelComponent(locationProvider.GetComponentIndex, this);
			Position = locationProvider.GetPosition();
		}

		foreach (Component component in m.GetComponentsBoundToEntity(index, (c) => c is IRenderableComponent)) {

			ComponentRenderer componentRenderer = generateComponent(component.ComponentType);
			AddChild(componentRenderer);
			componentRenderer.BindComponent(component, events);
			ComponentRenderersByType.Add(component.ComponentType, componentRenderer);
		}
		Name = $"{index}";
	}

	public void FreeComponentRenderer(int componentType) {
		if (!ComponentRenderersByType.ContainsKey(componentType)) return;
		ComponentRenderer renderer = ComponentRenderersByType[componentType];
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

	public virtual void Update()
	{
		if (LocationProvider != null)
		{
			Position = LocationProvider.GetPosition();
		}
	}

}
