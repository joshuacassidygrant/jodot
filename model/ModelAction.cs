namespace Jodot.Model;

using Jodot.Injection;
using Jodot.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Godot;

public abstract class ModelAction: IActionSource
{
	protected IServiceContext s;

	protected Model model;

	public CancellationTokenSource cts;

	public HashSet<int> SoiledEntities = [];

	public HashSet<int> SoiledComponents = [];

	[Inject("ModelRunner")] protected ModelRunner modelRunner;

	[Inject("Events")] protected Events events;

	public string SubName = "Default";
	
	public abstract void Do();
	public abstract bool CanDo();

	public virtual Task Show() {
		return Task.CompletedTask;
	}

	public virtual void Finish() {
		SoiledComponents.ToList().ForEach(events.SoilComponentDeferred);
		SoiledEntities.ToList().ForEach(i => {
			events.SoilItemDeferred(i);
			if (model.ComponentsByEntity != null && model.ComponentsByEntity[i] != null) {
				model.ComponentsByEntity[i].ToList().ForEach(events.SoilComponentDeferred);
			}
		});

	}

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
