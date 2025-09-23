using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum TypeMenu {
	None,
	Home,
	Playing,
	Win,
	Lose,
	Tutorial
}

public class UIContainer : MonoBehaviour {
	private static UIContainer Instance;

	private CanvasScaler _mainCanvasScaler;
	[ReadOnly, SerializeField] private GameState[] _gameStatesArray = Array.Empty<GameState>();
	[SerializeField] private PopupContainer _popupContainer;
	[SerializeField] private List<TopBarContainer> _topBarContainerList = new List<TopBarContainer>();

	public void Init()
	{
		Instance = this;
		this._mainCanvasScaler = this.GetComponent<CanvasScaler>();
		this._gameStatesArray.ForEach(item => item.Initialization(this));
		this._topBarContainerList = this.GetComponentsInChildren<TopBarContainer>(true).ToList();
		this._topBarContainerList.ForEach(item => item.Initialization(this));
		this._popupContainer.Initialization(this);
		this.CheckPixelSize();
	}
	
	private void CheckPixelSize()
	{
		if (Camera.main.pixelWidth > this._mainCanvasScaler.referenceResolution.x)
		{
			if (Camera.main.pixelWidth > 2000) this._mainCanvasScaler.matchWidthOrHeight = 0.5f;
			this._mainCanvasScaler.referenceResolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
			this.GetComponentsInChildren<PanelsGroup>(true);
			foreach (GameState state in _gameStatesArray)
			{
				state.GetComponentInChildren<PanelsGroup>().UdapteBaseResolution();
			}
		}
	}

#if UNITY_EDITOR
	[Button]
	public void GetGameStatesEditor() {
		this._gameStatesArray = this.GetComponentsInChildren<GameState>(true);
	}
#endif

	public static void ShowPanel(TypeMenu menu)
	{
	    if (Instance == null) return;
	    if (Instance._gameStatesArray == null) return;
	    foreach (GameState state in Instance._gameStatesArray)
	    {
		    if (state.PanelGroup.ContainsPanel(menu))
		    {
			    state.PanelGroup.SetPanel(menu);
			    break;
		    }
	    }
	}
	
	public static Panel GetPanel(TypeMenu menu)
	{
		if (Instance == null) return null;
		if (Instance._gameStatesArray == null) return null;
		foreach (GameState state in Instance._gameStatesArray)
		{
			Panel p = null;
			if (state.PanelGroup.ContainsPanel(menu,out p))
			{
				return p;
			}
		}

		return null;
	}

	public static void UICorouDelay(float delay, UnityAction action) {
		Instance.StartCoroutine(Instance.DelayedCorou(delay, action));
	}

	private IEnumerator DelayedCorou(float delay, UnityAction action) {
		while (delay > 0.0f) {
			delay -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		action.Invoke();
	}
}
