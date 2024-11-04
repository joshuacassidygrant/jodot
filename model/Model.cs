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
	public ModelItemComponent[] ModelItemComponents = new ModelItemComponent[DATA_ARRAY_SIZE_INCREMENT];
	public int NextModelItemPointer = 1;
	public int NextModelItemComponentPointer = 1;
	public Dictionary<int, List<ModelItemComponent>> ModelComponentsByType;

	public HashSet<int> FreedItems = new();
	public int[][] ComponentsByItem = new int[DATA_ARRAY_SIZE_INCREMENT][];

	protected IServiceContext s;
	protected ModelRunner _modelRunner;

	public virtual string[] ModelItemTypeStrings => [];
	public virtual string[] ModelComponentTypeStrings => [];

	public virtual void BindListeners() {}

	public virtual ModelItemComponent GenerateComponent(int type, IServiceContext s) {
		return Info.GetComponentGenerator(type)(s);
	}

	public Model(IServiceContext s) {
		this.s = s;
		_modelRunner = s.GetService("ModelRunner");
		// Generate empty lists for all model item types
		ModelComponentsByType = Enumerable.Range(0, Info.ComponentTypeCount).ToDictionary(t => t, t => new List<ModelItemComponent>());
		InitializeModel();
		BindListeners();
	}

	public int GenerateModelItem(ModelItemComponent[] components) {
		int index;

		if (FreedItems.Count > 0) {
			index = FreedItems.First();
			FreedItems.Remove(index);
		} else {
			index = NextModelItemPointer++;
		}
		HashSet<int> componentsAdded = [];
		HashSet<int> defaultComponentsToGenerate = [];

		foreach (ModelItemComponent component in components) {
			if (component.ModelItemIndex != -1 || component.Model != null) {
				GD.PrintErr("Component already bound");
				continue;
			}

			component.ModelItemIndex = index;
			AddModelItemComponent(component, index);
			
			componentsAdded.Add(component.ModelComponentType);
			defaultComponentsToGenerate.Remove(component.ModelComponentType);
			
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
				ModelItemComponent c2 = resource.GenerateComponent(s);
				AddModelItemComponent(c2, index);
				
			}
		} else {
			GD.Print($"Missing {defaultComponentsToGenerate.Count} required components on {index}");
		}

		return index;
	}

	public ModelItemComponent GetComponentOfTypeBoundToItem(int componentType, int itemIndex) {
		int componentIndex = ComponentsByItem[itemIndex][componentType];

		if (componentIndex <= 0) return null;

		return ModelItemComponents[ComponentsByItem[itemIndex][componentType]];
	}

	public ModelItemComponent[] GetComponentsBoundToItem(int itemIndex, System.Type type) {
		Func<ModelItemComponent, bool> predicate = (c) => type.IsInstanceOfType(c);
		return ComponentsByItem[itemIndex]
			.Where(i => i != 0)
			.Select(i => ModelItemComponents[i])
			.Where(predicate).ToArray();
	}


	public ModelItemComponent[] GetComponentsBoundToItem(int itemIndex, Func<ModelItemComponent, bool> predicate = null) {
		if (predicate == null) {
			predicate = (c) => true;
		}
		return ComponentsByItem[itemIndex]
			.Where(i => i != 0)
			.Select(i => ModelItemComponents[i])
			.Where(predicate).ToArray();
	}

	public ModelItemComponent AddModelItemComponent(ModelItemComponent component, int itemIndex) {
		if (NextModelItemComponentPointer >= ModelItemComponents.Length) {
			Array.Resize(ref ModelItemComponents, ModelItemComponents.Length + DATA_ARRAY_SIZE_INCREMENT);
		}
		component.Model = this;
		component.ComponentIndex = NextModelItemComponentPointer;
		ModelItemComponents[NextModelItemComponentPointer] = component;
		ModelComponentsByType[(int)component.ModelComponentType].Add(component);
		NextModelItemComponentPointer++;

		if (itemIndex >= ComponentsByItem.Length) {
			Array.Resize(ref ComponentsByItem, ComponentsByItem.Length + DATA_ARRAY_SIZE_INCREMENT);
		}

		if (ComponentsByItem[itemIndex] == null) {
			ComponentsByItem[itemIndex] = new int[Info.ComponentTypeCount];
		} else if (ComponentsByItem[itemIndex][(int)component.ModelComponentType] != 0) {
			// TODO remove and dispose of old component
		}
		ComponentsByItem[itemIndex][(int)component.ModelComponentType] = component.ComponentIndex;

		
		return component;

	}

	public void AddImportedModelItemComponent(ModelItemComponent component) {
		ModelItemComponents[component.ComponentIndex] = component;
		ModelComponentsByType[(int)component.ModelComponentType].Add(component);
	}

	public T GetModelItemComponentOrNull<T>(int index) where T: ModelItemComponent {
		if (index < 0) {
			return null;
		}

		if (index >= ModelItemComponents.Length) {
			GD.PushError();
			GD.PrintErr($"Tried to get a component at {index} but index was greater than model item length");
			return null;
		}

		T component = ModelItemComponents[index] as T;
		if (component == null) {
			GD.PushError();
			GD.PrintErr($"Tried to get a component of type {typeof(T).Name} at {index} but found wrong type");
		}
		return component;
	}


	public virtual Godot.Collections.Dictionary<string, Variant> ExportData() {
		PrintModelIndexLists();

		Godot.Collections.Dictionary<string, Variant> data = new Godot.Collections.Dictionary<string, Variant> {
			{"NextModelItemPointer", NextModelItemPointer},
			{"NextModelItemComponentPointer", NextModelItemComponentPointer},
			{"ModelItemComponents", new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>(ModelItemComponents.Select(item => item?.ExportData()).ToArray())}
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
			ModelItemComponent item = GenerateComponent(modelComponentType, s);
			item.ImportData(modelItemComponentSerialized);
			AddImportedModelItemComponent(item);

		};
		// TODO: need to make sure they are in correct list indices!
		//PrintModelIndexLists();

		//Link model items;
		//Array.ForEach(ModelItems, m => m?.Relink(this, s));

		//Link components
		Array.ForEach(ModelItemComponents, c => c?.Relink(this, s));

		//Trigger rerenders
		s.GetService("ModelRendererContainer")?.GenerateRenderers(this);
	}

	public void InitializeModel(int nextModelItemPointer = 1, int nextModelItemComponentPointer = 1) {
		NextModelItemComponentPointer = nextModelItemComponentPointer;
		NextModelItemPointer = nextModelItemPointer;
		
		ModelItemComponents = new ModelItemComponent[Mathf.CeilToInt((float)nextModelItemComponentPointer/DATA_ARRAY_SIZE_INCREMENT) * DATA_ARRAY_SIZE_INCREMENT];
	}

	// DEBUGGING
	public void PrintModelIndexLists() {
		string clist = string.Join("\n", ModelItemComponents.Select(s => s == null ? "###" : s.ComponentIndex.ToString() + s.ModelComponentType.ToString()).ToArray());
		GD.Print("###MODEL ITEM COMPONENTS###");
		GD.Print(clist);
	}



}
