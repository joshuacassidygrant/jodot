namespace Jodot.Content.Resources;

using System.Collections.Generic;

public interface IContentModelItemResource 
{

	public List<ModelItemComponentResource> GetComponentResources();

	public string GetName();
}
