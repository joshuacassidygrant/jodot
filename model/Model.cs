namespace Jodot.Model;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Jodot.Injection;
using Jodot.Content.Resources;

public partial class Model: IActionSource
{
	public virtual IModelInfo Info => null;
	public const int DATA_ARRAY_SIZE_INCREMENT = 32;
	public Component[] Components = new Component[DATA_ARRAY_SIZE_INCREMENT];
	public int NextEntityPointer = 1;
	public int NextComponentPointer = 1;
	public Dictionary<int, List<Component>> ComponentsByType;

	public HashSet<int> FreedEntities = new();
	public int[][] ComponentsByEntity = new int[DATA_ARRAY_SIZE_INCREMENT][];

	protected IServiceContext s;
	protected ModelRunner _modelRunner;

	public virtual void BindListeners() {}

	public virtual Component GenerateComponent(int type, IServiceContext s) {
		return Info.GetComponentGenerator(type)(s);
	}

	public Model(IServiceContext s) {
		this.s = s;
		_modelRunner = s.GetService("ModelRunner");
		// Generate empty lists for all model item types
		ComponentsByType = Enumerable.Range(0, Info.ComponentTypeCount).ToDictionary(t => t, t => new List<Component>());
		InitializeModel();
		BindListeners();
	}

	public int GenerateEntity(Component[] components) {
		int index;

		if (FreedEntities.Count > 0) {
			index = FreedEntities.First();
			FreedEntities.Remove(index);
		} else {
			index = NextEntityPointer++;
		}
		HashSet<int> componentsAdded = [];
		HashSet<int> defaultComponentsToGenerate = [];

		foreach (Component component in components) {
			if (component.EntityIndex != -1 || component.Model != null) {
				GD.PrintErr("Component already bound");
				continue;
			}

			component.EntityIndex = index;
			AddComponent(component, index);
			
			componentsAdded.Add(component.ComponentType);
			defaultComponentsToGenerate.Remove(component.ComponentType);
			
			foreach (int type in component.RequiredComponents) {
				if (!componentsAdded.Contains(type)) {
					defaultComponentsToGenerate.Add(type);
				}
			}
		}

		DefaultComponentResourceFactory defaultComponentResourceFactory = s.GetService("DefaultComponentResourceFactory");
		if (defaultComponentResourceFactory != null) {
			foreach (int componentType in defaultComponentsToGenerate) {
				ModelItemComponentResource resource = defaultComponentResourceFactory.GetDefaultResource(componentType);
				Component c2 = resource.GenerateComponent(s);
				AddComponent(c2, index);
				
			}
		} else {
			GD.Print($"Missing {defaultComponentsToGenerate.Count} required components on {index}");
		}

		return index;
	}

	public Component GetComponentOfTypeBoundToEntity(int componentType, int itemIndex) {
		int componentIndex = ComponentsByEntity[itemIndex][componentType];

		if (componentIndex <= 0) return null;

		return Components[ComponentsByEntity[itemIndex][componentType]];
	}

	public Component[] GetComponentsBoundToEntity(int entityIndex, System.Type type) {
		Func<Component, bool> predicate = (c) => type.IsInstanceOfType(c);
		return ComponentsByEntity[entityIndex]
			.Where(i => i != 0)
			.Select(i => Components[i])
			.Where(predicate).ToArray();
	}


	public Component[] GetComponentsBoundToEntity(int entityIndex, Func<Component, bool> predicate = null) {
		if (predicate == null) {
			predicate = (c) => true;
		}
		return ComponentsByEntity[entityIndex]
			.Where(i => i != 0)
			.Select(i => Components[i])
			.Where(predicate).ToArray();
	}

	public Component AddComponent(Component component, int entityIndex) {
		if (NextComponentPointer >= Components.Length) {
			Array.Resize(ref Components, Components.Length + DATA_ARRAY_SIZE_INCREMENT);
		}
		component.Model = this;
		component.ComponentIndex = NextComponentPointer;
		Components[NextComponentPointer] = component;
		ComponentsByType[(int)component.ComponentType].Add(component);
		NextComponentPointer++;

		if (entityIndex >= ComponentsByEntity.Length) {
			Array.Resize(ref ComponentsByEntity, ComponentsByEntity.Length + DATA_ARRAY_SIZE_INCREMENT);
		}

		if (ComponentsByEntity[entityIndex] == null) {
			ComponentsByEntity[entityIndex] = new int[Info.ComponentTypeCount];
		} else if (ComponentsByEntity[entityIndex][(int)component.ComponentType] != 0) {
			// TODO remove and dispose of old component
		}
		ComponentsByEntity[entityIndex][(int)component.ComponentType] = component.ComponentIndex;

		
		return component;

	}

