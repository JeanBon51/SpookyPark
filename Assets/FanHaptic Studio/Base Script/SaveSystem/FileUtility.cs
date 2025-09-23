using System.Collections.Generic;
using System.IO;

public static class FileUtility {

	/// <summary>
	/// create a directory path inside the build folder
	/// </summary>
	/// <param name="directory">relative path to the directory</param>
	/// <returns>absolute path to directory</returns>
	public static string CreateWorkDir(string directory = "") {
		string workDir = directory == "" ? PathManager.GetMainDirectoryPath() : PathManager.GetMainDirectoryPath() + PathManager.pathSeparator;
		if (Directory.Exists(workDir + directory))
			return workDir + directory;

		string[] elems = directory.Split(new char[] { '/', '\\' });
		string currentPath = "";

		foreach (string elem in elems) {
			currentPath += $"{PathManager.pathSeparator}{elem}";
			if (Directory.Exists(workDir + elem) == false) {
				Directory.CreateDirectory(workDir + currentPath);
			}
		}
		return workDir + currentPath;
	}

	/// <summary>
	/// create a directory path
	/// </summary>
	/// <param name="path">absolute path to the directory</param>
	/// <returns>absolute path to the directory</returns>
	public static string CreateDir(string path) {
		string currentPath = "";
		List<string> elems = new List<string>(path.Split(new char[] { '/', '\\' }));
		elems.RemoveAll(e => string.IsNullOrWhiteSpace(e));

#if UNITY_EDITOR_WIN || UNITY_ANDROID
		currentPath = elems[0];
#elif UNITY_EDITOR_OSX || UNITY_IOS
		currentPath = PathManager.pathSeparator + elems[0];
#endif

		for (int i = 1; i < elems.Count; i++) {
			if (elems[i].Contains(".")) {
				break;
			}
			currentPath += $"{PathManager.pathSeparator}{elems[i]}";
			if (Directory.Exists(currentPath) == false) {
				Directory.CreateDirectory(currentPath);
			}
		}
		return currentPath;
	}
}
