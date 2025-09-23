using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapContainer : MonoBehaviour {
    private const string ID_lastMapKeys = "lastMapKeys";
    private const string ID_lastLevelGenerated = "lastLevelGenerated";
    private static MapContainer Instance;
	public static Map currentMap { get => Instance._currentMap; }
	[SerializeField] private Map _currentMap = null;

	[SerializeField, Header("Endless")] private Vector2Int _endlessLevels = new Vector2Int(0, 100);
	[SerializeField] private int _levelNoRepeatAmount = 3;
	[SerializeField, Header("Debug")] private bool _autoInit = false;
	[SerializeField] private int _autoInitIndex = 0;

	private void Awake() {
        Instance = this;
	}

	private void Start() {
		if (this._autoInit) this.InitMapDebug(this._autoInitIndex);
	}

	public void InitMapDebug(int index) {
		DOVirtual.DelayedCall(0.1f, () => { currentMap.InitMap(index, index); });
	}
	public static bool InitMap() {
		if (SaveDataJsonInterface.Exist<List<int>>(ID_lastMapKeys) == false) SaveDataJsonInterface.SetObject(ID_lastMapKeys, new List<int>());
		if (SaveDataJsonInterface.Exist<int>(ID_lastLevelGenerated) == false) SaveDataJsonInterface.SetInt(ID_lastLevelGenerated, -1);

		int index = LevelContainer.GetLevelIndex();
		List<int> lastMapKeys = SaveDataJsonInterface.GetObject<List<int>>(ID_lastMapKeys);
		TutorialPanel tutorialPanel = UIContainer.GetPanel(TypeMenu.Tutorial) as TutorialPanel;

		int levelIndex = Instance.GetMapIndex(index, lastMapKeys);
		currentMap.InitMap(levelIndex, index);

		lastMapKeys.Add(levelIndex);
		if (lastMapKeys.Count == (Instance._levelNoRepeatAmount + 1)) lastMapKeys.RemoveAt(0);
		SaveDataJsonInterface.SetObject(ID_lastMapKeys, lastMapKeys);
		SaveDataJsonInterface.SetInt(ID_lastLevelGenerated, index);

		return true;
	}

	private int GetMapIndex(int index, List<int> lastMapKeys) {
		int lastLevelGenerated = SaveDataJsonInterface.GetInt(ID_lastLevelGenerated);
		if (index <= this._endlessLevels.y) return index;
		if (index == lastLevelGenerated && lastMapKeys != null && lastMapKeys.Count > 0) return lastMapKeys.Last();

		System.Random mRandom = new System.Random(index);

		int[] allLevels = Enumerable.Range(this._endlessLevels.x, this._endlessLevels.y - this._endlessLevels.x + 1).ToArray();
		List<int> levelToRemove = new List<int>();

		levelToRemove = lastMapKeys.Distinct().ToList();
		allLevels = allLevels.Except(levelToRemove).ToArray();
		int result = allLevels[mRandom.Next(0, allLevels.Length)];

		return result;
	}
}
