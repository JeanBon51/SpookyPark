using AllIn1SpringsToolkit;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public enum ObjType
{
    None = 0,
    Color_1 = 1,
    Color_2 = 2,
    Color_3,
    Color_4,
    Color_5,
    Color_6,
    Color_7,
}
public class Obj : MonoBehaviour
{
    //------------------ Variables ------------------
    [SerializeField, Header("Ref")] private ColorPalette _palette;
    [SerializeField, Header("Pathfinding")] private NavMeshAgent _agent;

	[SerializeField, Header("Spring")] private TransformSpringComponent _spring = null;
	[SerializeField] private float _moveForceFactor = 1.0f;
	[SerializeField, Range(0.01f, 0.1f)] private float _rotForceFactor = 0.075f;
	[SerializeField] private SpriteRenderer _spriteHidden;
	[SerializeField] private Transform _spritePivot;
	private Vector3 _lastPos = Vector3.zero;
	private Quaternion _lastRot = Quaternion.identity;
	private SkinnedMeshRenderer _meshRenderer = null;
	//------------------ AutoProperties -------------
	public ObjType type { get; private set; }
    //------------------ Getter/Setter --------------
    public Sequence sequence { get; set; }
    public Spot Spot { get; set; }
    //------------------ Unity Void -----------------
    //------------------ Void -----------------------
    public void Init(ObjType type, bool isHidden, bool isInRuntime = true) {
        this.type = type;
		this._lastPos = this.transform.position;
		this._lastRot = this.transform.rotation;
		this._spring.followerTransform = this._spring.transform;
		this._meshRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
		
		if(isHidden && isInRuntime) {
			this._meshRenderer.material = this._palette.hidenMaterials[0];
		}else {
			this._meshRenderer.material = this._palette.colorDict[type].materials[0];
			if (isInRuntime) this._meshRenderer.material.color = this._meshRenderer.material.color * 0.85f;
		}
		this._spriteHidden.enabled = isHidden;
		this._spritePivot.transform.rotation = Quaternion.Euler(this._spritePivot.transform.rotation.eulerAngles.x,0,this._spritePivot.transform.rotation.z);
		this.AgentEnable(false);
	}

	public void SetPassengerMovementEffect(bool isInReverse) {
		float moveForce = (this.transform.position - this._lastPos).magnitude;

		Vector3 currRot = this.transform.rotation * Vector3.up;

		float rotDiff = Vector3.SignedAngle(this._lastRot * Vector3.forward, this.transform.rotation * Vector3.forward, this._lastRot * Vector3.up);
		rotDiff *= this._rotForceFactor;

		this.UpdateSeatSpring(isInReverse ? -moveForce : moveForce, rotDiff);

		this._lastPos = this.transform.position;
		this._lastRot = this.transform.rotation;
	}

	private void UpdateSeatSpring(float moveFwrVal, float angleDiff) {
		this._spring.AddVelocityRotation(new Vector3(-angleDiff, 0, moveFwrVal) * this._moveForceFactor);
	}

	[Button]
    public void AgentEnable(bool on)
    {
        this._agent.enabled = on;
    }

    public void SetTargetAI(Vector3 target)
    {
        this._agent.SetDestination(target);
    }

	public void UnveilHiddenMat() {
		this._meshRenderer.material = this._palette.colorDict[this.type].materials[0];
		this._spriteHidden.enabled = false;
	}
}
