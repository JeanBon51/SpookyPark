using DG.Tweening;
using OM.AC;
using TMPro;
using UnityEngine;

public class TutorialPopup : BasePopup {
	[SerializeField] private ACAnimatorPlayer _showAnimation;
	[SerializeField] private GifAnimator _gifAnimator;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	[SerializeField] private SpringValueVector3 _springButton;
    TutorialPanel _tutorialPanel = null;

	public override void Init(PopupContainer popupContainer) {
        base.Init(popupContainer);
        DOVirtual.DelayedCall(Time.deltaTime * 2, () => {
            this._tutorialPanel = UIContainer.GetPanel(TypeMenu.Tutorial) as TutorialPanel;
		});
    }

    public void SetUpData(TutorialGifData data, int framerate = 10) {
		this._gifAnimator.frames = data.frames;
		this._gifAnimator.frameRate = framerate;
		this._descriptionText.text = data.description;
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
        this._tutorialPanel?.NextStep();
		InputController.StaticUnlockInput();
    }

    public void HidePopupButton() => PopupContainer.StaticHidePopup(this.type);
    public void AppearSFX() => SoundContainer.PlaySound(SoundType.AppearSFX);
    public void EnableButton(bool on) => _springButton.enabled = on;

}
