namespace Jodot.Model; 

using Godot;
using System;
using Nito.Collections;
using System.Text;
using System.Collections.Generic;
using Jodot.Injection;
using Jodot.Events;

public partial class ModelRunner : IActionSource
{
	public Model Model;

	// SERVICES
	public IEventBus _events;
	protected IServiceContext s;

	private Deque<ModelAction> _actionQueue = new();

	public void BindEvents(IEventBus events) {
		_events = events;
	}

	public ModelRunner() {}

	public ModelRunner(IServiceContext s, IEventBus events) {
		this.s = s;
		BindEvents(events);
	}

	public virtual void NewModel() {
		Model = new Model(s);
	}

	public void QueueAction(ModelAction action, IActionSource source)
	{
		_actionQueue.AddToBack(action);
		RunQueue();
	}

	public void QueueActions(List<ModelAction> actions, IActionSource source) {
		foreach (ModelAction action in actions) {
			_actionQueue.AddToBack(action);
		}
		RunQueue();
	}

	public void PushChildAction(ModelAction action, ModelAction parentAction) {
		if (action.Complexity >= parentAction.Complexity) {
			throw new Exception($"Illegal action parenting between {action} and {parentAction}. {action} must be must have less complexity than {parentAction} ");
		}
		_actionQueue.AddToFront(action);
		RunQueue();
	}

	public void PushChildActions(List<ModelAction> actions, ModelAction parentAction) {		
		foreach (ModelAction action in actions) {
			if (action.Complexity >= parentAction.Complexity) {
				throw new Exception($"Illegal action parenting between {action} and {parentAction}.  {action} must be must have less complexity than {parentAction} ");
			}
			_actionQueue.AddToFront(action);
		}

		RunQueue();
	}

	public void RunQueue() {	
		if (_actionQueue.Count == 0) return;
		//DebugLogQueue();

		ModelAction action = _actionQueue.RemoveFromFront();
		if (action.CanDo(Model)) {
			action.Do(Model);
		}
		RunQueue();
	}

	public void DebugLogQueue() {
		StringBuilder sb = new();
		sb.AppendLine("Model Action Queue:");
		foreach (ModelAction action in _actionQueue) {
			sb.AppendLine(action.ToString());
		}
		GD.Print(sb.ToString());
	}
}
