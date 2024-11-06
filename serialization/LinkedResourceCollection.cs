namespace Jodot.Serialization;
using System.Collections.Generic;
using System.Reflection;
using Jodot.Content.Libraries;
using Jodot.Injection;


[System.AttributeUsage(System.AttributeTargets.Field)]
public abstract class LinkedResourceCollectionBase: System.Attribute {
    public abstract void LinkTo(object o, IServiceContext s, FieldInfo field);
}

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LinkedResourceCollection<C, T>: LinkedResourceCollectionBase
    where C : System.Collections.IList, new()
{
    public string ResourceLibraryKey;
    public string ResourceKeyField;


    public LinkedResourceCollection(string resourceLibraryKey, string resourceKeyField) {
        ResourceLibraryKey = resourceLibraryKey;
        ResourceKeyField = resourceKeyField;
    }

    public override void LinkTo(object o, IServiceContext s, FieldInfo field) {
        List<int> indices = (List<int>)GetFieldOfName(o, ResourceKeyField).GetValue(o);
        C collection = new();
        IContentLibrary library = s.GetService(ResourceLibraryKey);

        foreach (int i in indices) {
            collection.Add(library.Get(i));
        }
        field.SetValue(o, collection);
        
    }

    public FieldInfo GetFieldOfName(object o, string name) {
		return o.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

}