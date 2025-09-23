using Newtonsoft.Json.Linq;
using UnityEngine;

public static class MapDataJsonInterface {
	private static JObject _jsonSaveFile = null;

	private static void InitJson() {
		NewtonSoftConverterInitializer.SetUpJsonConverterSettings();
		GetJson();
	}

	private static void GetJson() {
        object obj = Resources.Load("MapData");
		if(obj == null) {
			_jsonSaveFile = new JObject();
		}
		else {
			TextAsset tmp = obj as TextAsset;
			_jsonSaveFile = (JObject)JToken.Parse(tmp.text);
		}
	}

	public static void SetMap(MapData data) {
		InitJson();
		JsonCustomWrapper.SetObject(PathManager.GetMapFilePath(), _jsonSaveFile, data.id, data);
	}

	//public static void SetMaps(NewMapData[] dataArr) {
	//	InitJson();
	//	for (int i = 0; i < dataArr.Length; i++) {
	//		JsonCustomWrapper.SetObject(PathManager.GetMapFilePath(), _jsonSaveFile, dataArr[i].id, dataArr[i]);
	//	}
	//}

	public static MapData GetMap(string index) {
		InitJson();
		JToken token = JsonCustomWrapper.GetJToken<MapData>(_jsonSaveFile, index);
		if (token == null) return null;
		return token.ToObject<MapData>();
	}

	//public static NewMapData GetNewMap(string index) {
	//	InitJson();
	//	JToken token = JsonCustomWrapper.GetJToken<NewMapData>(_jsonSaveFile, index);
	//	if (token == null) return null;
	//	return token.ToObject<NewMapData>();
	//}

	public static bool MapExist(string id) {
		InitJson();
		return JsonCustomWrapper.Exist<MapData>(_jsonSaveFile, id);
	}

	public static JObject GetJsonFileAsJobject() {
		InitJson();
		return _jsonSaveFile;
	}

	public static void Remove(string id) {
		InitJson();
		JsonCustomWrapper.RemoveFromJson<MapData>(PathManager.GetMapFilePath(), _jsonSaveFile, id);
	}

	public static void ClearLocalCReference() {
		_jsonSaveFile = null;
	}
}
