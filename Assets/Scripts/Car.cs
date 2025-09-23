using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public enum CarType {
	Car1 = 0,
	Car2,
	Car3,
	Car4
}

public enum StepMoving {
	onPark,
	enterOnSpline,
	onSpline,
	OutOffLevel
}

public class Car : MonoBehaviour {

	//------------------ Variables ------------------

	//--- ObjContainer ---
	[SerializeField] private Obj _objPrefab;
	[SerializeField] private LayerMask _layerMask;

	private float _delayEnterSpline = 0;
	private SplineContainer _splineContainer;
	// private ObjType _type = ObjType.None;
	private StepMoving _stepMoving = StepMoving.onPark;

	private bool _stop = false;
	private bool _inverseEnter = false;
	private Dictionary<Obj, CarPart> _currentObj = new Dictionary<Obj, CarPart>();

	//--- Car ---
	[SerializeField, Header("Ref")] private ColorPalette _colorPalette = null;
	[SerializeField, Header("Var")] private CarPart[] _carParts = null;
	[SerializeField] private float _speed = 5.0f;
	[SerializeField] private float _maxSpeed = 36.0f;
	[SerializeField] private float _offsetCar = 1.0f;
	[SerializeField] private Transform[] _raycastSpawns = null, _raycastVeilSpawns = null;
	[SerializeField] private float _detectionDistForPathing = 3.0f;
	[SerializeField] private LayerMask _carLayer = new LayerMask();

	[SerializeField, Header("Hidden Mecha")] private bool _isHidden = false;
	private int _moveIndex = int.MinValue;
	private bool _isWaitingForCarToMove = false;
	private bool _isEnded = false;

	private bool _isRunningForward = false;
	private bool _isInReverse = false;
	private Board _board;
	private int _maxObj = 0;


	private bool _tryToUnveil;
	private bool _firstUnveilPass = true;

	public ColorPalette colorPalette => this._colorPalette;
	[SerializeField, Header("Data")] private CarType _carType = CarType.Car1;
	//[SerializeField] private ObjType _eColor = ObjType.Color_1;
	[SerializeField] private PassengersData _forcePassengerData = null; 

	//------------------ AutoProperties -------------
	public bool hasStoppedMovingThisFrame = false;
	//------------------ Getter/Setter --------------
	public bool isRunningForward => this._isRunningForward;
	public bool isOnSpline { get => this._stepMoving == StepMoving.onSpline || this._stepMoving == StepMoving.enterOnSpline; }
	public bool yAxis { get => this.transform.localEulerAngles.y % 180 == 0; }
	public CarType carType { get => this._carType; }
	//public ObjType eColor { get => this._eColor; }
	// public ObjType type => this._type;
	public StepMoving stepMoving => this._stepMoving;
	public int numberOfObj => this._currentObj.Values.Count;
	public Dictionary<Obj, CarPart> dictObj => this._currentObj;
	public PassengersData forcePassengerData => this._forcePassengerData;
	public CarPart[] carParts => this._carParts;
	public bool isHidden => this._isHidden;
	public bool tryToUnveil => this._tryToUnveil;

	//------------------ Unity Void -----------------
	private void FixedUpdate() {
		if(this._isEnded) return;
		this.hasStoppedMovingThisFrame = false;
		switch (this._stepMoving)
		{
			case StepMoving.onPark:
				this.MoveToSpline();
				break;
			case StepMoving.enterOnSpline:
				this.MoveEnterSpline();
				break;
			case StepMoving.onSpline:
				this.MoveOnSpline();
				break;
			case StepMoving.OutOffLevel:
				this.MoveOutOfSpline();
				break;
		}

		if (this._stepMoving != StepMoving.OutOffLevel) {
			foreach (Obj obj in this._currentObj.Keys) {
				obj.SetPassengerMovementEffect(this._isInReverse);
			}
		}
	}

	private void LateUpdate() {
		if (this._tryToUnveil == false) return;

		if (this.CheckIfPossibleToSentOnSpline()) 
			this.UnveilHiddenCar();

		if (this._firstUnveilPass) this._firstUnveilPass = false;
	}