	public void AddImportedComponent(Component component) {
		Components[component.ComponentIndex] = component;
		ComponentsByType[(int)component.ComponentType].Add(component);
	}

	public T GetComponentOrNull<T>(int index) where T: Component {
		if (index < 0) {
			return null;
		}

		if (index >= Components.Length) {
			GD.PushError();
			GD.PrintErr($"Tried to get a component at {index} but index was greater than model item length");
			return null;
		}

		T component = Components[index] as T;
		if (component == null) {
			GD.PushError();
			GD.PrintErr($"Tried to get a component of type {typeof(T).Name} at {index} but found wrong type");
		}
		return component;
	}


	public virtual Godot.Collections.Dictionary<string, Variant> ExportData() {
		PrintModelIndexLists();


		Godot.Collections.Dictionary<string, Variant> data = new Godot.Collections.Dictionary<string, Variant> {
			{"NextEntityPointer", NextEntityPointer},
			{"NextComponentPointer", NextComponentPointer},
			{"Components", new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>(Components.Select(item => item?.Export()).ToArray())},
			{"Version", Info.Version}
		};
		return data;
	}

	public virtual void ImportData(Godot.Collections.Dictionary<string, Variant> data) {
		// Clear renderers
		s.GetService("ModelRendererContainer")?.ClearRenderers();

		// Reinitialize model
		InitializeModel((int)data["NextModelItemPointer"], (int)data["NextModelItemComponentPointer"]);
		
		//Import model items
		Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> modelItemsSerialized = (Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>)data["ModelItems"];
		foreach (Godot.Collections.Dictionary<string, Variant> modelItemSerialized in modelItemsSerialized) {
			/*if (!modelItemSerialized.ContainsKey("ModelIndex")) {
				continue;
			};
			int modelItemType = (int)modelItemSerialized["ModelItemType"];
			ModelItem item = GenerateItem(modelItemType, s);
			item.ImportData(modelItemSerialized); 
			AddImportedModelItem(item);*/
		};
		// TEMP
		//Import components
		Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> modelItemComponentsSerialized = (Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>)data["ModelItemComponents"];
		foreach (Godot.Collections.Dictionary<string, Variant> modelItemComponentSerialized in modelItemComponentsSerialized) {
			if (!modelItemComponentSerialized.ContainsKey("ComponentIndex")) {
				continue;
			};
			int modelComponentType = (int)modelItemComponentSerialized["ModelComponentType"];
			Component item = GenerateComponent(modelComponentType, s);
			item.ImportData(modelItemComponentSerialized);
			AddImportedComponent(item);

		};
		// TODO: need to make sure they are in correct list indices!
		//PrintModelIndexLists();

		//Link model items;
		//Array.ForEach(ModelItems, m => m?.Relink(this, s));

		//Link components
		Array.ForEach(Components, c => c?.Relink(this, s));

		//Trigger rerenders
		s.GetService("ModelRendererContainer")?.GenerateRenderers(this);
	}

	public void InitializeModel(int nextEntityPointer = 1, int nextComponentPointer = 1) {
		NextComponentPointer = nextComponentPointer;
		NextEntityPointer = nextEntityPointer;
		
		Components = new Component[Mathf.CeilToInt((float)nextComponentPointer/DATA_ARRAY_SIZE_INCREMENT) * DATA_ARRAY_SIZE_INCREMENT];
	}

	// DEBUGGING
	public void PrintModelIndexLists() {
		string clist = string.Join("\n", Components.Select(s => s == null ? "###" : s.ComponentIndex.ToString() + s.ComponentType.ToString()).ToArray());
		GD.Print("### COMPONENTS###");
		GD.Print(clist);
	}



}
