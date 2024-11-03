
using Jodot.Injection;
using Jodot.Model;
using Jodot.Events;

public abstract class ModelAction
{
	protected IServiceContext s;
	[Inject("ModelRunner")] protected ModelRunner modelRunner;

	[Inject("Events")] protected Events events;

	// Actions called by other actions must have less complexity.
	// Used to protect vs. cycles. Set in constructor.
	public int Complexity = 0;

	public string SubName = "Default";
	
	public abstract Model Do(Model model);
	public abstract bool CanDo(Model model);

	public virtual Model Undo(Model model){
		// TODO
		return model;
	}

	public ModelAction(IServiceContext serviceDirectory) {
		s = serviceDirectory;
		s.InjectDependencies(this);
	}

    public override string ToString()
    {
        return base.ToString() + "-" + SubName;
    }
}