	//------------------ Void -----------------------
	public void Init(Board board, CarData carData, Transform carContainer, bool isInRuntime = true)
	{
		this._speed = this._speed + (1.1f * LevelContainer.GetLevelIndex() + 1);
		if (this._speed > this._maxSpeed) this._speed = this._maxSpeed;
		this._board = board;
		this._carType = carData.carType;
		this._isHidden = carData.isHidden;
		this.transform.localPosition = new Vector3(carData.localPosition[0], carData.localPosition[1], carData.localPosition[2]);
		this.transform.localRotation = Quaternion.Euler(carData.localRotation[0], carData.localRotation[1], carData.localRotation[2]);
		DOVirtual.DelayedCall(Time.deltaTime * 2, () => { this._tryToUnveil = true; });

		for (int i = 0; i < this._carParts.Length; i++) {
			CarPart cp = this._carParts[i];
			if (isInRuntime) cp.transform.SetParent(carContainer);
			List<Material> materials = new List<Material>() { cp.meshRenderer.sharedMaterials[0] };

			cp.eColor = carData.carPartColors[i];
			if (this._isHidden) {
				materials.AddRange(this._colorPalette.hidenMaterials);
				//materials.AddRange(this._colorPalette.baseCartColor);
			}
			else {
				materials.AddRange(this._colorPalette.colorDict[cp.eColor].materials);
				//materials.AddRange(this._colorPalette.baseCartColor);
			}
			
			cp.Init(this, materials, cp.eColor,this._isHidden, isInRuntime);
			
			this._maxObj += cp.carSeats.Length;
		}

		this.InitObj(carData.passengerData, isInRuntime);
	}
	public void LDInit(Board board, CarData carData, Transform carContainer) {
		this.Init(board, carData, carContainer, false);
		this._forcePassengerData = carData.passengerData == null ? new PassengersData() : carData.passengerData;
	}
	private void InitObj(PassengersData passengersData, bool isInRuntime)
	{
		foreach (PassengerGroupData g in passengersData.passengerGroupes)
		{
			for (int i = 0; i < g.number; i++)
			{
				Obj o = Instantiate(this._objPrefab,this.transform);
				this.AddObjSpawn(o);
				o.Init(g.color, this._isHidden, isInRuntime);
			}
		}
		this.StopMove(true);
	}

	//--- Unveil Car ---
	private bool CheckIfPossibleToSentOnSpline() {
		RaycastHit hit = new RaycastHit();
		bool front = this.isFrontFree();
		bool back = this.isBackFree();
		return front || back;
	}

	public void UnveilHiddenCar() {
		this._isHidden = false;
		this._tryToUnveil = false;
		foreach (CarPart cp in this.carParts) {
			List<Material> materials = new List<Material>() { cp.meshRenderer.sharedMaterials[0] };
			materials.AddRange(this._colorPalette.colorDict[cp.eColor].materials);
			cp.meshRenderer.SetSharedMaterials(materials);
			if(this._firstUnveilPass == false) cp.transform.DOScale(1.2f, .2f).SetLoops(2, LoopType.Yoyo);
			cp.meshRenderer.renderingLayerMask = cp.GetLayer(cp.eColor) + 1;
		}

		foreach (Obj p in this._currentObj.Keys) {
			p.UnveilHiddenMat();
		}
	}

	//--- Input Controll ---
	public bool TryMove(bool isInReverse, int moveIndex)
	{
		if (this._board.autoSort != null && this._board.autoSort.canAddObjContainer == false) return false;
		if(this._stepMoving != StepMoving.onPark) return false;
		SoundContainer.PlaySound(SoundType.MoveCarInput);
		VibrationInterface.VibrateLight();
		this._moveIndex = moveIndex;

		this._isRunningForward = true;
		this._isInReverse = this.isFrontFree() == false;

		return true;
	}
	private void MoveToSpline() {
		if (this._isRunningForward == false) return;
		this._isWaitingForCarToMove = this.CheckMovingCarInFront(this._isWaitingForCarToMove);
		if (this._isWaitingForCarToMove) return;

		Vector3 moveVec = (this._isInReverse ? -this._carParts.Last().transform.forward : this._carParts[0].transform.forward) * this._speed * Time.fixedDeltaTime;

		if(this._isInReverse == false) {
			for (int i = this._carParts.Length - 1; i > 0; i--) {
				Vector3 dir = (this._carParts[i - 1].transform.localPosition - this._carParts[i].transform.localPosition).normalized;
				this._carParts[i].transform.localPosition += dir * moveVec.magnitude;
				float angle = Vector3.SignedAngle(this._carParts[i].transform.forward, dir, Vector3.up);
				this._carParts[i].transform.localRotation = Quaternion.Euler(0, this._carParts[i].transform.localEulerAngles.y + angle, 0);
			}

			this._carParts[0].transform.localPosition += moveVec;
		}
		else {
			for (int i = 0; i < this._carParts.Length - 1; i++) {
				Vector3 dir = (this._carParts[i + 1].transform.localPosition - this._carParts[i].transform.localPosition).normalized;
				this._carParts[i].transform.localPosition += dir * moveVec.magnitude;
				float angle = Vector3.SignedAngle(this._carParts[i].transform.forward, -dir, Vector3.up);
				this._carParts[i].transform.localRotation = Quaternion.Euler(0, this._carParts[i].transform.localEulerAngles.y + angle, 0);
			}

			this._carParts.Last().transform.localPosition += moveVec;
		}

	}
	private bool isFrontFree() {
		bool result = true;
		RaycastHit hit = new RaycastHit();
		for (int i = 0; i < 2; i++) {
			Physics.Raycast(this._raycastVeilSpawns[i].transform.position, this._raycastVeilSpawns[i].transform.forward, out hit, 100.0f, this._carLayer);
			if (hit.collider != null && hit.collider.TryGetComponent(out CarPart cp)) {
				if (cp.car.stepMoving == StepMoving.onPark)
					result = false;
			}
		}
		return result;
	}
	private bool isBackFree() {
		bool result = true;
		RaycastHit hit = new RaycastHit();
		for (int i = 2; i < 4; i++) {
			Physics.Raycast(this._raycastVeilSpawns[i].transform.position, this._raycastVeilSpawns[i].transform.forward, out hit, 100.0f, this._carLayer);
			if (hit.collider != null && hit.collider.TryGetComponent(out CarPart cp)) {
				if (cp.car.stepMoving == StepMoving.onPark)
					result = false;
			}
		}
		return result;
	}

