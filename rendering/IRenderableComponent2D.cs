using Godot;

namespace Jodot.Rendering {
    public interface IRenderableComponent2D {
        public Node2D RendererProto { get; }
        public Node2D ColliderProto { get; }
    }
    
}