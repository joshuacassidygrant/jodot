namespace Jodot.Events;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Jodot.Model;


public partial class Events : Node, IEventBus
{

	public virtual void ConnectTo(string eventName, Callable fn)
	{
		Connect(eventName, fn);
	}



	public virtual void EmitFrom(string eventName, params Variant[] args)
	{
		EmitSignal(eventName, args);
	}

	public virtual void DisconnectFrom(string eventName, Callable fn)
	{
		Disconnect(eventName, fn);
	}

	public HashSet<int> DirtyComponents = [];
	public HashSet<int> DirtyItems = [];

	public Dictionary<int, List<IModelItemUpdateListener>> ItemListeners = [];

	public Dictionary<int, List<IModelComponentUpdateListener>> ComponentListeners = [];

	public Events() { }

	public override void _Process(double delta)
	{
		base._Process(delta);
		foreach (int index in DirtyComponents)
		{
			if (ComponentListeners.TryGetValue(index, out List<IModelComponentUpdateListener> value))
			{
				foreach (IModelComponentUpdateListener listener in value)
				{
					listener?.Update();
				}
			}
		}

		foreach (int index in DirtyItems)
		{
			if (ItemListeners.TryGetValue(index, out List<IModelItemUpdateListener> value))
			{
				foreach (IModelItemUpdateListener listener in value)
				{
					listener?.Update();
				}
			}
		}

		DirtyComponents = [];
		DirtyItems = [];
	}

	public void SoilComponent(int index)
	{
		DirtyComponents.Add(index);
	}

	public void SoilItem(int index)
	{
		DirtyItems.Add(index);
	}

	public void WatchModelComponent(int componentIndex, IModelComponentUpdateListener listener)
	{
		if (!ComponentListeners.ContainsKey(componentIndex))
		{
			ComponentListeners.Add(componentIndex, new());
		}
		ComponentListeners[componentIndex].Add(listener);
	}

	public void WatchModelItem(int itemIndex, IModelItemUpdateListener listener)
	{
		if (!ItemListeners.ContainsKey(itemIndex))
		{
			ItemListeners.Add(itemIndex, new());
		}
		ItemListeners[itemIndex].Add(listener);
	}

	public void UnwatchModelComponent(int componentIndex, IModelComponentUpdateListener listener)
	{
		if (ComponentListeners.ContainsKey(componentIndex))
		{
			ComponentListeners[componentIndex].Remove(listener);
		}
	}

	public void UnwatchModelItem(int itemIndex, IModelItemUpdateListener listener)
	{
		if (ItemListeners.ContainsKey(itemIndex))
		{
			ItemListeners[itemIndex].Remove(listener);
		}
	}

	public void SoilAll(Model model)
	{
		DirtyComponents.UnionWith(Enumerable.Range(0, model.NextComponentPointer).ToArray());
		DirtyItems.UnionWith(Enumerable.Range(0, model.NextEntityPointer).ToArray());
	}

	// INFRA

	[Signal]
	public delegate void ServiceDirectoryInitializedEventHandler();

	// MODEL GENERIC
	[Signal]
	public delegate void ModelItemDestroyedEventHandler(int modelIdx);

	[Signal]
	public delegate void RequestGameSaveEventHandler(string fileName);

	[Signal]
	public delegate void RequestGameLoadEventHandler(string fileName);

	[Signal]
	public delegate void HandleGameLoadedEventHandler();

	[Signal]
	public delegate void RequestDataTestEventHandler();

	[Signal]
	public delegate void ModelSetupCompletedEventHandler();

	// RENDERING
	[Signal]
	public delegate void RequestGenerateAllRenderersEventHandler();

	[Signal]
	public delegate void RequestDestroyAllRenderersEventHandler();

	[Signal]
	public delegate void OnComponentFreedEventHandler(int componentIndex);

	[Signal]
	public delegate void OnEntityFreedEventHandler(int modelIndex);

}