	// --- Stop Moving ---
	private void StopMoving(CarPart a, CarPart b) {
		Vector3 vec = b.transform.localPosition - a.transform.localPosition;

		if (b.car.isRunningForward || b.car.hasStoppedMovingThisFrame) {
			bool afc = a.isFrontCarInMovement(), bfc = b.isFrontCarInMovement();
			if(afc && bfc) {
				if (a.car.yAxis != b.car.yAxis) {
					Vector3 pos = this.yAxis ? new Vector3(a.transform.localPosition.x, 0, b.transform.localPosition.y) : new Vector3(b.transform.localPosition.x, 0, a.transform.localPosition.y);
					float aDist = Vector3.Distance(a.transform.localPosition, pos), bDist = Vector3.Distance(b.transform.localPosition, pos);
					if (aDist < bDist) this.StopMovingWithoutRepositioning();
					else this.StopMoving(a.transform.localPosition, a.size, b.transform.localPosition, b.size, b.yAxis);
				}
				else {
					this.StopMoving(a.transform.localPosition, a.size, b.transform.localPosition, b.size, b.yAxis);
				}
			}
			else if (afc) {
				this.StopMoving(a.transform.localPosition, a.size, b.transform.localPosition, b.size, b.yAxis);
			} else {
				this.StopMovingWithoutRepositioning();
			}
		}
		else {
			this.StopMoving(a.transform.localPosition, a.size, b.transform.localPosition, b.size, b.yAxis);
			b.car.BumpedByOtherCar(a, b);
		}

	}
	private void StopMoving(CarPart a, Transform b) {
		this.StopMoving(a.transform.localPosition, a.size, b.localPosition, b.localScale, false);
	}
	private void StopMoving(Vector3 aPos, Vector3 aSize, Vector3 bPos, Vector3 bSize, bool bYAxis) {
		Vector3 pos = this.yAxis ?
			new Vector3(aPos.x, aPos.y, bPos.z) :
			new Vector3(bPos.x, aPos.y, aPos.z);

		float otherOffset = (bYAxis ? (this.yAxis ? bSize.z : bSize.x) : (this.yAxis ? bSize.x : bSize.z)) / 2;
		float currOffset = otherOffset + (this._carParts[0].size.z / 2);
		currOffset += 0.1f;

		CarPart a = this._isInReverse ? this._carParts.Last() : this._carParts[0];

		a.transform.localPosition = pos + ((this._isInReverse ? this.transform.forward : -this.transform.forward).normalized * currOffset);
		a.BumpSpringEffect(this._isInReverse, false);

		if (this._isInReverse) {
			for (int i = this._carParts.Length - 2; i > -1; i--) {
				Vector3 dir = (this._carParts[i].transform.localPosition - this._carParts[i + 1].transform.localPosition).normalized;
				this._carParts[i].transform.localPosition = this._carParts[i + 1].transform.localPosition + dir * this._carParts[i].offsetToNextCar;

				float angle = Vector3.SignedAngle(this._carParts[i].transform.forward, dir, Vector3.up);
				this._carParts[i].transform.localRotation = Quaternion.Euler(0, this._carParts[i].transform.localEulerAngles.y + angle, 0);
				this._carParts[i].BumpSpringEffect(this._isInReverse, false);
			}
		}
		else {
			for (int i = 1; i < this._carParts.Length; i++) {
				Vector3 dir = (this._carParts[i].transform.localPosition - this._carParts[i - 1].transform.localPosition).normalized;
				this._carParts[i].transform.localPosition = this._carParts[i - 1].transform.localPosition + dir * this._carParts[i].offsetToNextCar;

				float angle = Vector3.SignedAngle(this._carParts[i].transform.forward, -dir, Vector3.up);
				this._carParts[i].transform.localRotation = Quaternion.Euler(0, this._carParts[i].transform.localEulerAngles.y + angle, 0);
				this._carParts[i].BumpSpringEffect(this._isInReverse, false);
			}
		}
	}
	private void StopMovingWithoutRepositioning() {
		CarPart a = this._isInReverse ? this._carParts.Last() : this._carParts[0];
		a.transform.localPosition += (this._isInReverse ? this.transform.forward : -this.transform.forward).normalized * 0.1f;
		a.BumpSpringEffect(this._isInReverse, false);

		if (this._isInReverse) {
			for (int i = this._carParts.Length - 2; i > -1; i--) {
				Vector3 dir = (this._carParts[i].transform.localPosition - this._carParts[i + 1].transform.localPosition).normalized;
				this._carParts[i].transform.localPosition = this._carParts[i + 1].transform.localPosition + dir * this._carParts[i].offsetToNextCar;

				float angle = Vector3.SignedAngle(this._carParts[i].transform.forward, dir, Vector3.up);
				this._carParts[i].transform.localRotation = Quaternion.Euler(0, this._carParts[i].transform.localEulerAngles.y + angle, 0);
				this._carParts[i].BumpSpringEffect(this._isInReverse, false);
			}
		}
		else {
			for (int i = 1; i < this._carParts.Length; i++) {
				Vector3 dir = (this._carParts[i].transform.localPosition - this._carParts[i - 1].transform.localPosition).normalized;
				this._carParts[i].transform.localPosition = this._carParts[i - 1].transform.localPosition + dir * this._carParts[i].offsetToNextCar;

				float angle = Vector3.SignedAngle(this._carParts[i].transform.forward, -dir, Vector3.up);
				this._carParts[i].transform.localRotation = Quaternion.Euler(0, this._carParts[i].transform.localEulerAngles.y + angle, 0);
				this._carParts[i].BumpSpringEffect(this._isInReverse, false);
			}
		}
	}
	private void BumpedByOtherCar(CarPart a, CarPart b) {
		bool side = a.car.yAxis != this.yAxis;
		bool negativeValue = false;
		if (side) {
			bool onRight = this.isOnRightSide(a, b);
			if (onRight) negativeValue = true;
			else negativeValue = false;
		}
		else {
			Vector3 vec = a.transform.localPosition - b.transform.localPosition;
			float angle = Vector3.Angle(vec, b.transform.forward);
			bool inFront = angle < 90.0f;
			if (inFront) negativeValue = true;
			else negativeValue = false;
		}

		foreach (CarPart cp in this._carParts) {
			cp.BumpSpringEffect(negativeValue, side);
		}
	}
	private bool isOnRightSide(CarPart a, CarPart b) {
		float diff = 0.0f;

		if (this.yAxis) {
			diff = (a.transform.localPosition.x - b.transform.localPosition.x);
			if (this.transform.forward.z > 0)
				return diff < 0;
			else
				return diff > 0;
		}
		else {
			diff = (a.transform.localPosition.z - b.transform.localPosition.z);
			if (this.transform.forward.x > 0)
				return diff > 0;
			else
				return diff < 0;
		}
	}
	public bool isCarPartFirstCarInMovement(CarPart cp) {
		if (this._isInReverse) return cp == this._carParts.Last();
		else return cp == this._carParts[0];
	}

