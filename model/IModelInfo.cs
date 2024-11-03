using System;
using Jodot.Injection;

namespace Jodot.Model;

public interface IModelInfo {
    public int Version {get;}
    public int ComponentTypeCount {get;}
    public string[] ComponentTypeNames {get;}
    public string ComponentTypeName(int type);
    public Func<IServiceContext, ModelItemComponent> GetComponentGenerator(int type);
}