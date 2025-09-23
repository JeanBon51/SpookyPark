using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;


#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[RequireComponent(typeof(Canvas)),RequireComponent(typeof(CanvasGroup)),RequireComponent(typeof(CanvasScaler)),RequireComponent(typeof(GraphicRaycaster))]
public class LevelContainer : MonoBehaviour {
    private const string ID_IndexLevel_BaseMode = "IndexLevelInfo";
    public static UnityEvent<int> onLevelUp = new UnityEvent<int>();
    public static void AddLevelIndex() {
	    SaveDataJsonInterface.SetInt(ID_IndexLevel_BaseMode,SaveDataJsonInterface.Exist<int>(ID_IndexLevel_BaseMode)?SaveDataJsonInterface.GetInt(ID_IndexLevel_BaseMode)+1:1);
    }
    public static void SetLevelIndex(int value) {
	    SaveDataJsonInterface.SetInt(ID_IndexLevel_BaseMode,value);
    }
    public static int GetLevelIndex() {
	    return SaveDataJsonInterface.Exist<int>(ID_IndexLevel_BaseMode) ? SaveDataJsonInterface.GetInt(ID_IndexLevel_BaseMode) : 0; 
    }
	public enum TypeLoading {
		OnlyOneScene,
		MultiScene,
	}

	[System.Serializable]
	public class SceneSetting {
		public int levelIndex = 0;
		public SceneReference scene;
#if UNITY_EDITOR
		[Button]
		public void AddSceneBuildSetting() {
			List<EditorBuildSettingsScene> tempo = EditorBuildSettings.scenes.ToList();
			if (tempo.Find(item => item.path == this.scene.Path) != null) return;
			tempo.Add(new EditorBuildSettingsScene(this.scene.Path, true));
			EditorBuildSettings.scenes = tempo.ToArray();
		}

		[Button]
		public void LoadScene() {
			EditorApplication.SaveCurrentSceneIfUserWantsTo();
			EditorApplication.OpenScene(this.scene.Path);
		}
#endif
	}

	public static UnityEvent onLoadComplete = new UnityEvent();
	public static UnityEvent onUnloadComplete = new UnityEvent();

	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private RectTransform _iCircle;
	[SerializeField] private RectTransform _iBlockInput;
	[SerializeField] private TypeLoading _typeLoading;

	[SerializeField, ShowIf("_typeLoading", TypeLoading.OnlyOneScene)]
	public SceneReference _sceneGame;

	[SerializeField, TableList, ShowIf("_typeLoading", TypeLoading.MultiScene)]
	private SceneSetting[] _sceneSettingArray;

	private Coroutine _sceneRoutine;
	public int currentSceneIndex { get; private set; } = -1;

	private bool _isLoadingUIActive => this._canvasGroup.alpha == 1;
	
	[Button]
	public void UpdateUI(bool startLoad) {
		if (startLoad) {
			this._canvasGroup.blocksRaycasts = true;
			this._canvasGroup.alpha = 1;
			this._iBlockInput.gameObject.SetActive(true);
			this._iCircle.DOSizeDelta(Vector2.zero, 0.35f);
		} else {
			this._iCircle.DOSizeDelta(Vector2.one * 3500f, 0.35f).OnComplete((() =>
			{
				this._canvasGroup.alpha = 0;
				this._canvasGroup.blocksRaycasts = false;
				this._iBlockInput.gameObject.SetActive(false);
			}));
		}
	}
	[Button]
	public void Enable(bool enable)
	{
		if (enable) {
			this._canvasGroup.blocksRaycasts = true;
			this._canvasGroup.alpha = 1;
			this._iBlockInput.gameObject.SetActive(true);
			this._iCircle.sizeDelta = Vector2.zero;
		} else {
			this._canvasGroup.alpha = 0;
			this._canvasGroup.blocksRaycasts = false;
			this._iBlockInput.gameObject.SetActive(false);
			this._iCircle.sizeDelta = Vector2.one * 3500f;
		}
	}

    [Button]
    public void LoadGameScene()
    {
        if (this._typeLoading == TypeLoading.OnlyOneScene)
        {
            this.LoadScene(this._sceneGame);
        }
        else
        {
            if (this.CanLoadNextLevel(out SceneReference sceneR) == false)
            {
                Debug.LogError($"Missing Scene for level index : {GetLevelIndex()}");
                return;
            }
            this.LoadScene(sceneR);
        }
    }

    public bool CanLoadNextLevel() => this.CanLoadNextLevel(out SceneReference s);
    public bool CanLoadNextLevel(out SceneReference sceneReference)
    {
        if (this._typeLoading == TypeLoading.OnlyOneScene)
        {
            sceneReference = this._sceneGame;
            return true;
        }

        SceneSetting sc = Array.Find(this._sceneSettingArray, item => item.levelIndex == GetLevelIndex());
        sceneReference = sc != null ? sc.scene : null;
        return sceneReference != null;
	}

	private void LoadScene(SceneReference sceneReference) {
		if (this._sceneRoutine == null)
			this._sceneRoutine =
				this.StartCoroutine(this.LoadSceneRoutine(sceneReference, ThreadPriority.High, LoadSceneMode.Additive));
	}

	public void UnloadScene() {
		if (this._sceneRoutine == null)
			this._sceneRoutine = this.StartCoroutine(this.UnloadSceneRoutine(ThreadPriority.High));
	}

	IEnumerator LoadSceneRoutine(SceneReference scene, ThreadPriority priority = ThreadPriority.Normal, LoadSceneMode loadSceneMode = LoadSceneMode.Additive) {
		if (this._isLoadingUIActive == false)
		{
			this.UpdateUI(true);
			yield return new WaitForSeconds(0.35f);
		}
		Application.backgroundLoadingPriority = ThreadPriority.High;

		//wait one frame
		yield return null;

		bool unload = false;

		if (this.currentSceneIndex != -1) {
			unload = true;
			AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(this.currentSceneIndex);
			while (!asyncUnload.isDone) {
				//this._sliderLoadingBar.value = (asyncUnload.progress / 2f);
				yield return null;
			}
		}

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.BuildIndex, loadSceneMode);
		asyncLoad.allowSceneActivation = true;

		while (!asyncLoad.isDone) {
			//if (unload) this._sliderLoadingBar.value = 0.5f + (asyncLoad.progress / 2f);
			//else this._sliderLoadingBar.value = asyncLoad.progress;
			yield return null;
		}
		this.currentSceneIndex = scene.BuildIndex;
		//SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.Name));
		
		yield return new WaitUntil(MapContainer.InitMap);

		//wait one frame
		yield return null;
		this.UpdateUI(false);
		onLoadComplete?.Invoke();
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
		this._sceneRoutine = null;
		yield return new WaitForSeconds(0.35f);
	}

	IEnumerator UnloadSceneRoutine(ThreadPriority priority = ThreadPriority.Normal) {
		this.UpdateUI(true);
		yield return new WaitForSeconds(0.35f);
		Application.backgroundLoadingPriority = ThreadPriority.High;

		//wait one frame
		yield return null;

		bool unload = false;

		if (this.currentSceneIndex != -1) {
			unload = true;
			AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(this.currentSceneIndex);
			while (!asyncUnload.isDone) {
				//this._sliderLoadingBar.value = (asyncUnload.progress / 2f);
				yield return null;
			}

			this.currentSceneIndex = -1;
		}

		//wait one frame
		yield return null;
		this.UpdateUI(false);
		onUnloadComplete?.Invoke();
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
		this._sceneRoutine = null;
		yield return new WaitForSeconds(0.35f);
	}
}

