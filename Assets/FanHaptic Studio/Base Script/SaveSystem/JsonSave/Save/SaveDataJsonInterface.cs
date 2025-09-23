using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Numerics;
using UnityEngine;

public static class SaveDataJsonInterface {
	private const string PLAYER_PREFS_KEY = "SaveDatat";
	private static JObject _jsonSaveFile = null;

	private static void InitJson() {
		NewtonSoftConverterInitializer.SetUpJsonConverterSettings();
		RecoverSave(true);
	}

	private static void RecoverSave(bool TryOnline) {
		if (!(TryOnline && GetOnlineSave())) GetLocalJson();
#if UNITY_WEBGL && !UNITY_EDITOR_WIN
		if (_jsonSaveFile == null)  _jsonSaveFile = JObject.Parse(PlayerPrefs.GetString(PLAYER_PREFS_KEY));
#else
		if (_jsonSaveFile == null) {
			using (StreamReader file = File.OpenText(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY))) {
				JsonSerializer serializer = new JsonSerializer();
				_jsonSaveFile = (JObject)serializer.Deserialize(file, typeof(JObject));
			}
		}
#endif
	}

	private static bool GetOnlineSave() {
		return false; 
	}

	private static void GetLocalJson() {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
		if (!File.Exists(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY))) {
			FileUtility.CreateDir(PathManager.GetJsonDirectoryPath());
			File.WriteAllText(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), "{\n}");
		}
#elif UNITY_ANDROID || UNITY_IOS
		if (!File.Exists(PathManager.GetSaveFilePath())) {
			File.WriteAllText(PathManager.GetSaveFilePath(), "{\n}");
		}
#elif UNITY_WEBGL
		string request = PlayerPrefs.GetString(PLAYER_PREFS_KEY, "null");
		if (request == "null") PlayerPrefs.SetString(PLAYER_PREFS_KEY, "{}");
#endif
	}

	public static void SetString(string jsonLocalPath, string value) {
		InitJson();
		JsonCustomWrapper.SetString(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}
	public static void SetInt(string jsonLocalPath, int value) {
		InitJson();
		JsonCustomWrapper.SetInt(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}
	public static void SetBigInteger(string jsonLocalPath, BigInteger value) {
		InitJson();
		JsonCustomWrapper.SetBigInteger(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}
	public static void SetFloat(string jsonLocalPath, float value) {
		InitJson();
		JsonCustomWrapper.SetFloat(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}
	public static void SetBool(string jsonLocalPath, bool value) {
		InitJson();
		JsonCustomWrapper.SetBool(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}
	public static void SetObject<T>(string jsonLocalPath, T value) {
		InitJson();
		JsonCustomWrapper.SetObject(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}

	public static string GetString(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.GetString(_jsonSaveFile, jsonLocalPath);
	}
	public static int GetInt(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.GetInt(_jsonSaveFile, jsonLocalPath);
	}
	public static BigInteger GetBigInteger(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.GetBigInteger(_jsonSaveFile, jsonLocalPath);
	}
	public static float GetFloat(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.GetFloat(_jsonSaveFile, jsonLocalPath);
	} 
	public static bool GetBool(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.GetBool(_jsonSaveFile, jsonLocalPath);
	}
	public static T GetObject<T>(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.GetObject<T>(_jsonSaveFile, jsonLocalPath);
	} 

	public static JObject GetJsonFileAsJobject() {
		InitJson();
		return _jsonSaveFile;
	}

	public static bool Exist<T>(string jsonLocalPath) {
		InitJson();
		return JsonCustomWrapper.Exist<T>(_jsonSaveFile, jsonLocalPath);
	}

	public static void Remove<T>(string jsonLocalPath) {
		InitJson();
		JsonCustomWrapper.RemoveFromJson<T>(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath);
	}
	public static void Remove<T>(string jsonLocalPath, T value) {
		InitJson();
		JsonCustomWrapper.RemoveFromJson<T>(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY), _jsonSaveFile, jsonLocalPath, value);
	}

	public static void ClearLocalJsonReference() {
		_jsonSaveFile = null;
	}

	public static void DeleteJson() {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
		if (File.Exists(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY))){
			FileUtility.CreateDir(PathManager.GetJsonDirectoryPath());
			File.Delete(PathManager.GetSaveFilePath(PLAYER_PREFS_KEY));
		}
		ClearLocalJsonReference();
#elif UNITY_ANDROID || UNITY_IOS || !UNITY_EDITOR
		if (File.Exists(PathManager.GetSaveFilePath())) File.Delete(PathManager.GetSaveFilePath());
		ClearLocalJsonReference();
#elif UNITY_WEBGL
		PlayerPrefs.SetString(PLAYER_PREFS_KEY, "null");
		ClearLocalJsonReference();
#endif
	}

}