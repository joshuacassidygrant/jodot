namespace Jodot.Model;

using Jodot.Injection;
using Jodot.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

public abstract class ModelAction: IActionSource
{
	protected IServiceContext s;

	protected Model model;

	public CancellationTokenSource cts;

	[Inject("ModelRunner")] protected ModelRunner modelRunner;

	[Inject("Events")] protected Events events;

	public string SubName = "Default";
	
	public abstract void Do();
	public abstract bool CanDo();

	public virtual Task Show() {
		return Task.CompletedTask;
	}

	public virtual void Finish() {}

	public virtual Model Undo(Model model){
		// TODO
		return model;
	}

	public Queue<ModelAction> SubActions = new();
	public ModelAction ParentAction;

	public ModelAction(IServiceContext serviceDirectory) {
		s = serviceDirectory;
		s.InjectDependencies(this);
		model = s.GetService("ModelRunner").Model;
	}

    public override string ToString()
    {
        return base.ToString() + "-" + SubName;
    }
}
