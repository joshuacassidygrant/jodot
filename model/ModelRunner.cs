namespace Jodot.Model; 

using Godot;
using System;
using System.Text;
using System.Collections.Generic;
using Jodot.Injection;
using Jodot.Events;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

public partial class ModelRunner : IActionSource
{
	public Model Model;

	// SERVICES
	public IEventBus _events;
	protected IServiceContext s;

	private Queue<ModelAction> _actionQueue = new();
	CancellationTokenSource cts = new();

	public void BindEvents(IEventBus events) {
		_events = events;

		_events.ConnectTo("RequestSkipAnimation", Callable.From(() => {
			cts.Cancel();
		}));
	}

	public ModelRunner() {}

	public ModelRunner(IServiceContext s, IEventBus events) {
		this.s = s;
		BindEvents(events);
	}

	public virtual void NewModel() {
		Model = new Model(s);
	}

	public void QueueActions(List<ModelAction> actions, IActionSource source) {
        foreach (ModelAction action in actions)
        {
            if (source is ModelAction parentAction)
            {
                action.ParentAction = parentAction;

                if (IsCyclicalAction(action))
                {
                    throw new Exception("Cyclical action detected");
                }
                parentAction.SubActions.Enqueue(action);
            }
            else
            {
                _actionQueue.Enqueue(action);
            }
        }

		if (source is ModelAction parent) {
			RunQueue(parent.SubActions);
		} else {
        	RunQueue(_actionQueue);
		}
	}

	public async void RunQueue(Queue<ModelAction> queue) {	
		if (queue.Count == 0) return;

		// DebugLogQueue();

		ModelAction action = queue.Dequeue();
		cts = new();
		if (action.CanDo()) {
			action.Do();
			action.cts = cts;
			await Task.Run(action.Show, cts.Token);
			action.Finish();
		}
		RunQueue(queue);
	}

	public bool IsCyclicalAction(ModelAction action) {
		HashSet<Type> seenActionTypes = new();
		ModelAction current = action;
		while(current != null) {
			if (seenActionTypes.Contains(current.GetType())) {
				return true;
			}
			seenActionTypes.Add(current.GetType());
			current = current.ParentAction;
		}
		return false;
	}

	public void DebugLogQueue() {
		StringBuilder sb = new();

		sb.AppendLine("Model Action Queue:");
		foreach (ModelAction action in _actionQueue) {
			BuildActionString(sb, 0, action);
		}
		sb.Append('\n');
		GD.Print(sb.ToString());
	}

	private StringBuilder BuildActionString(StringBuilder sb, int depth, ModelAction action) {
		sb.Append(action.ToString());
		foreach (ModelAction subaction in action.SubActions) {
			sb.Append(Enumerable.Repeat("-", depth));
			sb.Append(subaction.ToString());
			sb = BuildActionString(sb, depth + 1, subaction);
			sb.Append('\n');
		}
		return sb;
	}
}
