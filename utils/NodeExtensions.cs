using Godot;
using Jodot.Injection;

namespace NodeExtensions
{

    public static class NodeExtensions
	{
		public static void InjectServiceContext(this Node node)
		{
			node.GetNode<ServiceDirectory>("/root/ServiceContext").InjectDependencies(node);
		}
	
		public static void ClearChildren(this Node node) {
			foreach(Node child in node.GetChildren()) {
				child.QueueFree();
			}
		}
	}
	
}
