using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePopup : MonoBehaviour
{
    public static UnityEvent onShow = new UnityEvent();
    public static UnityEvent onHide= new UnityEvent();
    
    [SerializeField] private PopupType _type;
    protected RectTransform _rectTransform;
    protected PopupContainer _popupContainer;
    public PopupType type => this._type;
    
    public virtual void Init(PopupContainer popupContainer)
    {
        this._popupContainer = popupContainer;
        this._rectTransform = this.GetComponent<RectTransform>();
    }

    public virtual void Show()
    {
        EventInterface.SendEvent($"Show Popup : {this.type}");
        this.gameObject.SetActive(true);
        this._rectTransform.DOKill();
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(Vector3.one, 0.15f);
        onShow?.Invoke();
    }

    public virtual void Hide()
    {
        if(this.gameObject.activeInHierarchy == false) return;
        EventInterface.SendEvent($"Hide Popup : {this.type}");
        this._rectTransform.DOKill();
        this.transform.DOScale(Vector3.zero, 0.15f).OnComplete(()=>this.gameObject.SetActive(false));
        onHide?.Invoke();
    }
}
