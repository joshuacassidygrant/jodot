namespace Jodot.Injection;

using System;
using Colony.Scripts.Model.GameModel;

public interface IServiceContext {
    public void InjectDependencies(Object o);
    public Model Model { get; }

    public dynamic GetService(string name);
}