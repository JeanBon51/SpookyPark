using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour {
	private static InputController Instance;
	public static void StaticLockInput() => canInput = false;
	public static void StaticUnlockInput() => canInput = true;
	public static bool canInput { get; private set; } = true;
	public static UnityEvent OnCliked = new UnityEvent();

	[SerializeField, Header("Ref")] private Board _board;
	[SerializeField, Header("Var")] private LayerMask _inputLayer = new LayerMask();
	[SerializeField] private float _minDistanceToSwipe;
	private bool _onClick = false, _onHold = false, _onRelease = false;
	private CarPart _currCarPart = null;
	private Vector3 _mouseStartPos = Vector3.zero;

	private TutorialPanel _tutorialPanel = null;

	private void Awake() {
		StaticUnlockInput();
		if (Instance != null) Destroy(this.gameObject);
		Instance = this;
		if (this._board == null) this._board = this.GetComponent<Board>();
		this._tutorialPanel = UIContainer.GetPanel(TypeMenu.Tutorial) as TutorialPanel;
	}

	private void Update() {
		if (canInput) {
			if (Input.GetMouseButtonDown(0)) {
				this._mouseStartPos = Input.mousePosition;
				this._onClick = true;
			}
			else if (Input.GetMouseButtonUp(0)) {
				this._onRelease = true;
			}
			else if (Input.GetMouseButton(0)) {
				this._onHold = true;
			}
		}
	}

	private void FixedUpdate() {
		if (canInput) {
			if (this._onClick) {
				this._currCarPart = this.RaycastCar();
				if (this._currCarPart) {
					this._board.TryMoveCar(this._currCarPart);
					if (this._tutorialPanel != null) this._tutorialPanel.NextStep();
				}
				this._onClick = false;
			}
		}
	}

	private CarPart RaycastCar() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, 10000.0f, this._inputLayer) && hit.collider.gameObject.TryGetComponent(out CarPart carPart) && carPart.car.isHidden == false) {
			return carPart;
		}
		return null;
	}
}