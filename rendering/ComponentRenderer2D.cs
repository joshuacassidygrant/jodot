namespace Jodot.Rendering;

using Godot;
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;

public partial class ComponentRenderer2D: Node2D, IModelComponentUpdateListener {

    public int ComponentIndex;
    public Component Component;

    public Node2D Renderer;
    public Node2D Collider;

	// Services
	#pragma warning disable CS0649
	[Inject("Events")] private IEventBus _events;
	#pragma warning restore CS0649

    public void BindComponent(Component component, IEventBus events) {
        Component = component;
        ComponentIndex = component.ComponentIndex;

        IRenderableComponent2D renderableComponent2D = (IRenderableComponent2D)component;

        if (renderableComponent2D != null) {
            Renderer = (Node2D)renderableComponent2D.RendererProto?.Duplicate();
            Collider = (Area2D)renderableComponent2D.ColliderProto?.Duplicate();
            
            if (Renderer != null) {
                Renderer.Visible = true;
                AddChild(Renderer);
            }

            if (Collider != null) {
                AddChild(Collider);
            }
        }
        Name = $"{component.ComponentIndex} : {component.ComponentType}";

        events.WatchModelComponent(ComponentIndex, this);
    }

    public virtual void Update() {

    }

    public virtual void PostBind() {
        
    }
}