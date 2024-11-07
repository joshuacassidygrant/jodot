namespace Jodot.Model;

using System.Reflection;
using Godot;
using Jodot.Injection;
using Jodot.Serialization;

public partial class Component: ISerializable
{
	public virtual int ComponentType => 0;
	public virtual int ModelComponentLayoutTag => 0;

	public virtual string Name => "Base Component";

	public virtual int[] RequiredComponents => []; 

	[ModelProperty(SerializationStrategy.INT)]
	public int ComponentIndex = -1;
	[ModelProperty(SerializationStrategy.INT)]
	public int EntityIndex = -1;
	public Model Model;

	// SERVICES
	protected IServiceContext s;

	public int GetComponentIndex => ComponentIndex;
	public int GetBoundEntityIndex => EntityIndex;

	public virtual void OnCreated() {
		
	}

	public Component(IServiceContext serviceDirectory) {
		s = serviceDirectory;
		s.InjectDependencies(this);
	}

	
    public virtual ModelItemComponentSubPanel GenerateUiSubPanel() {
        return null;
    }

	public virtual void Link(int modelItemIndex, Model model) {
	}
	
	public virtual void OnPhaseEnd() {

	}

	public virtual void OnPhaseBegin() {

	}

	public virtual void OnSelect() {

	}

	public virtual void OnDeselect() {

	}

	public virtual bool IsActive() {
		return true; // TODO
	}

	public virtual Godot.Collections.Dictionary<string, Variant> ExportData() {
		Godot.Collections.Dictionary<string, Variant> data =  new() {
			{"ComponentType", (int)ComponentType},
			{"ComponentIndex", ComponentIndex},
			{"EntityIndex", EntityIndex}
		};

		return data;
	}

	public virtual void ImportData(Godot.Collections.Dictionary<string, Variant> data) {
		ComponentIndex = (int)data["ComponentIndex"];
		EntityIndex = (int)data["EntityIndex"];
	}

	public virtual void Rebind(Model model, IServiceContext s) {
		Model = model;
		this.s = s;

		this.ResolveBoundResources(s);

	}


	public virtual void Relink(Model model, IServiceContext s) {
		//TODO
		//ModelItem = model.ModelItems[ModelItemIndex];
	}

	public virtual string GetStatusText() {
		return "";
	}
	

}
