using Godot;
using System;
using System.Linq;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Rendering;
using Jodot.Events;

public partial class ModelItemRenderer : Node3D, IModelItemUpdateListener, IModelComponentUpdateListener
{
	public ModelItem ModelItem;
	public Vector3 NextPosition;
	public Vector3 LastPosition;
	public Area3D Collider;

	public bool Valid => IsInstanceValid(this);

	// Services
#pragma warning disable CS0649
	[Inject("Events")] private Events _events;
#pragma warning restore CS0649

	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void BindModelItem(ModelItem item, Func<int, ComponentRenderer> generateComponent, IEventBus events)
	{
		ModelItem = item;
 
		events.WatchModelItem(item.ModelIndex, this);

 		ILocationProvider locationProvider = ModelItem?.Components.OfType<ILocationProvider>().FirstOrDefault();
		if (locationProvider != null) {
			events.WatchModelComponent(locationProvider.GetComponentIndex, this);
		}

		foreach (ModelItemComponent component in item.Components.OfType<IRenderableComponent>()) {
			ComponentRenderer componentRenderer = generateComponent(component.ModelComponentType);
			AddChild(componentRenderer);
			componentRenderer.BindComponent(component, events);
		}
		Name = $"{item.ModelIndex} : {item.ModelItemType}";
	}

	public virtual void Update()
	{
 		ILocationProvider locationProvider = ModelItem?.Components.OfType<ILocationProvider>().FirstOrDefault();
		if (locationProvider != null)
		{
			Position = locationProvider.GetPosition();
		}
	}

}
