namespace Jodot.Rendering;

using Godot;

public interface IAnimateable2D {
    public AnimationLibrary AnimationLibrary {get;}
    public int AnimationHFrames {get;}
    public int AnimationVFrames {get;}
}