namespace Jodot.Rendering;

public interface IEntityRenderer
{
    void FreeComponentRenderer(int componentType);
    void FreeRenderer();
}