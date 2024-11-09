namespace Jodot.Model;

using Jodot.Injection;
using Jodot.Events;
using System.Collections.Generic;

public abstract class ModelAction: IActionSource
{
	protected IServiceContext s;
	[Inject("ModelRunner")] protected ModelRunner modelRunner;

	[Inject("Events")] protected Events events;

	public string SubName = "Default";
	
	public abstract Model Do(Model model);
	public abstract bool CanDo(Model model);

	public virtual Model Undo(Model model){
		// TODO
		return model;
	}

	public Queue<ModelAction> SubActions = new();
	public ModelAction ParentAction;

	public ModelAction(IServiceContext serviceDirectory) {
		s = serviceDirectory;
		s.InjectDependencies(this);
	}

    public override string ToString()
    {
        return base.ToString() + "-" + SubName;
    }
}
