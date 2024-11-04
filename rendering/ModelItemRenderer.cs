using Godot;
using System;
using System.Linq;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Rendering;
using Jodot.Events;
using Colony.Scripts.Model.GameModel;

public partial class ModelItemRenderer : Node3D, IModelItemUpdateListener, IModelComponentUpdateListener
{
	public int BoundModelItemIndex;
	public Model BoundModel;
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

	public void BindModelItem(int index, Func<int, ComponentRenderer> generateComponent, IEventBus events, Model m)
	{
 
		events.WatchModelItem(index, this);

 		ILocationProvider locationProvider = (ILocationProvider)m.GetComponentOfTypeBoundToItem((int)ModelComponentType.LOCATEABLE, index);
		BoundModelItemIndex = index;
		BoundModel = m;
		if (locationProvider != null) {
			events.WatchModelComponent(locationProvider.GetComponentIndex, this);
			Position = locationProvider.GetPosition();
		}

		foreach (ModelItemComponent component in m.GetComponentsBoundToItem(index, (c) => c is IRenderableComponent)) {

			ComponentRenderer componentRenderer = generateComponent(component.ModelComponentType);
			AddChild(componentRenderer);
			componentRenderer.BindComponent(component, events);
		}
		Name = $"{index}";
	}

	public virtual void Update()
	{
 		ILocationProvider locationProvider = (ILocationProvider)BoundModel.GetComponentOfTypeBoundToItem((int)ModelComponentType.LOCATEABLE, BoundModelItemIndex);
		if (locationProvider != null)
		{
			Position = locationProvider.GetPosition();
		}
	}

}
