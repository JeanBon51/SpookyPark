using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Tabs : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TypeMenu _tabsType;
    [SerializeField] private RectTransform _onFocus;
    [SerializeField] private bool _lock;
    [SerializeField] private int _unlockAtLevel;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Image _unlockImage;

    private RectTransform _rectTransform;
    private TabsGroup _tabsGroup;

    public RectTransform rectTransform => this._rectTransform;
    public TypeMenu type => this._tabsType;
    public void Initialization(TabsGroup tabsGroup)
    {
        this._tabsGroup = tabsGroup;
        this._button.onClick.AddListener(OnClickButton);
        this.SetFocus(false);
        if (this._unlockAtLevel > LevelContainer.GetLevelIndex())
        {
            this._lockImage.gameObject.SetActive(true);
            this._unlockImage.gameObject.SetActive(false);
            this._lock = true;
            LevelContainer.onLevelUp.AddListener((index =>
            {
                if (index >= this._unlockAtLevel)
                {
                    this._lock = false;
                    this._lockImage.gameObject.SetActive(false);
                    this._unlockImage.gameObject.SetActive(true);
                }
            }));
        }
        else
        {
            this._lockImage.gameObject.SetActive(false);
            this._unlockImage.gameObject.SetActive(true);
        }
    }

    public void SetFocus(bool focus)
    {
        if(focus)this._onFocus.anchoredPosition = new Vector2(this._onFocus.anchoredPosition.x, 93);
        else this._onFocus.anchoredPosition = new Vector2(this._onFocus.anchoredPosition.x, -120);
    }

    private void OnClickButton()
    {
        if(this._lock)return;
        this._tabsGroup.ShowPanel(this.type);
        //VibrationInterface.VibrateMedium();
    }


    public void AnimationSelectTabs()
    {
        this._onFocus.gameObject.SetActive(true);
        this._onFocus.DOAnchorPosY(93, 0.25f).SetEase(Ease.OutBack);
    }
    public void AnimationUnseletectTabs()
    {
        this._onFocus.DOAnchorPosY(-120, 0.20f);
    }
}
