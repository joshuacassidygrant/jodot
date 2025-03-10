namespace Jodot.Model;

using Godot;

public interface ILocationProvider {
    public int GetComponentIndex { get; }
    public int GetBoundEntityIndex { get; }
    public Vector3 GetPosition();

    public bool IsPositionValid();

}