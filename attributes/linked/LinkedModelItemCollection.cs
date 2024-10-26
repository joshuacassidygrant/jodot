namespace Jodot.Attributes.Linked;

using System;
using System.Collections.Generic;
using System.Reflection;
using Colony.Scripts.Infra;
using Colony.Scripts.Model.Core;

[System.AttributeUsage(System.AttributeTargets.Field)]
public abstract class LinkedModelItemCollectionBase : Attribute {
    public abstract void LinkTo(object o, IServiceContext s, FieldInfo field);
}

[System.AttributeUsage(System.AttributeTargets.Field)]
public class LinkedModelItemCollection<C, T>: LinkedModelItemCollectionBase
    where C : System.Collections.IList, new()
    where T: ModelItem
{
    public string IndicesField;


    public LinkedModelItemCollection(string indicesField) {
        IndicesField = indicesField;
    }

    public override void LinkTo(object o, IServiceContext s, FieldInfo field) {
        List<int> indices = (List<int>)GetFieldOfName(o, IndicesField).GetValue(o);
        C collection = new();

        foreach (int i in indices) {
            collection.Add(s.Model.GetModelItemOrNull<T>(i));
        }
        field.SetValue(o, collection);
        
    }

    public FieldInfo GetFieldOfName(object o, string name) {
		return o.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
	}

}