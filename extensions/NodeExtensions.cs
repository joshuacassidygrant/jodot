namespace Jodot.Extensions;

using Godot;
using Jodot.Injection;


public static class NodeExtensions
{
	public static void InjectServiceContext(this Node node)
	{
		node.GetNode<ServiceContext>("/root/ServiceContext").InjectDependencies(node);
	}

	public static void ClearChildren(this Node node) {
		foreach(Node child in node.GetChildren()) {
			child.QueueFree();
		}
	}
}
	

