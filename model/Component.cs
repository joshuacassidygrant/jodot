namespace Jodot.Model;

using Godot;
using Jodot.Injection;

public partial class Component
{
	public virtual int ComponentType => 0;
	public virtual int ModelComponentLayoutTag => 0;

	public virtual string Name => "Base Component";

	public virtual int[] RequiredComponents => []; 

	public int ComponentIndex = -1;
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
			{"ModelComponentType", (int)ComponentType},
			{"ComponentIndex", ComponentIndex},
			{"ModelItemIndex", EntityIndex}
		};

		return data;
	}

	public virtual void ImportData(Godot.Collections.Dictionary<string, Variant> data) {
		ComponentIndex = (int)data["ComponentIndex"];
		EntityIndex = (int)data["ModelItemIndex"];
	}

	public virtual void Relink(Model model, IServiceContext s) {
		//TODO
		//ModelItem = model.ModelItems[ModelItemIndex];
	}

	public virtual string GetStatusText() {
		return "";
	}
	

}
