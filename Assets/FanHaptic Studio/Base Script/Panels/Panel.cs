using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster)), RequireComponent(typeof(CanvasGroup))]
public class Panel : MonoBehaviour {
	[SerializeField] private TypeMenu _type;

	private RectTransform _rect;
	protected PanelsGroup _panelsGroup;
	protected CanvasGroup _canvasGroup;
	public TypeMenu type => this._type;
	public RectTransform rect => this._rect == null ? this.GetComponent<RectTransform>() : this._rect;
	public CanvasGroup canvasGroup => this._rect == null ? this.GetComponent<CanvasGroup>() : this._canvasGroup;

	public virtual void Initialization(PanelsGroup panelsGroup) {
		this._panelsGroup = panelsGroup;
		this._rect = this.GetComponent<RectTransform>();
		this._canvasGroup = this.GetComponent<CanvasGroup>();
	}

	public virtual void Show()
	{
		
	}
	
	public virtual void Hide()
	{
		
	}
}