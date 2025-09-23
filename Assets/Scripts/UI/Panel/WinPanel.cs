using System.Collections.Generic;
using DG.Tweening;
using OM.AC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FeatureUnlock
{
    public int indexUnlock;
    public Sprite mask2;
    public Sprite mask1;
    public Sprite featureUnlock;
    public Color featureColor = Color.white;
}
public class WinPanel : Panel
{
    //Win Parent
    [SerializeField] private GameObject _winNoFeature;
    [SerializeField] private GameObject _winWithFeature;
    
    //Button
    [SerializeField] private Button[] _nextLevel;
    [SerializeField] private Button[] _backHome;

    //Win Text
    [SerializeField] private ACAnimatorPlayer _showAnimation;
    [SerializeField] private TextMeshProUGUI _tLevel;

    //Feature Unlock
    [SerializeField] private ACAnimatorPlayer _showAnimationWithFeature;
    [SerializeField] private Image _mask2;
    [SerializeField] private Image _mask1;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _featureImage;
    [SerializeField] private TextMeshProUGUI _tProgress;
    [SerializeField] private GameObject _parentProgress;
    [SerializeField] private GameObject _parentFeature;
    [SerializeField] private List<FeatureUnlock> _unlockFeature = new List<FeatureUnlock>();
    
    private float _finalValue = 0;
    private float _startValue = 0;
    
    public override void Initialization(PanelsGroup panelsGroup)
    {
        foreach (Button button in this._nextLevel)
        {
            button.onClick.AddListener(GameContainer.Instance.NextGame);
        }
        foreach (Button button in this._backHome)
        {
            button.onClick.AddListener(GameContainer.Instance.BackHome);
        }
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            this.Show();
    }


    public void Show()
    {
        LevelContainer.AddLevelIndex();
        SoundContainer.PlaySound(SoundType.ShowWinPanel);
        this._tLevel.text = $"Level {LevelContainer.GetLevelIndex()}\n<size=110><color=green>Complete</size>";
        if (this.SetProgressFeature())
        {
            //this._nextLevel[1].transform.localScale = Vector3.zero;
            this._winWithFeature.SetActive(true);
            this._winNoFeature.SetActive(false);
            this._showAnimationWithFeature.Play();
        }
        else
        {
            this._winWithFeature.SetActive(false);
            this._winNoFeature.SetActive(true);
            this._showAnimation.Play();
        }
    }
    private bool SetProgressFeature()
    {
        FeatureUnlock fu = null;
        int i = LevelContainer.GetLevelIndex();
        List<FeatureUnlock> tempo = new List<FeatureUnlock>(this._unlockFeature);
        tempo.Reverse();
        foreach (FeatureUnlock unlock in this._unlockFeature)
        {
            if (i <= unlock.indexUnlock)
            {
                fu = unlock;
                break;
            }
        }
        
        if (fu == null) return false;

        this._parentFeature.gameObject.SetActive(false);
        this._parentProgress.gameObject.SetActive(true);
        this._mask1.sprite = fu.mask1;
        this._mask2.sprite = fu.mask2;
        this._featureImage.sprite = fu.featureUnlock;
        this._featureImage.color = fu.featureColor;
        int indexFeature = this._unlockFeature.FindIndex(item => item == fu);
        if (indexFeature == 0)
        {
            _finalValue = Mathf.FloorToInt((i * 100f) / fu.indexUnlock);
            _startValue = Mathf.FloorToInt(((i-1) * 100f) / fu.indexUnlock);
        }
        else
        {
            int totalIndex = fu.indexUnlock - this._unlockFeature[indexFeature - 1].indexUnlock;
            int currentValueIndex = i - this._unlockFeature[indexFeature - 1].indexUnlock; 
            _finalValue = Mathf.FloorToInt((currentValueIndex * 100f) / totalIndex);
            _startValue = Mathf.FloorToInt(((currentValueIndex-1) * 100f) / totalIndex);
        }

        this._fillImage.fillAmount = this._startValue / 100f;
        this._tProgress.text = $"{this._startValue}%";
        return true;
    }
    
    public void AnimeProgressFeature()
    {
        DOVirtual.Float(this._startValue, this._finalValue, 0.3f, UpdateProgressText).OnComplete((() =>
        {
            if (this._finalValue >= 100)
            {
                this._parentProgress.gameObject.SetActive(false);
                this._parentFeature.gameObject.SetActive(true);
                this._parentFeature.transform.DOScale(1.15f,.15f).SetLoops(2,LoopType.Yoyo);
            }
        }));
    }

    private void UpdateProgressText(float value)
    {
        this._tProgress.text = $"{(int)value}%";
        this._fillImage.fillAmount = value / 100f;
    }


    public void AppearSFX() => SoundContainer.PlaySound(SoundType.AppearSFX);
}