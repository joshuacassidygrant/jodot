namespace Jodot.Rendering;

using Godot;
using Jodot.Model;
using Jodot.Events;

public partial class ComponentRenderer2D: Node2D, IModelComponentUpdateListener, IComponentRenderer {

    public int ComponentIndex;
    public Component Component;

    public Node2D Renderer;
    public Node2D Collider;
    public AnimationPlayer Animator;

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

                if (component is IAnimateable2D animateable) {
                    AnimationLibrary animationLibrary = animateable.AnimationLibrary;
                    if (animationLibrary == null) return;
                    Sprite2D renderer2d = (Sprite2D)Renderer;
                    renderer2d.Vframes = animateable.AnimationVFrames;
                    renderer2d.Hframes = animateable.AnimationHFrames;
                    Animator = new AnimationPlayer();
                    Renderer.AddChild(Animator);
                    // TODO: move this
                    Animator.AddAnimationLibrary("GENERIC", animationLibrary);
                }
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