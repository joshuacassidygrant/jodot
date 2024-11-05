using Godot;

namespace Jodot.Rendering {
    public interface IRenderableComponent {
        public Node3D RendererProto { get; }
        public Node3D ColliderProto { get; }
    }
    
}