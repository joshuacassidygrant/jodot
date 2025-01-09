namespace Jodot.Events;

using Godot;
using Jodot.Model;

public interface IEventBus {
    public void ConnectTo(string name, Callable call);
    public void EmitFrom(string eventName, params Variant[] args);
    public void DisconnectFrom(string name, Callable call);

    // seperate these!

    public void SoilComponent(int index);
    public void SoilItem(int index);
    public void SoilAll(Model model);

    public void SoilComponentDeferred(int index);
    public void SoilItemDeferred(int index);
    public void SoilDeferred();

    public void WatchModelComponent(int index, IModelComponentUpdateListener listener);

    public void WatchModelItem(int index, IModelItemUpdateListener listener);

    public void UnwatchModelComponent(int index, IModelComponentUpdateListener listener);

    public void UnwatchModelItem(int index, IModelItemUpdateListener listener);
}