using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("FanHaptic Component/UI/Toggle Switch")]
public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] private Button _bSwitch;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private TextMeshProUGUI _textHandle;
    private bool _toggleOn = false;
    public bool On => this._toggleOn;
    public UnityEvent<bool> OnSwitch = new UnityEvent<bool>();

    private Vector2 _posDefaultOff = Vector2.zero;

    private bool isInit = false;
    
    private void Awake()
    {
        this.Init();
    }

    private void Init()
    {
        if(isInit) return;
        this._posDefaultOff = this._handle.anchoredPosition;
        this._bSwitch.onClick.AddListener(OnClick);
        isInit = true;
    }

    private void OnClick()
    {
        if (this._toggleOn)
        {
            this._handle.DOAnchorPosX(this._posDefaultOff.x, 0.15f).SetEase(Ease.OutBack);
            this._textHandle.text = "OFF";
        }
        else
        {
            this._handle.DOAnchorPosX(-this._posDefaultOff.x, 0.15f).SetEase(Ease.OutBack);
            this._textHandle.text = "ON";
        }
        this._toggleOn = !this._toggleOn;
        OnSwitch?.Invoke(this._toggleOn);
    }

    public void SetEnable(bool on)
    {
        this.Init();
        if (on)
        {
            this._handle.anchoredPosition = new Vector2(-this._posDefaultOff.x,this._handle.anchoredPosition.y);
            this._textHandle.text = "ON";
        }
        else
        {
            this._handle.anchoredPosition = new Vector2(this._posDefaultOff.x,this._handle.anchoredPosition.y);
            this._textHandle.text = "OFF";
        }
        this._toggleOn = on;
    }
}
