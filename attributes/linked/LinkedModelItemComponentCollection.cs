namespace Jodot.Attributes.Linked;

using System;
using System.Collections.Generic;
using System.Reflection;
using Jodot.Injection;
using Jodot.Model;

[System.AttributeUsage(System.AttributeTargets.Field)]
public abstract class LinkedModelItemComponentCollectionBase : Attribute {
    public abstract void LinkTo(object o, IServiceContext s, FieldInfo field);
}

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LinkedModelItemComponentCollection<C, T>: LinkedModelItemComponentCollectionBase
    where C : System.Collections.IList, new()
    where T: Component
{
    public string IndicesField;


    public LinkedModelItemComponentCollection(string indicesField) {
        IndicesField = indicesField;
    }

    public override void LinkTo(object o, IServiceContext s, FieldInfo field) {
        List<int> indices = (List<int>)GetFieldOfName(o, IndicesField).GetValue(o);
        C collection = new();

        foreach (int i in indices) {
            collection.Add(s.Model.GetComponentOrNull<T>(i));
        }
        field.SetValue(o, collection);
        
    }

    public FieldInfo GetFieldOfName(object o, string name) {
		return o.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

}