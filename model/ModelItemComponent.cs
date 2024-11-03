namespace Jodot.Model;

using Godot;
using Jodot.Injection;

public partial class ModelItemComponent
{
	public virtual int ModelComponentType => 0;
	public virtual int ModelComponentLayoutTag => 0;

	public virtual string Name => "Base Component";

	public virtual int[] RequiredComponents => []; // TODO: something with this

	public int ComponentIndex = -1;
	public ModelItem ModelItem;
	public int ModelItemIndex = -1;
	public Model Model;

	// SERVICES
	protected IServiceContext s;

	public int GetComponentIndex => ComponentIndex;
	public int GetBoundEntityIndex => ModelItemIndex;

	public virtual void OnCreated() {
		
	}

	public ModelItemComponent(IServiceContext serviceDirectory) {
		s = serviceDirectory;
		s.InjectDependencies(this);
	}

	
    public virtual ModelItemComponentSubPanel GenerateUiSubPanel() {
        return null;
    }

	public virtual void Link(ModelItem modelItem, Model model) {
	}

	public virtual void Bind(ModelItem modelItem, Model model) {
		ModelItem = modelItem;
		ModelItemIndex = modelItem.ModelIndex;
		// model.AddModelItemComponent(this);
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
		return ModelItem != null && ModelItem.Status == ModelItemStatus.ACTIVE;
	}

	public virtual Godot.Collections.Dictionary<string, Variant> ExportData() {
		Godot.Collections.Dictionary<string, Variant> data =  new() {
			{"ModelComponentType", (int)ModelComponentType},
			{"ComponentIndex", ComponentIndex},
			{"ModelItemIndex", ModelItemIndex}
		};

		return data;
	}

	public virtual void ImportData(Godot.Collections.Dictionary<string, Variant> data) {
		ComponentIndex = (int)data["ComponentIndex"];
		ModelItemIndex = (int)data["ModelItemIndex"];
	}

	public virtual void Relink(Model model, IServiceContext s) {
		//TODO
		//ModelItem = model.ModelItems[ModelItemIndex];
	}

	public virtual string GetStatusText() {
		return "";
	}
	

}
