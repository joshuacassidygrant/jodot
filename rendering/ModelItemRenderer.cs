namespace Jodot.Rendering;

using Godot;
using System;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;

public partial class ModelItemRenderer : Node3D, IModelItemUpdateListener, IModelComponentUpdateListener
{
	public int BoundModelItemIndex;
	public Model BoundModel;
	public Vector3 NextPosition;
	public Vector3 LastPosition;
	public Area3D Collider;
	public ILocationProvider LocationProvider;

	public bool Valid => IsInstanceValid(this);

	// Services
#pragma warning disable CS0649
	[Inject("Events")] private Events _events;
#pragma warning restore CS0649

	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void BindModelItem(int index, Func<int, ComponentRenderer> generateComponent, IEventBus events, Model m, ILocationProvider locationProvider)
	{
 
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
		}
		Name = $"{index}";
	}

	public virtual void Update()
	{
		if (LocationProvider != null)
		{
			Position = LocationProvider.GetPosition();
		}
	}

}