	// --- Move Enter Spline ---
	private void MoveEnterSpline()
	{
		bool allOnSpline = true;
		if (this._inverseEnter)
		{
			for (int i = this._carParts.Length-1; i > -1; i--)
			{
				if (i == this._carParts.Length-1 && this._carParts[i].onSpline == false)
				{
					Vector3 posOnSpline = this._splineContainer.EvaluatePosition(this._splineContainer.Spline,
						this._carParts[i].splineProgression);
					Vector3 dir = (this._carParts[i].transform.position - posOnSpline).normalized.SetY(0);
					this._carParts[i].transform.position -= dir * this._speed * Time.fixedDeltaTime;
					Quaternion target = Quaternion.LookRotation(
						this._splineContainer.EvaluateTangent(this._carParts[i].splineProgression),
						this._splineContainer.EvaluateUpVector(this._carParts[i].splineProgression));
					this._carParts[i].transform.rotation = Quaternion.Lerp(this._carParts[i].transform.rotation,
						Quaternion.LookRotation(
							this._splineContainer.EvaluateTangent(this._carParts[i].splineProgression),
							this._splineContainer.EvaluateUpVector(this._carParts[i].splineProgression)),
						Time.fixedDeltaTime * 1.5f);

					if (Vector3.Distance(posOnSpline, this._carParts[i].transform.position)<= 50f)
						this._carParts[i].onSpline = true;
					else continue;
				}

				if (this._carParts[i].onSpline)
				{
					this.MoveCartOnEnterSpline(this._carParts[i],true);
				}
				else
				{
					Vector3 posOnSpline = this._splineContainer.EvaluatePosition(this._splineContainer.Spline, this._carParts[i].splineProgression);
					Vector3 dir = (this._carParts[i].transform.position - this._carParts[i + 1].transform.position).normalized * this._offsetCar;
					this._carParts[i].transform.position = this._carParts[i + 1].transform.position + dir;
					//this._carParts[i].transform.LookAt(this._carParts[i + 1].transform);
					
					
					float offset = this.GetOffsetFloatSpline(this._offsetCar);
					
					if (this._carParts[i + 1].splineProgressionAdd >= offset)
					{
						float value = this._carParts[i+1].splineProgression + offset;
						
						if (value > 0) this._carParts[i].splineProgression = value;
						else this._carParts[i].splineProgression = 1f+value;
						this._carParts[i].splineProgressionAdd = 0;
						
						this._carParts[i].onSpline = true;
					}
					
					else allOnSpline = false;
				}
			}
		}
		else
		{
			for (int i = 0; i < this._carParts.Length; i++)
			{
				if (i == 0 && this._carParts[i].onSpline == false)
				{
					Vector3 posOnSpline = this._splineContainer.EvaluatePosition(this._splineContainer.Spline,
						this._carParts[i].splineProgression);
					Vector3 dir = (this._carParts[i].transform.position - posOnSpline).normalized.SetY(0);
					this._carParts[i].transform.position += -dir * (this._speed * Time.fixedDeltaTime);
					Quaternion target = Quaternion.LookRotation(
						this._splineContainer.EvaluateTangent(this._carParts[i].splineProgression),
						this._splineContainer.EvaluateUpVector(this._carParts[i].splineProgression));
					this._carParts[i].transform.rotation = Quaternion.Lerp(this._carParts[i].transform.rotation,
						Quaternion.LookRotation(
							this._splineContainer.EvaluateTangent(this._carParts[i].splineProgression),
							this._splineContainer.EvaluateUpVector(this._carParts[i].splineProgression)),
						Time.fixedDeltaTime * 1.5f);

					if (Vector3.Distance(posOnSpline, this._carParts[i].transform.position) <= 50f)
						this._carParts[i].onSpline = true;
					else continue;
				}

				if (this._carParts[i].onSpline)
				{
					this.MoveCartOnEnterSpline(this._carParts[i]);
				}
				else
				{
					Vector3 posOnSpline = this._splineContainer.EvaluatePosition(this._splineContainer.Spline, this._carParts[i].splineProgression);
					Vector3 dir = (this._carParts[i].transform.position - this._carParts[i - 1].transform.position).normalized * this._offsetCar;
					this._carParts[i].transform.position = this._carParts[i - 1].transform.position + dir;
					this._carParts[i].transform.LookAt(this._carParts[i - 1].transform);
					float offset = this.GetOffsetFloatSpline(this._offsetCar);
					
					if (this._carParts[i - 1].splineProgressionAdd >= offset)
					{
						float value = this._carParts[i-1].splineProgression - offset;
						
						if (value > 0) this._carParts[i].splineProgression = value;
						else this._carParts[i].splineProgression = 1f+value;
						this._carParts[i].splineProgressionAdd = 0;
						
						this._carParts[i].onSpline = true;
					}
					else allOnSpline = false;
				}
			}
		}

		if (allOnSpline)
		{
			if (this._inverseEnter)
			{
				if (this.d > 0.05f) this._stepMoving = StepMoving.onSpline;
				else this.d += Time.fixedDeltaTime;
			}
			else
			{
				this._stepMoving = StepMoving.onSpline;
			}
			AutoSortingContainer.LockValue.Remove(this);
		};
	}
	private float d = 0;
	
