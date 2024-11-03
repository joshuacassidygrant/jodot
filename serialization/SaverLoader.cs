namespace Jodot.Serialization;

using Godot;
using Colony.Scripts.Model.Core;
using Jodot.Injection;
using Jodot.Events;


public partial class SaverLoader: IInjectSubject
{
	[Inject("Events")] private IEventBus _events;
	[Inject("ModelRunner")] private GameModelRunner _modelRunner;

	public void SaveModel(string fileName) {
		if (!IsLegalFileName(fileName)) return;

		string jsonString = Json.Stringify(_modelRunner.GameModel.ExportData());
		
		using var file = Godot.FileAccess.Open($"user://{fileName}.json", Godot.FileAccess.ModeFlags.Write);
		file.StoreString(jsonString);
		file.Close();
	}

	public void LoadModel(string fileName) {
		if (!IsLegalFileName(fileName)) return;

		using var file = Godot.FileAccess.Open($"user://{fileName}.json", Godot.FileAccess.ModeFlags.Read);
		string text = file.GetAsText();
		Godot.Collections.Dictionary<string, Variant> data = (Godot.Collections.Dictionary<string, Variant>)Json.ParseString(text);
		_modelRunner.GameModel.ImportData(data);

		GD.Print("todo");
	}

	public bool IsLegalFileName(string fileName) {
		return !(fileName.Length == 0 || fileName.Length > 50);
	}

    public void OnPostInject()
    {
		_events.ConnectTo("RequestGameSave", Callable.From((string fileName) => {
			SaveModel(fileName);
		}));

		_events.ConnectTo("RequestGameLoad", Callable.From((string fileName) => {
			LoadModel(fileName);
		}));

		_events.ConnectTo("RequestDataTest", Callable.From(() => {
			SaveModel("data-test");
			LoadModel("data-test");
			SaveModel("data-test2");

			using var file1 = Godot.FileAccess.Open($"user://data-test.json", Godot.FileAccess.ModeFlags.Read);
			using var file2 = Godot.FileAccess.Open($"user://data-test2.json", Godot.FileAccess.ModeFlags.Read);
			string text1 = file1.GetAsText();
			string text2 = file2.GetAsText();

			// TODO: make this more useful
			if (text1 != text2) {
				GD.PrintErr("Not equal");
			}

		}));
    }

}
