using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonCustomWrapper {

	public static JToken GetJToken<T>(JObject jsonFile, string key) {
		string formatedKey = key + GetKeySuffixByType<T>();
		if (jsonFile != null && jsonFile.ContainsKey(formatedKey)) 
			return jsonFile.SelectToken($"$.{formatedKey}");
		else 
			return null;
	}

	/// <summary>
	/// Methode to add a keyValuePair to the save file (json)
	/// </summary>
	/// <param name="localPath">path to pair in json, ex: if you want to add entry "level" in "account" object => "account/level" </param>
	/// <param name="token">value</param>
	public static void addKeyJTokenPairToJson<T>(string filePath, JObject jsonFile, string key, T value) {
		string s = key + GetKeySuffixByType<T>(value);
		jsonFile[s] = JToken.Parse(JsonConvert.SerializeObject(value));
		File.WriteAllText(filePath, jsonFile.ToString());
	}

	public static void addKeyJTokenPairToPlayerPrefs<T>(string PlayerPrefsKey, JObject jsonFile, string key, T value) {
		string s = key + GetKeySuffixByType<T>(value);
		jsonFile[s] = JToken.Parse(JsonConvert.SerializeObject(value));
		PlayerPrefs.SetString(PlayerPrefsKey, jsonFile.ToString());
	}

	public static void RemoveFromJson<T>(string filePath, JObject jsonFile, string key) {
		string formatedKey = key + GetKeySuffixByType<T>();

		if (jsonFile == null || !jsonFile.ContainsKey(formatedKey)) {
			Debug.LogWarning($"key ({key}) does not exist in this context");
			return;
		}

		jsonFile.Remove(formatedKey);
		File.WriteAllText(filePath, jsonFile.ToString());
	}
	public static void RemoveFromJson<T>(string filePath, JObject jsonFile, string key, T value) {
		string formatedKey = key + GetKeySuffixByType<T>(value);

		if (jsonFile == null || !jsonFile.ContainsKey(formatedKey)) {
			Debug.LogWarning($"key ({key}) does not exist in this context");
			return;
		}

		jsonFile.Remove(formatedKey);
		File.WriteAllText(filePath, jsonFile.ToString());
	}

	public static void RemoveFromPlayerPrefs<T>(string PlayerPrefsKey, JObject jsonFile, string key) {
		string formatedKey = key + GetKeySuffixByType<T>();

		if (jsonFile == null || !jsonFile.ContainsKey(formatedKey)) {
			Debug.LogWarning($"key ({key}) does not exist in this context");
			return;
		}

		jsonFile.Remove(formatedKey);
		PlayerPrefs.SetString(PlayerPrefsKey, jsonFile.ToString());
	}
	public static void RemoveFromPlayerPrefs<T>(string PlayerPrefsKey, JObject jsonFile, string key, T value) {
		string formatedKey = key + GetKeySuffixByType<T>(value);

		if (jsonFile == null || !jsonFile.ContainsKey(formatedKey)) {
			Debug.LogWarning($"key ({key}) does not exist in this context");
			return;
		}

		jsonFile.Remove(formatedKey);
		PlayerPrefs.SetString(PlayerPrefsKey, jsonFile.ToString());
	}

	public static bool Exist<T>(JObject jsonFile, string key) {
		if (jsonFile == null) {
			Debug.LogWarning($"key ({key}) does not exist in this context");
			return false;
		}
		string formatedkey = key + GetKeySuffixByType<T>();
		return jsonFile.ContainsKey(formatedkey);
	}

	public static void SetString(string PathOrKey, JObject jsonFile, string key, string value) {
#if UNITY_WEBGL
		addKeyJTokenPairToPlayerPrefs<string>(PathOrKey, jsonFile, key, value);
#else
		addKeyJTokenPairToJson<string>(PathOrKey, jsonFile, key, value);
#endif
	}
	public static void SetInt(string PathOrKey, JObject jsonFile, string key, int value) {
#if UNITY_WEBGL
		addKeyJTokenPairToPlayerPrefs<int>(PathOrKey, jsonFile, key, value);
#else
		addKeyJTokenPairToJson<int>(PathOrKey, jsonFile, key, value);
#endif
	}
	public static void SetBigInteger(string PathOrKey, JObject jsonFile, string key, BigInteger value) {
#if UNITY_WEBGL
		addKeyJTokenPairToPlayerPrefs<BigInteger>(PathOrKey, jsonFile, key, value);
#else
		addKeyJTokenPairToJson<BigInteger>(PathOrKey, jsonFile, key, value);
#endif
	}
	public static void SetFloat(string PathOrKey, JObject jsonFile, string key, float value) {
#if UNITY_WEBGL
		addKeyJTokenPairToPlayerPrefs<float>(PathOrKey, jsonFile, key, value);
#else
		addKeyJTokenPairToJson<float>(PathOrKey, jsonFile, key, value);
#endif
	}
	public static void SetBool(string PathOrKey, JObject jsonFile, string key, bool value) {
#if UNITY_WEBGL
		addKeyJTokenPairToPlayerPrefs<bool>(PathOrKey, jsonFile, key, value);
#else
		addKeyJTokenPairToJson<bool>(PathOrKey, jsonFile, key, value);
#endif
	}
	public static void SetObject<T>(string PathOrKey, JObject jsonFile, string key, T value) {
#if UNITY_WEBGL
		addKeyJTokenPairToPlayerPrefs<T>(PathOrKey, jsonFile, key, value);
#else
		addKeyJTokenPairToJson<T>(PathOrKey, jsonFile, key, value);
#endif
	}

	public static string GetString(JObject jsonFile, string key) {
		JToken token = GetJToken<string>(jsonFile, key);
		return token != null ? token.ToObject<string>() : "STRING.NULL";
	}
	public static int GetInt(JObject jsonFile, string key) {
		JToken token = GetJToken<int>(jsonFile, key);
		return token != null ? token.ToObject<int>() : -1;
	}
	public static BigInteger GetBigInteger(JObject jsonFile, string key) {
		JToken token = GetJToken<BigInteger>(jsonFile, key);
		return token != null ? token.ToObject<BigInteger>() : -1;
	}
	public static float GetFloat(JObject jsonFile, string key) {
		JToken token = GetJToken<float>(jsonFile, key);
		return token != null ? token.ToObject<float>() : -1.0f;
	}
	public static bool GetBool(JObject jsonFile, string key) {
		JToken token = GetJToken<bool>(jsonFile, key);
		return token != null ? token.ToObject<bool>() : false;
	}
	public static T GetObject<T>(JObject jsonFile, string key) {
		return GetJToken<T>(jsonFile, key).ToObject<T>();
	}

	private static string GetKeySuffixByType<T>(T instance){
		Type type = instance.GetType();
		return TypeToString(type);
	}

	private static string GetKeySuffixByType<T>() {
		Type type = typeof(T);
		return TypeToString(type);
	}

	private static string TypeToString(Type type) {
		bool isSimpleType = type.IsPrimitive || type.Equals(typeof(string));
		if (type == typeof(float)) return "_f";
		else if (type == typeof(BigInteger)) return "_bi";
		else if (isSimpleType) return "_" + type.Name.ToLower()[0];
		else return "_" + type.Name;
	}
}

public class JsonInternalPath {
	public string[] directories;
	public string key;

	public JsonInternalPath(string jsonLocalPath) {
		string[] dirs = jsonLocalPath.Split('/');
		this.key = dirs.Last();
		this.directories = new string[dirs.Length - 1];
		for (int i = 0; i < dirs.Length - 1; i++) {
			this.directories[i] = dirs[i];
		}
	}
}