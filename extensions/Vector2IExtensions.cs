namespace Jodot.Extensions;

using Godot;

public static class Vector2IExtensions
{
    public static float DistanceTo(this Vector2I v2i, Vector2I other)
    {
        return Mathf.Sqrt(
            ((other.X - v2i.X) * (other.X - v2i.X)) + 
            ((other.Y - v2i.Y) * (other.Y - v2i.Y))
        );
    }

    public static Vector2 Normalized(this Vector2I v2i) {
        return new Vector2(v2i.X/v2i.Magnitude(), v2i.Y/v2i.Magnitude());
    }

    public static float Magnitude(this Vector2I v2i) {
        return Mathf.Sqrt(v2i.X * v2i.X + v2i.Y + v2i.Y);
    }

    public static Vector2 ToVector2(this Vector2I v2i) {
        return new (v2i.X, v2i.Y);
    }

    public static Vector2I ToVector2I(this Vector2 vector2) {
        return new (Mathf.RoundToInt(vector2.X), Mathf.RoundToInt(vector2.Y));
    }
}
