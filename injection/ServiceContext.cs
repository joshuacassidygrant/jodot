namespace Jodot.Injection;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jodot.Model;
using Jodot.Rendering;
using Jodot.Events;
using Jodot.Content.Libraries;


public partial class ServiceContext : Node, IServiceContext
{
	[Injectable("Events")] public IEventBus Events;
	[Injectable("ModelRunner")] public ModelRunner ModelRunner;
	[Injectable("ModelRendererContainer")] public ModelRendererContainer ModelRendererContainer;
	
	public Model Model => ModelRunner.Model;


	private Dictionary<string, FieldInfo> _injectableFields;

	private List<IInjectSubject> _queuedInjectSubjects = new();

	public virtual void SetupContentServices() {}

	public virtual void SetupFrameworkServices() {

		Events events = new();
		GetTree().Root.CallDeferred("add_child", events);
		Events = events;
		ModelRunner = new ModelRunner(this, Events);

		ModelRendererContainer = GetNode<ModelRendererContainer>("/root/ModelRendererContainer");
	}

	public override void _Ready()
	{
		SetupFrameworkServices();
		SetupContentServices();

		BindDependencies();
		Events.EmitFrom("ServiceDirectoryInitialized");
	
		_queuedInjectSubjects.ForEach((IInjectSubject o) => {
			InjectDependencies(o);
		});
	}

	public IContentLibrary GetLibrary(string injectableName) {
		if (_injectableFields.ContainsKey(injectableName))
		{
			IContentLibrary lib = _injectableFields[injectableName].GetValue(this) as IContentLibrary;
			if (lib != null) return lib;
			return null;
		} else
		{
			GD.PrintErr("Can't find service with name " + injectableName);
			return null;
		}	
	}

	public void BindDependencies() {
		// Initialize _injectableFields 
		Type t = GetType();
		FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(Injectable), false).Any()).ToArray();

		_injectableFields = new();
		foreach (FieldInfo fieldInfo in fields) {
			object[] atts = fieldInfo.GetCustomAttributes(typeof(Injectable), false);
			string name = ((Injectable)atts[0]).Name;

			_injectableFields.Add(name, fieldInfo);
		}

		// Trigger inject on all injectables
		foreach (FieldInfo fieldInfo in fields) {
			InjectDependencies(fieldInfo.GetValue(this));
		}
	}

	public virtual void InjectDependencies(object o) {
		if (o == null) {
			throw new Exception("Tried to inject object but it is null!");
		}
		IInjectSubject iis = o as IInjectSubject;
		if (_injectableFields == null) {
            // If not initialized, add to queue.
            if (iis != null)
            {
                _queuedInjectSubjects.Add(iis);
            }
            return;
		}
		Type t = o.GetType();
		FieldInfo[] injectFields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(Inject), false).Any()).ToArray();
		foreach (FieldInfo fieldInfo in injectFields) {
			object[] atts = fieldInfo.GetCustomAttributes(typeof(Inject), false);

			string name = ((Inject)atts[0]).Name;
			if (name == null) name = fieldInfo.FieldType.Name;
			
			if (_injectableFields.ContainsKey(name))
			{
				try {
					fieldInfo.SetValue(o, _injectableFields[name].GetValue(this));
				} catch (Exception e) {
					GD.PrintErr($"Can't bind {name}!");
					GD.PrintErr(e);
				}
			} else
			{
				GD.PrintErr("Can't find service with name " + name + " to bind to " + t.Name);
			}
		}

		iis?.OnPostInject();
	}

    public dynamic GetService(string name)
    {
        return _injectableFields[name].GetValue(this);
    }

}
