using System;
using AllIn1SpringsToolkit;
using System.Collections.Generic;
using UnityEngine;

public class CarPart : MonoBehaviour {
	public bool yAxis => this._carRef.yAxis;
	public Vector3 size => this._boxCollider.size;
	public ObjType eColor { get => this._eColor; set => this._eColor = value; }
	[SerializeField, Header("Data")] private ObjType _eColor = ObjType.None;
	public MeshRenderer meshRenderer { get => this._meshRenderer; set => this._meshRenderer = value; }
	[SerializeField, Header("Refs")] private MeshRenderer _meshRenderer = null;

	public BoxCollider boxCollider { get => this._boxCollider; set => this._boxCollider = value; }
	[SerializeField] private BoxCollider _boxCollider = null;

	public Transform[] carSeats { get => this._carSeats; set => this._carSeats = value; }
	[SerializeField] private Transform[] _carSeats = null;

	public float offsetToNextCar { get => this._offsetToNextCar; set => this._offsetToNextCar = value; }
	[SerializeField, Header("Collisions")] private float _offsetToNextCar = 1.65f;

	[SerializeField] private TransformSpringComponent _spring = null;
	[SerializeField] private float _springForce = 5.0f;
	private List<Vector3> _springForceCue = new List<Vector3>(); 

	[Header("Spline")] public float splineProgression = 0;
	public float splineProgressionAdd = 0;
	public bool onSpline = false;
	public bool isHeadCart = false;
	private Car _carRef = null;
	
	public List<Obj> currentObjs = new List<Obj>();

	private bool _isInitiated = false;

	public Car car => this._carRef;

	public void Init(Car carRef, List<Material> materials, ObjType eColor, bool isHiden, bool isInRuntime) {
		this._carRef = carRef;
		this.eColor = eColor;
		this._isInitiated = true;
		this._meshRenderer.SetSharedMaterials(materials);
		this._meshRenderer.renderingLayerMask = isHiden ? 1 : GetLayer(eColor) + 1;
		if (isHiden == false && isInRuntime)
		{
			this._meshRenderer.renderingLayerMask = 1;
			for (int j = 1; j < 4; j++) {
				this._meshRenderer.materials[j].color = this._meshRenderer.materials[j].color * 0.85f;
			}
		}
	}

	public uint GetLayer(ObjType eColor)
	{
		switch (eColor)
		{
			case ObjType.None:
				return 0;
				break;
			case ObjType.Color_1:
				return 2;
				break;
			case ObjType.Color_2:
				return 4;
				break;
			case ObjType.Color_3:
				return 8;
				break;
			case ObjType.Color_4:
				return 16;
				break;
			case ObjType.Color_5:
				return 32;
				break;
			case ObjType.Color_6:
				return 64;
				break;
			case ObjType.Color_7:
				return 128;
				break;
			default:
				return 0;
		}
	}

	private void FixedUpdate() {
		while (this._springForceCue.Count > 0) {
			this._spring.AddVelocityRotation(this._springForceCue[0]);
			this._springForceCue.RemoveAt(0);
		}
	}

	public bool TryMove(Vector2 dragVector, int moveIndex) {
		if (this._carRef.stepMoving != StepMoving.onPark) return false;
		else this._carRef.TryMove(true, moveIndex);

		return true;
	}

	private void OnTriggerEnter(Collider other) {
		if (this._isInitiated == false || this._carRef.stepMoving != StepMoving.onPark) return;
		if(other.TryGetComponent(out CarPart carPart) && carPart.car.stepMoving != StepMoving.onPark) return;
		if (other.gameObject.CompareTag("Spline"))
			this._carRef.OnCarPartCollisionWithSpline(this);
		else if (other.gameObject.CompareTag("Car") && this._carRef.isOneOfOurOwnCarPartCollider(other) == false)
		{
			this._carRef.OnCarPartCollisionWithAnotherCarPart(this, other);
		}
		else if (other.transform.parent != null && other.gameObject.CompareTag("Obstacle"))
		{
			this._carRef.OnCarPartCollisionWithObstacle(this, other);
		}
	}

	public void BumpSpringEffect(bool negativeVal, bool side) {
		if(side) this._springForceCue.Add(new Vector3(0, 0, negativeVal ? -this._springForce : this._springForce));
		else this._springForceCue.Add(new Vector3(negativeVal ? -this._springForce : this._springForce, 0, 0));
		VibrationInterface.VibrateMedium();
		SoundContainer.PlaySound(SoundType.CollideWithCar);
	}

	public bool isFrontCarInMovement() {
		return this._carRef.isCarPartFirstCarInMovement(this);
	}
}
