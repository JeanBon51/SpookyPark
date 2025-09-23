using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialPanel : Panel {
	public bool waintingSlotTutorialDoneOnce { get; private set; } = false;

	[SerializeField] private TutoGifDataBankScriptable _gifDataBankScriptable = null;
	[SerializeField] private bool _isCanvaOverlay = false;
	[SerializeField] private Animator _handAnim = null;
	[SerializeField] private Image _bubbleSpeech = null;
	[SerializeField] private TextMeshProUGUI _bubbleSpeechTMP= null;
	[SerializeField] private GameObject _scoreHighlight = null;

	private TutorialPopup _tutorialPopup = null;

	private bool isHandActivated = false;
	private Transform _handTargetPos = null;

	private string _currKey = "null";
	private int _currIndex = -1;

	private Sequence _handSequence;

	private Dictionary<string, UnityAction[]> dictionary = new Dictionary<string, UnityAction[]>();
	private Dictionary<string, string> _textDictionary = new Dictionary<string, string>() {
		{ "0_0", "Ceci est un text test!" },
	};

	public override void Initialization(PanelsGroup panelsGroup) {
		base.Initialization(panelsGroup);
		this._gifDataBankScriptable.InitData();

		this.dictionary["Level0"] = new UnityAction[] {
			() => { this.AwaitForSecond(1.0f); },
			() => { this.Tutorial0(); }
		};

		this.dictionary["Level2"] = new UnityAction[] {
			() => { this.AwaitForSecond(1.0f); },
			() => { this.HiddenPopupCall(); }
		};

		this.dictionary["Level12"] = new UnityAction[] {
			() => { this.AwaitForSecond(1.0f); },
			() => { this.DBPopupCall(); }
		};

		//this.dictionary["Level10"] = new UnityAction[] {
		//	() => { this.ExamplePopupCall(); },
		//};

		DOVirtual.DelayedCall(Time.deltaTime * 2, () => {
			this._tutorialPopup = PopupContainer.StaticGetPopup(PopupType.Tutorial) as TutorialPopup;
		});
	}

	public void TryLaunchTutorial(string key, Transform tutorialSubject = null) {
		if (this.dictionary.ContainsKey(key) == false) return;
		this.gameObject.SetActive(true);
		this._bubbleSpeech.gameObject.SetActive(false);
		this._scoreHighlight.gameObject.SetActive(false);
		this._currKey = key;
		this.NextStep();
	}

	public void NextStep() {
		if (this._currKey == "null" || this._currIndex >= this.dictionary[this._currKey].Length) return;
		this._currIndex++;
		if (this._currIndex < this.dictionary[this._currKey].Length) {
			this.ResetStep();
			this.dictionary[this._currKey][this._currIndex].Invoke();
		} else if (this._currIndex == this.dictionary[this._currKey].Length) {
			this.ResetStep();
			this.ResetPanel();
		}
	}

	// Tutorial steps ------------------------------------------------------------------------------------------------------

	private void Tutorial0() {
		Car car = Board.instance.carList[0];
		Transform target = car.transform;
		if(this._isCanvaOverlay) this.SetUpHandClickEffectOverlay(target.transform);
		else this.SetUpHandClickEffectCamera(target.transform.position);
		// this.SetUpBubbleSpeech(this._textDictionary["0_0"]);
	}

	private void HiddenPopupCall() {
		this.CallPopupTutorial("Hidden", 33);
	}
	private void DBPopupCall() {
		this.CallPopupTutorial("DB", 33);
	}

	// ---------------------------------------------------------------------------------------------------------------------

	private void AwaitForSecond(float delay) {
		InputController.StaticLockInput();
		UIContainer.UICorouDelay(delay, () => { 
			InputController.StaticUnlockInput();
			this.NextStep(); 
		});
		// this.StartCoroutine(this.AwaitForSecondCorou(delay));
	}

	//private IEnumerator AwaitForSecondCorou(float delay) {
	//	// can lock Input here
	//	while(delay > 0.0f) {
	//		delay -= Time.deltaTime;
	//		yield return new WaitForEndOfFrame();
	//	}
	//	this.NextStep();
	//	// can unlock Input here
	//	Debug.Log("EndCorou");
	//}

	private void CallPopupTutorial(string dataKey, int framerate) {
		this._tutorialPopup.SetUpData(this._gifDataBankScriptable.dictionary[dataKey], framerate);
		PopupContainer.StaticShowPopup(this._tutorialPopup.type);
		InputController.StaticLockInput();
	}

	private void SetUpHandClickEffectCamera(Vector3 targetPos, string triggername = "IsActive") {
		this._handAnim.gameObject.SetActive(true);
		this._handAnim.transform.localScale = Vector3.one;

		this._handAnim.transform.position = targetPos;
		this._handAnim.transform.position += Vector3.forward * -5;

		if (GameContainer.CurrentState == GameContainer.State.InGame) this._handAnim.SetTrigger(triggername);
		else GameContainer.Instance.StartCoroutine(this.waitRoutine(() => this._handAnim.SetTrigger(triggername)));
	}

	private void SetUpHandClickEffectOverlay(Transform target, string triggername = "IsActive") {
		this._handAnim.gameObject.SetActive(true);
		this._handAnim.transform.localScale = Vector3.one;
		this.isHandActivated = true;

		this._handTargetPos = target;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(this._handTargetPos.position);

		// if (GameContainer.CurrentState == GameContainer.State.InGame) this._handAnim.SetTrigger(triggername);
		// else GameContainer.Instance.StartCoroutine(this.waitRoutine(() => this._handAnim.SetTrigger(triggername)));

		//this._handSequence = DOTween.Sequence();
		//this._handSequence.AppendCallback(() => { this._handAnim.transform.position = screenPos; });
		//this._handSequence.Append(this._handAnim.transform.DOScale(0.8f, 0.15f));
		//this._handSequence.Append(this._handAnim.transform.DOMove(screenPos + Vector3.up * 150.0f, 1.0f));
		//this._handSequence.Append(this._handAnim.transform.DOScale(1.0f, 0.15f));
		//this._handSequence.AppendInterval(0.75f);
		//this._handSequence.SetLoops(-1, LoopType.Restart);
		//this._handSequence.Play();

		this._handAnim.transform.position = screenPos;
		this._handAnim.SetTrigger(triggername);
	}

	private void SetUpBubbleSpeech(string text) {
		this._bubbleSpeech.gameObject.SetActive(true);
		this._bubbleSpeechTMP.text = text;
	}

	private void SetUpBubbleSpeech(string text, float posY) {
		this._bubbleSpeech.gameObject.SetActive(true);
		this._bubbleSpeech.transform.position = new Vector3(this._bubbleSpeech.transform.position.x, posY, this._bubbleSpeech.transform.position.z);
		this._bubbleSpeechTMP.text = text;
	}

	private void Update() {
		//if (this.isHandActivated && GameContainer.CurrentState == GameContainer.State.InGame) {
		//	//Vector3 vDir = (Camera.main.transform.position - this._handTargetPos.position).normalized;
		//	//this._handAnim.transform.position = this._handTargetPos.position + vDir * 1;
		//	Vector3 screenPos = Camera.main.WorldToScreenPoint(this._handTargetPos.position);
		//	this._handAnim.transform.position = screenPos;
		//}
	}
	private void StopHandAnim() {
		this._handSequence.Kill();
		this._handAnim.SetTrigger("IsIdle");
		this._handAnim.gameObject.SetActive(false);
		this._handTargetPos = null;
		this.isHandActivated = false;
	}

	public void ResetStep() {
		this.StopHandAnim();
		this._bubbleSpeechTMP.text = "null";
		this._bubbleSpeech.gameObject.SetActive(false);
		this._scoreHighlight.gameObject.SetActive(false);
	}

	public void ResetPanel() {
		this._currIndex = -1;
		this._currKey = "null";
		this._handAnim.gameObject.SetActive(false);
	}

	public IEnumerator waitRoutine(UnityAction lambda) {
		Panel p = UIContainer.GetPanel(TypeMenu.Tutorial);
		yield return new WaitUntil(() => p.gameObject.activeInHierarchy);
		lambda?.Invoke();
	}
}
