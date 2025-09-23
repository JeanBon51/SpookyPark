using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[System.Serializable]
public enum PopupType
{
    none,
    Setting,
    TroubleShooter,
    Tutorial,
    UnlockFeature
}
public class PopupContainer : MonoBehaviour
{
    //----------------------------------------- STATIC ---------------------------------------------------
   
    private static PopupContainer Instance;
    public static void StaticShowPopup(PopupType type, bool hideOtherPopup = false)
    {
        if(Instance == null) return;
        if(Instance._currentTypePopupList.Count > 0 && Instance._currentTypePopupList[^1] == type) Instance.HidePopup();
        else Instance.ShowPopup(type, hideOtherPopup);
    }
    public static BasePopup StaticGetPopup(PopupType type)
    {
        if(Instance == null) return null;
        if (Instance._popupDict.ContainsKey(type)) return Instance._popupDict[type];
        else return null;
    }
    public static void StaticHidePopup(PopupType type, bool hideOtherPopup = false)
    {
        if(Instance == null) return;
        if(Instance._currentTypePopupList.Count > 0 && Instance._currentTypePopupList[^1] == type) Instance.HidePopup();
    }

    public static void HideAllPopup()
    {
        if(Instance == null) return;
        Instance._currentTypePopupList.ForEach(item =>Instance._popupDict[item].Hide());
        Instance._currentTypePopupList.Clear();
        Instance._bBackground.gameObject.SetActive(false);
    }

    public static UnityEvent<PopupType> OnHidePopup = new UnityEvent<PopupType>();

    public static bool isPopupActive => Instance._currentTypePopupList.Count > 0;
    
    //----------------------------------------- VOID ---------------------------------------------------
    
    [ReadOnly] private Dictionary<PopupType,BasePopup> _popupDict = new Dictionary<PopupType, BasePopup>();
    [SerializeField] private Button _bBackground;
    private UIContainer _uiContainer;
    private List<PopupType> _currentTypePopupList = new List<PopupType>();
    
    public void Initialization(UIContainer uiContainer)
    {
        Instance = this;
        this._uiContainer = uiContainer;
        this.GetComponentsInChildren<BasePopup>(true).ForEach(item =>
        {
            item.Init(this);
            if(item.type != PopupType.none)
                this._popupDict.Add(item.type,item);
        });
        this._bBackground.onClick.AddListener(()=>HidePopup(false));
    }
    public void ShowPopup(PopupType type) => this.ShowPopup(type,false);
    
    [Button]
    public void ShowPopup(PopupType type, bool hideOtherPopup = false)
    {
        this._bBackground.gameObject.SetActive(true);
        if (this._popupDict.ContainsKey(type) == false)
        {
            if(type != PopupType.none) Debug.LogError($"Missing Popup Type : {type}");
            return;
        }
        else
        {
            if (hideOtherPopup)
            {
                this._currentTypePopupList.ForEach(item => this._popupDict[item].Hide());
                this._currentTypePopupList.Clear();
            }
            this._currentTypePopupList.Add(this._popupDict[type].type);
            this._popupDict[type].Show();
        }
    }
    public void HidePopup(bool allPopup = false)
    {
        if(this._currentTypePopupList.Count <= 0) return;
        if (allPopup)
        {
            this._currentTypePopupList.ForEach(item =>this._popupDict[item].Hide());
            this._currentTypePopupList.Clear();
        }
        else
        {
            OnHidePopup?.Invoke(this._popupDict[this._currentTypePopupList[^1]].type);
            this._popupDict[this._currentTypePopupList[^1]].Hide();
            this._currentTypePopupList.RemoveAt(this._currentTypePopupList.Count-1);
        }
        
        this._bBackground.gameObject.SetActive(this._currentTypePopupList.Count != 0);
    }
    public void EnablePopup(bool on, PopupType type)
    {
        this._bBackground.gameObject.SetActive(on);
        if (this._popupDict.ContainsKey(type))
        {
            this._popupDict[type].gameObject.SetActive(on);
        }
    }
}
