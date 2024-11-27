namespace Jodot.Rendering;

using Godot;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;

public partial class ComponentRenderer: Node3D, IModelComponentUpdateListener, IComponentRenderer {

    public int ComponentIndex;
    public Component Component;

    public Node3D Renderer;
    public Node3D Collider;

	// Services
	#pragma warning disable CS0649
	[Inject("Events")] private IEventBus _events;
	#pragma warning restore CS0649

    public void BindComponent(Component component, IEventBus events) {
        Component = component;
        ComponentIndex = component.ComponentIndex;

        IRenderableComponent renderableComponent = (IRenderableComponent)component;

        if (renderableComponent != null) {
            Renderer = (Node3D)renderableComponent.RendererProto?.Duplicate();
            Collider = (Area3D)renderableComponent.ColliderProto?.Duplicate();
            
            if (Renderer != null) {
                Renderer.Visible = true;
                AddChild(Renderer);
            }

            if (Collider != null) {
                AddChild(Collider);
            }
        }
        Name = $"{component.ComponentIndex} : {component.ComponentType}";
        PostBind();
        events.WatchModelComponent(ComponentIndex, this);
    }

    public virtual void Update() {

    }

    public virtual void PostBind() {

    }
}