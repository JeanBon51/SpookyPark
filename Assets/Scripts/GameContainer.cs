using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameContainer : MonoBehaviour
{
    //--------------------------------------------- Static ---------------------------------------------------------------------------------------------------------------------------
    public static GameContainer Instance;
    public static UnityEvent onBackHome = new UnityEvent();
    private static Dictionary<State, GameState> GameStatDict = new Dictionary<State, GameState>();
    public static State CurrentState => Instance.currentState;

	public static void AddGameStateDict(State s, GameState obj)
    {
        if (GameStatDict.ContainsKey(s))
        {
            GameStatDict[s] = obj;
        }
        else
        {
            GameStatDict.Add(s, obj);
        }
    }

    public static void RemoveGameStateDict(State s, GameState obj)
    {
        if (GameStatDict.ContainsKey(s))
        {
            GameStatDict.Remove(s);
        }
    }

    //--------------------------------------------- Variable ---------------------------------------------------------------------------------------------------------------------------

    [SerializeField] private UIContainer _uiContainer;
    [SerializeField] private LevelContainer _levelContainer;
    [SerializeField] private SoundContainer _soundContainer;
    [SerializeField] private bool _directPlay = false;

	public enum State
    {
        None,
        Main,
        InGame
    }

    private State _currentState;

    public State currentState
    {
        get { return this._currentState; }
        private set
        {
            if (this._currentState != value)
            {
                this.ShowStat(value);
                this._currentState = value;
            }
        }
    }

    //--------------------------------------------- Unity Void ------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if(this._directPlay) this._levelContainer.Enable(true);
        this.StartSdk();
    }
    private void StartSdk()
    {
        Debug.Log("Start Init");
        TimerInterface.Init(); // Timer First Always
        
        this.Initialization(); // After Game Container
        this._soundContainer.Init(); // Init _soundContainer
        this._uiContainer.Init(); // Init UI
        
        this.currentState = State.Main; // Set State
        DOTween.SetTweensCapacity(400, 400);
        if(this._directPlay) this.Play();
        EventInterface.SendEvent("Game Init");
    }
    //--------------------------------------------- Void ------------------------------------------------------------------------------------------------------------------------------

    public void Initialization()
    {
        Instance = this;
        if (UserPropertyInterface.FirstLaunch == false)
        {
            InventoryInterface.AddCurrency(CurrencyType.Life, 5);
            UserPropertyInterface.FirstLaunch = true;
        }
    }
    
    public void Play()
    {
        TinySauce.OnGameStarted((LevelContainer.GetLevelIndex()+1));
        this._levelContainer.LoadGameScene();
        LevelContainer.onLoadComplete.AddListener(this.SetCurrentStateInGame);
    }

    private void SetCurrentStateInGame()
    {
        this.currentState = State.InGame;
        LevelContainer.onLoadComplete.RemoveListener(this.SetCurrentStateInGame);
    }

    public void NextGame() {
        TinySauce.OnGameStarted((LevelContainer.GetLevelIndex()+1));
		this._levelContainer.LoadGameScene();
        LevelContainer.onLoadComplete.AddListener(this.ResetCurrentStateInGame);
    }
    public void RetryGame()
    {
        TinySauce.OnGameFinished(false,0,(LevelContainer.GetLevelIndex()+1));
        TinySauce.OnGameStarted((LevelContainer.GetLevelIndex()+1));
        this._levelContainer.LoadGameScene();
        LevelContainer.onLoadComplete.AddListener(this.ResetCurrentStateInGame);
    }

    private void ResetCurrentStateInGame()
    {
        (GameStatDict[State.InGame] as InGameState)?.Reset();
        LevelContainer.onLoadComplete.RemoveListener(this.ResetCurrentStateInGame);
    }

    public void BackHome()
    {
        onBackHome?.Invoke();
        this._levelContainer.UnloadScene();
        PopupContainer.HideAllPopup();
        LevelContainer.onUnloadComplete.AddListener(this.BackStateMain);
        EventInterface.BackHome();
	}


	private void BackStateMain()
    {
        this.currentState = State.Main;
        LevelContainer.onUnloadComplete.RemoveListener(this.BackStateMain);
    }
    
    public void Win()
    {
        TinySauce.OnGameFinished(true,0,(LevelContainer.GetLevelIndex()+1));
        UIContainer.ShowPanel(TypeMenu.Win);
        Debug.Log("<color=green>WIN</color>");
    }

    public void Lose()
    {
        TinySauce.OnGameFinished(false,0,(LevelContainer.GetLevelIndex()+1));
        UIContainer.ShowPanel(TypeMenu.Lose);
        Debug.Log("<color=red>LOSE</color>");
    }

    private void ShowStat(State s)
    {
        foreach (KeyValuePair<State, GameState> valuePair in GameStatDict)
        {
            if (valuePair.Key != s)
            {
                valuePair.Value.HideState();
            }
            else
            {
                valuePair.Value.ShowState();
            }
        }
    }
    
    private void LateUpdate()
    {
        TimerInterface.UpdateTimer();
    }
}