	// --- Move On Spline ---
	private void MoveOnSpline()
	{
		if(this._stop) return;
		Ray r = new Ray(this._carParts[0].transform.position, this._carParts[0].transform.forward);
		if (Physics.Raycast(r, out RaycastHit hit,3.25f,this._layerMask) && hit.collider.TryGetComponent(out CarPart c) && c.car._stepMoving is StepMoving.onSpline or StepMoving.enterOnSpline)
		{
			Debug.Log("Stop");
		}
		else
		{
			for (int i = 0; i < this._carParts.Length; i++)
			{
				if (i == 0)
				{
					foreach (KeyValuePair<Car,float> valuePair in AutoSortingContainer.LockValue)
					{
						if (this._carParts[i].splineProgression <= valuePair.Value &&
						    this._carParts[i].splineProgression >= (valuePair.Value - valuePair.Value*0.1f))
						{
							Debug.Log("stop value");
							return;
						}
					}
				}
				this.MoveCartOnSpline(this._carParts[i]);
			}
		}
	}
	private void MoveCartOnEnterSpline(CarPart carPart, bool inverse = false)
	{
		carPart.transform.position = this._splineContainer.EvaluatePosition(this._splineContainer.Spline, carPart.splineProgression);
		carPart.transform.rotation = Quaternion.LookRotation(this._splineContainer.EvaluateTangent(carPart.splineProgression),
			this._splineContainer.EvaluateUpVector(carPart.splineProgression));
		if(inverse) carPart.splineProgression -= (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		else carPart.splineProgression += (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		carPart.splineProgressionAdd += (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		if (carPart.splineProgression > 1) carPart.splineProgression = 0;
		if (carPart.splineProgression < 0) carPart.splineProgression = 1 + carPart.splineProgression;
	}
	private void MoveCartOnSpline(CarPart carPart)
	{
		carPart.transform.position = this._splineContainer.EvaluatePosition(this._splineContainer.Spline, carPart.splineProgression);
		carPart.transform.rotation = Quaternion.LookRotation(this._splineContainer.EvaluateTangent(carPart.splineProgression),
			this._splineContainer.EvaluateUpVector(carPart.splineProgression));
		carPart.splineProgression += (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		carPart.splineProgressionAdd += (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		if (carPart.splineProgression > 1) carPart.splineProgression -= 1;
	}
	private void MoveCartOutOfSpline(CarPart carPart)
	{
		carPart.transform.position = this._splineContainer.EvaluatePosition(this._splineContainer.Spline, carPart.splineProgression);
		carPart.transform.rotation = Quaternion.LookRotation(this._splineContainer.EvaluateTangent(carPart.splineProgression),
			this._splineContainer.EvaluateUpVector(carPart.splineProgression));
		carPart.splineProgression += (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		carPart.splineProgressionAdd += (this._speed * Time.fixedDeltaTime) / this._splineContainer.Spline.GetLength();
		if (carPart.splineProgression > 1) this._isEnded = true;
	}
	private void MoveOutOfSpline()
	{
		for (int i = 0; i < this._carParts.Length; i++)
		{
			if (i == 0 && this._carParts[i].onSpline == false)
			{
				Vector3 posOnSpline = this._splineContainer.EvaluatePosition(this._splineContainer.Spline,
					this._carParts[i].splineProgression);
				Vector3 dir = (this._carParts[i].transform.position - posOnSpline).normalized.SetY(0);
				this._carParts[i].transform.position += -dir * (this._speed * Time.fixedDeltaTime);
				Quaternion target = Quaternion.LookRotation(
					this._splineContainer.EvaluateTangent(this._carParts[i].splineProgression),
					this._splineContainer.EvaluateUpVector(this._carParts[i].splineProgression));
				this._carParts[i].transform.rotation = Quaternion.Lerp(this._carParts[i].transform.rotation,
					Quaternion.LookRotation(
						this._splineContainer.EvaluateTangent(this._carParts[i].splineProgression),
						this._splineContainer.EvaluateUpVector(this._carParts[i].splineProgression)),
					Time.fixedDeltaTime * 5f);

				if (Vector3.Distance(posOnSpline, this._carParts[i].transform.position) <= 1.4f)
					this._carParts[i].onSpline = true;
				else continue;
			}

			if (this._carParts[i].onSpline)
			{
				this.MoveCartOutOfSpline(this._carParts[i]);
			}
			else
			{
				Vector3 posOnSpline = this._splineContainer.EvaluatePosition(this._splineContainer.Spline,
					this._carParts[i].splineProgression);
				Vector3 dir =
					(this._carParts[i].transform.position - this._carParts[i - 1].transform.position).normalized *
					this._offsetCar;
				this._carParts[i].transform.position = this._carParts[i - 1].transform.position + dir;
				this._carParts[i].transform.LookAt(this._carParts[i - 1].transform);
				float offset = this.GetOffsetFloatSpline(this._offsetCar);

				if (this._carParts[i - 1].splineProgressionAdd >= offset)
				{
					this._carParts[i].onSpline = true;
				}
			}
		}
	}
	
	// --- Add On Spline ---
    public void AddToSplineForward(SplineContainer splineContainer)
    {
	    this._splineContainer = splineContainer;
	    float progression = WorldToSplineT(splineContainer, this._carParts[0].transform.position);
	    float v = progression - (this.GetOffsetFloatSpline(this._offsetCar)*0.5f);
	    v = v > 0 ? v : 1f - v;
	    AutoSortingContainer.LockValue.Add(this, v);
	    this._carParts[0].splineProgression = progression;
	    this._carParts[0].splineProgressionAdd = 0;
	    // for (int i = 0; i < this._carParts.Length; i++)
	    // {
		   //  float value =  progression /*- this.GetOffsetFloatSpline(this._offsetCar) * i*/;
		   //  if (value > 0) this._carParts[i].splineProgression = value;
		   //  else this._carParts[i].splineProgression = 1f-value;
	    // }
	    this.StopMove(false);
    }
    public void AddToSplineBackward(SplineContainer splineContainer)
    {
	    this._inverseEnter = true;
	    this._splineContainer = splineContainer;
	    float progression = WorldToSplineT(splineContainer, this._carParts[^1].transform.position);
	    float v = progression - this.GetOffsetFloatSpline(this._offsetCar) * (this._carParts.Length + 1);
	    v = v > 0 ? v : 1f - v;
	    AutoSortingContainer.LockValue.Add(this, v);
	    this._carParts[^1].splineProgression = progression;
	    this._carParts[^1].splineProgressionAdd = 0;
	    // for (int i = this._carParts.Length; i > 0; i--)
	    // {
		   //  float value =  progression /*- this.GetOffsetFloatSpline(this._offsetCar) * ((this._carParts.Length-1) - i)*/;
		   //  if (value > 0) this._carParts[i].splineProgression = value;
		   //  else this._carParts[i].splineProgression = 1f-value;
	    // }
	    this.StopMove(false);
    }
    public float GetOffsetFloatSpline(float distance)
    {
	    return distance / this._splineContainer.Spline.GetLength();
    }
    public static float WorldToSplineT(SplineContainer container, Vector3 worldPos)
    {
	    var spline = container.Spline;

	    // Les splines sont stockées en local : convertir world -> local
	    float3 local = container.transform.InverseTransformPoint(worldPos);

	    // t le plus proche sur la spline
	    SplineUtility.GetNearestPoint(spline, local, out _, out float t);

	    // t est déjà normalisé 0..1
	    return Mathf.Repeat(t, 1f);
    }
    public void RemoveSpline(SplineContainer spline)
    {
	    this._board.autoSort.RemoveCart(this);
	    foreach (CarPart carPart in this._carParts)
	    {
		    carPart.onSpline = false;
		    carPart.splineProgression = 0;
		    carPart.splineProgressionAdd = 0;
	    }
        this._splineContainer = spline;
        this.StopMove(false);
        this._stepMoving = StepMoving.OutOffLevel;
	    this._board.CheckWin();
    }
    
    // --- Stop Moving ---
    public void StopMove(bool on)
    {
	    if(on) this._board.CheckLose();
        this._stop = on;
        //if(on == false) this.ResetOffsetCart();
    }

    private void ResetOffsetCart()
    {
	    float offset = this.GetOffsetFloatSpline(this._offsetCar);

	    for (int i = 1; i < this.carParts.Length; i++)
	    {
		    float value = this._carParts[i-1].splineProgression - offset;
		    if (value > 0) this._carParts[i].splineProgression = value;
		    else this._carParts[i].splineProgression = 1f - value;
	    }
	    // if (this._carParts[i + 1].splineProgressionAdd >= offset)
	    // {
		   //  float value = this._carParts[i+1].splineProgression + offset;
					// 	
		   //  if (value > 0) this._carParts[i].splineProgression = value;
		   //  else this._carParts[i].splineProgression = 1f+value;
		   //  this._carParts[i].splineProgressionAdd = 0;
					// 	
		   //  this._carParts[i].onSpline = true;
	    // }
    }
    
	// --- On CarPart Collision ---
	public void OnCarPartCollisionWithSpline(CarPart carPart) {
		this._isRunningForward = false;
		this._isInReverse = false;
		this._stepMoving = StepMoving.enterOnSpline;
		
		// send to circuit
		this._board.carList.Remove(this);
		this._board.autoSort.AddCart(this);
		this._board.autoSort.AddObjContainer(this,carPart == this._carParts.Last());
	}
	public void OnCarPartCollisionWithAnotherCarPart(CarPart a, Collider other) {
		if (this._isRunningForward == false) return;
		if (a != this._carParts[0] && a != this._carParts.Last()) return;
		CarPart b = other.gameObject.GetComponent<CarPart>();
		this.StopMoving(a, b);
		this._isRunningForward = false;
		this.hasStoppedMovingThisFrame = true;
		this._isInReverse = false;
	}
	public void OnCarPartCollisionWithObstacle(CarPart a, Collider other) {
		if (this._isRunningForward == false || a != this._carParts[0]) return;
		this.StopMoving(a, other.transform);
		this._isRunningForward = false;
		this.hasStoppedMovingThisFrame = true;
		this._isInReverse = false;
	}
	public bool isOneOfOurOwnCarPartCollider(Collider col) {
		foreach (CarPart cp in this._carParts) {
			if (cp.boxCollider == col) return true;
		}
		return false;

	}
	// --- Add Obj ---
	public void AddObjSpawn(Obj obj)
    {
	    foreach (CarPart part in this._carParts)
	    {
		    if (part.currentObjs.Count < 2)
		    {
			    Transform t = part.carSeats[part.currentObjs.Count];
			    part.currentObjs.Add(obj);
			    obj.transform.SetParent(t);
			    obj.transform.localPosition = Vector3.zero;
			    this._currentObj.Add(obj,part);
			    return;
		    }
	    }
    }
    public Vector3 AddObjAnimate(Obj obj)
    {
	    foreach (CarPart part in this._carParts)
	    {
		    if (part.eColor == obj.type && part.currentObjs.Count < 2)
		    {
			    Transform t = part.carSeats[part.currentObjs.Count];
			    part.currentObjs.Add(obj);
			    obj.transform.SetParent(t);
			    this._currentObj.Add(obj,part);
			    return t.transform.position;
		    }
	    }
		return Vector3.zero;
    }
    
    
    // --- Check Slot ---
    public bool CanSwape(ObjType objType, List<Obj> currentObj, List<ObjType> exception, out List<Obj> SwapList)
    {
	    SwapList = new List<Obj>();
	    Dictionary<ObjType, List<Obj>> dict = new Dictionary<ObjType, List<Obj>>();
	    foreach (KeyValuePair<Obj,CarPart> valuePair in this._currentObj)
	    {
		    if(valuePair.Key.type == valuePair.Value.eColor) continue;
		    if (dict.ContainsKey(valuePair.Key.type))
		    {
			    dict[valuePair.Key.type].Add(valuePair.Key);
		    }
		    else
		    {
			    dict.Add(valuePair.Key.type,new List<Obj>(){valuePair.Key});
		    }
	    }
	    
	    foreach (KeyValuePair<ObjType,List<Obj>> valuePair in dict)
	    {
		    if(exception.Contains(valuePair.Key)) continue;
		    if (currentObj.Count <= valuePair.Value.Count)
		    {
			    SwapList = new List<Obj>(valuePair.Value);
		        return true;
		    }
	    }
	    return false;
    }
    public bool HaveTypeCart(ObjType type)
    {
	    foreach (CarPart part in this.carParts)
	    {
		    if (part.eColor == type) return true;
	    }
	    return false;
    }
    public bool NeedThisType(ObjType type, out int nbObj)
    {
	    int totalPlace = 0;
	    foreach (CarPart carPart in this._carParts)
	    {
		    if (carPart.eColor == type && carPart.currentObjs.Count < 2)
		    {
			    totalPlace += 2 - carPart.currentObjs.Count;
		    }
	    }
	    nbObj = totalPlace;
        return totalPlace > 0;
    }
    public bool HaveThisType(ObjType type, out List<Obj> result)
    {
        Dictionary<ObjType, List<Obj>> dict = new Dictionary<ObjType, List<Obj>>();
        foreach (KeyValuePair<Obj,CarPart> valuePair in this._currentObj)
        {
	        if(valuePair.Key.type == valuePair.Value.eColor) continue;
	        if (dict.ContainsKey(valuePair.Key.type))
	        {
		        dict[valuePair.Key.type].Add(valuePair.Key);
	        }
	        else
	        {
		        dict.Add(valuePair.Key.type,new List<Obj>(){valuePair.Key});
	        }
        }

        if (dict.TryGetValue(type, out result))
        {
            foreach (Obj obj in result)
            {
	            this._currentObj[obj].currentObjs.Remove(obj);
                this._currentObj.Remove(obj);
            }
            return true;
        }
        return false;
    }
    public bool HaveMoreOneType(List<ObjType> exception, out ObjType type, out List<Obj> result)
    {
        type = ObjType.None;
        result = new List<Obj>();
        Dictionary<ObjType, List<Obj>> dict = new Dictionary<ObjType, List<Obj>>();
        foreach (KeyValuePair<Obj,CarPart> valuePair in this._currentObj)
        {
	        if(exception.Contains(valuePair.Key.type)) continue;
	        if(valuePair.Key.type == valuePair.Value.eColor) continue;
	        if (dict.ContainsKey(valuePair.Key.type))
	        {
		        dict[valuePair.Key.type].Add(valuePair.Key);
	        }
	        else
	        {
		        dict.Add(valuePair.Key.type,new List<Obj>(){valuePair.Key});
	        }
        }
        
        if (dict.Keys.Count == 0)
        {
            return false;
        }
        
        type = dict.Keys.First();
        result = new List<Obj>(dict.Values.First());
        foreach (Obj obj in result)
        {
	        this._currentObj[obj].currentObjs.Remove(obj);
            this._currentObj.Remove(obj);
        }
        return true;
    }
    
    // --- Get Obj ---
    public List<List<Obj>> GetAllObj()
    {
        Dictionary<ObjType, List<Obj>> dict = new Dictionary<ObjType, List<Obj>>();
        foreach (Obj objValue in this._currentObj.Keys)
        {
            if (dict.ContainsKey(objValue.type))
            {
                dict[objValue.type].Add(objValue);
            }
            else
            {
                dict.Add(objValue.type,new List<Obj>(){objValue});
            }
        }
        return dict.Values.ToList();
    }
    public List<Obj> GetThisTypeObj(ObjType type)
    {
        List<Obj> result = new List<Obj>();
        foreach (KeyValuePair<Obj,CarPart> valuePair in this._currentObj)
        {
            if(valuePair.Key.type == type) result.Add(valuePair.Key);
        }
        foreach (Obj obj in result)
        {
            obj.transform.SetParent(null);
            this._currentObj.Remove(obj);
        }

        return result;
    }
    
    // --- Check Obj ---
    public bool IsFull()
    {
	    foreach (CarPart carPart in this._carParts)
	    {
		    if (carPart.currentObjs.Count < 2) return false;
		    else if (carPart.currentObjs.Count == 2)
		    {
			    foreach (Obj obj in carPart.currentObjs)
			    {
				    if (obj.type != carPart.eColor) return false;
			    }
		    }
		    else
		    {
			    return false;
		    }
	    }

	    return true;
    }

	// --- Raycast Detection ---
	private bool CheckMovingCarInFront(bool isWaitingForCarToMove) {
		for (int i = (this._isInReverse ? 1 : 0); i < (this._isInReverse ? 2 : 1); i++) {
			Transform currSpanw = this._raycastSpawns[i];
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(currSpanw.position, currSpanw.forward.normalized, out hit, isWaitingForCarToMove ? this._detectionDistForPathing + 1.0f : this._detectionDistForPathing, this._carLayer)) {
				CarPart carPart = hit.collider.gameObject.GetComponent<CarPart>();
				if (carPart.car.isOnSpline)
					return true;
			}
		}
		return false;
	}
	
	// --- Set Data ---
	public void SetPassengerForceData(PassengersData data) {
		this._forcePassengerData = data;
	}

	// --- Debug ---
	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		foreach (Transform spawn in this._raycastSpawns) {
			Gizmos.DrawLine(spawn.transform.position, spawn.transform.position + spawn.transform.forward * this._detectionDistForPathing);
		}
	}
}

