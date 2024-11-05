namespace Jodot.Model;

using System;
using Jodot.Injection;

public interface IModelInfo {
    public int Version {get;}
    public int ComponentTypeCount {get;}
    public string[] ComponentTypeNames {get;}
    public string ComponentTypeName(int type);
    public Func<IServiceContext, Component> GetComponentGenerator(int type);
}