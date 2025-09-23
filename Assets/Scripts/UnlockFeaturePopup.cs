using System;
using System.Collections.Generic;
using DG.Tweening;
using OM.AC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UnlockFeatureData
{
    public int index;
    public Color colorRibbon;
    public Color colorLine;
    public Color colorIcon;
    public Sprite icon;
    public string title;
    [TextArea] public string textUnlock;
}

public class UnlockFeaturePopup : BasePopup
{
    private const string keySave = "PopupNewColor";
    
    [SerializeField] private SpringValueVector3 _springButton;
    [SerializeField] private Image _ribbon;
    [SerializeField] private Image _line;
    [SerializeField] private Image _box;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private ACAnimatorPlayer _showAnimation;
    [SerializeField] private UnlockFeatureData[] _unlockFeature;

    public override void Init(PopupContainer popupContainer)
    {
        base.Init(popupContainer);
        LevelContainer.onLoadComplete.AddListener((() =>
        {
            if(GameContainer.CurrentState != GameContainer.State.InGame) return;
            UnlockFeatureData data = Array.Find(this._unlockFeature, item => item.index == LevelContainer.GetLevelIndex());
            if(data == null) return;
            List<int> alreadySeen = SaveDataJsonInterface.Exist<List<int>>(keySave) ? SaveDataJsonInterface.GetObject<List<int>>(keySave): new List<int>();
            if(alreadySeen.Contains(data.index)) return;
            alreadySeen.Add(data.index);
            this.SetUi(data.index);
            PopupContainer.StaticShowPopup(this.type);
            SaveDataJsonInterface.SetObject(keySave,alreadySeen);
        }));
    }

    public override void Show()
    {
        SoundContainer.PlaySound(SoundType.UnlockFeature);
        InputController.StaticLockInput();
        EventInterface.SendEvent($"Show Popup : {this.type}");
        this.gameObject.SetActive(true);
        this._showAnimation.Play();
        this._rectTransform.DOKill();
        this.transform.localScale = Vector3.one;
        onShow?.Invoke();
    }

    public override void Hide()
    {
        base.Hide();
        InputController.StaticUnlockInput();
    }


    private void SetUi(int levelIndex)
    {
        UnlockFeatureData data = Array.Find(this._unlockFeature, item => item.index == levelIndex);
        this._ribbon.color = data.colorRibbon;
        this._line.color = data.colorLine;
        this._box.sprite = data.icon;
        this._box.color = data.colorIcon;
        this._text.text = data.textUnlock;
        this._title.text = data.title;
    }
    
    public void HidePopupButton() => PopupContainer.StaticHidePopup(this.type);
    
    public void AppearSFX() => SoundContainer.PlaySound(SoundType.AppearSFX);
    public void EnableButton(bool on) => _springButton.enabled = on;
}
