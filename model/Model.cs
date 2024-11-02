namespace Jodot.Model;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Colony.Scripts.Model.Core;
using Jodot.Injection;

public partial class Model: IActionSource
{
	public const int DATA_ARRAY_SIZE_INCREMENT = 32;
	public ModelItem[] ModelItems = new ModelItem[DATA_ARRAY_SIZE_INCREMENT];
	public ModelItemComponent[] ModelItemComponents = new ModelItemComponent[DATA_ARRAY_SIZE_INCREMENT];
	public int NextModelItemPointer = 1;
	public int NextModelItemComponentPointer = 1;
	public Dictionary<int, List<ModelItem>> ModelItemsByType;
	public Dictionary<int, List<ModelItemComponent>> ModelComponentsByType;
	protected IServiceContext s;
	protected ModelRunner _modelRunner;

	public virtual int ModelItemTypeCount => 0;
	public virtual int ModelComponentTypeCount => 0;
	public virtual string[] ModelItemTypeStrings => [];
	public virtual string[] ModelComponentTypeStrings => [];

	public virtual void BindListeners() {}

	public virtual ModelItem GenerateItem(int type, IServiceContext s) {
		throw new NotImplementedException();
	}

	public virtual ModelItemComponent GenerateComponent(int type, IServiceContext s) {
		throw new NotImplementedException();
	}

	public Model(IServiceContext s) {
		this.s = s;
		_modelRunner = s.GetService("ModelRunner");
		// Generate empty lists for all model item types
		ModelItemsByType = Enumerable.Range(0, ModelItemTypeCount).ToDictionary(t => t, t => new List<ModelItem>());
		ModelComponentsByType = Enumerable.Range(0, ModelComponentTypeCount).ToDictionary(t => t, t => new List<ModelItemComponent>());
		InitializeModel();
		BindListeners();
	}

	public ModelItem AddModelItem(ModelItem item) {
		if (NextModelItemPointer >= ModelItems.Length) {
			Array.Resize(ref ModelItems, ModelItems.Length + DATA_ARRAY_SIZE_INCREMENT);
		}
		item.ModelIndex = NextModelItemPointer;
		item.Model = this;
		ModelItemsByType[(int)item.ModelItemType].Add(item);

		ModelItems[NextModelItemPointer] = item;
		NextModelItemPointer++;
		item.Link(this);

		/*foreach(ModelComponentType componentType in item.DefaultComponents) {
			ModelItemComponent component = ModelComponentTypeGenerator[componentType](s);
			AddModelItemComponent(component);
			item.AddComponent(component);
		}*/

		/*item.OnCreated();*/
		return item;
	}

	public void AddImportedModelItem(ModelItem item) {
		ModelItems[item.ModelIndex] = item;
		item.Model = this;
		ModelItemsByType[(int)item.ModelItemType].Add(item);
	}

	public ModelItemComponent AddModelItemComponent(ModelItemComponent component) {
		if (NextModelItemComponentPointer >= ModelItemComponents.Length) {
			Array.Resize(ref ModelItemComponents, ModelItemComponents.Length + DATA_ARRAY_SIZE_INCREMENT);
		}
		component.Model = this;
		component.ComponentIndex = NextModelItemComponentPointer;
		ModelItemComponents[NextModelItemComponentPointer] = component;
		ModelComponentsByType[(int)component.ModelComponentType].Add(component);
		NextModelItemComponentPointer++;
		return component;
	}

	public void AddImportedModelItemComponent(ModelItemComponent component) {
		ModelItemComponents[component.ComponentIndex] = component;
		ModelComponentsByType[(int)component.ModelComponentType].Add(component);
	}

	public T GetModelItemOrNull<T>(int index) where T: ModelItem {
		if (index < 0) {
			return null;
		}

		if (index >= ModelItems.Length) {
			GD.PushError();
			GD.PrintErr($"Tried to get a model item at {index} but index was greater than model item length");
			return null;
		}

		T item = ModelItems[index] as T;
		if (item == null) {
			GD.PushError();
			GD.PrintErr($"Tried to get a model item of type {typeof(T).Name} at {index} but found wrong type");
		}
		return item;
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
			{"ModelItems", new Godot.Collections.Array< Godot.Collections.Dictionary<string, Variant>>(ModelItems.Select(item => item?.ExportData()).ToArray())},
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
			if (!modelItemSerialized.ContainsKey("ModelIndex")) {
				continue;
			};
			int modelItemType = (int)modelItemSerialized["ModelItemType"];
			ModelItem item = GenerateItem(modelItemType, s);
			item.ImportData(modelItemSerialized); 
			AddImportedModelItem(item);
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
		Array.ForEach(ModelItems, m => m?.Relink(this, s));

		//Link components
		Array.ForEach(ModelItemComponents, c => c?.Relink(this, s));

		//Trigger rerenders
		s.GetService("ModelRendererContainer")?.GenerateRenderers(this);
	}

	public void InitializeModel(int nextModelItemPointer = 0, int nextModelItemComponentPointer = 0) {
		NextModelItemComponentPointer = nextModelItemComponentPointer;
		NextModelItemPointer = nextModelItemPointer;
		
		ModelItems = new ModelItem[Mathf.CeilToInt((float)nextModelItemPointer/DATA_ARRAY_SIZE_INCREMENT) * DATA_ARRAY_SIZE_INCREMENT];
		ModelItemComponents = new ModelItemComponent[Mathf.CeilToInt((float)nextModelItemComponentPointer/DATA_ARRAY_SIZE_INCREMENT) * DATA_ARRAY_SIZE_INCREMENT];
	}

	// DEBUGGING
	public void PrintModelIndexLists() {
		string list = string.Join("\n", ModelItems.Select(s => s == null ? "###" : s.ModelIndex.ToString() + s.ModelItemType.ToString()).ToArray());
		GD.Print("###MODEL ITEMS###");
		GD.Print(list);

		string clist = string.Join("\n", ModelItemComponents.Select(s => s == null ? "###" : s.ComponentIndex.ToString() + s.ModelComponentType.ToString()).ToArray());
		GD.Print("###MODEL ITEM COMPONENTS###");
		GD.Print(clist);
	}



}
