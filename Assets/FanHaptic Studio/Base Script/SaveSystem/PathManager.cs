using System;
using System.IO;
using UnityEngine;

public static class PathManager {
	public static char pathSeparator {
		get {
#if UNITY_EDITOR_WIN || UNITY_ANDROID
			return '/';
#elif UNITY_EDITOR_OSX || UNITY_IOS
			return Path.DirectorySeparatorChar; 
#endif
			return '/';
		}
	}

	public static string GetMainDirectoryPath() => Application.persistentDataPath + pathSeparator;

	public static string GetJsonDirectoryPath() {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
		return Application.persistentDataPath + pathSeparator + "DataJson";
#elif UNITY_ANDROID || UNITY_IOS
		return Application.persistentDataPath;
#endif
		return "";
	}

	public static string GetSaveFilePath(string PlayerPrefKey = "") {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
		return GetJsonDirectoryPath() + pathSeparator + "SaveData.json";
#elif UNITY_ANDROID || UNITY_IOS
		return Application.persistentDataPath + pathSeparator+ "SaveData.json";
#elif UNITY_WEBGL
	return PlayerPrefKey;
#endif
	}

	public static string GetRessourcesDirectory() => Application.dataPath + pathSeparator + "Resources";

	public static string GetMapFilePath() {
#if UNITY_WEBGL
		return PlayerPrefKey;
#else
		return GetRessourcesDirectory() + pathSeparator + "MapData.json";
#endif
	}

	public static string FormatPath(string pathToFormat) => String.Join(Path.DirectorySeparatorChar.ToString(), pathToFormat.Split(new char[] { '\\', '/' }));
}
