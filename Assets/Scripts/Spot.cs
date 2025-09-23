using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Sequence = DG.Tweening.Sequence;

public class Spot : MonoBehaviour
{
    //------------------ Static Variables -----------
    public static List<Spot> spotList = new List<Spot>();
    //------------------ Variables ------------------
    [SerializeField] private ColorPalette _palette;
    [SerializeField] private MeshRenderer[] _meshs;
    [SerializeField] private Transform _startPos1;
    [SerializeField] private Transform _posJump;
    [SerializeField] private Transform render;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _stickmanSpeed;
    [SerializeField] private PassengerGroupData _forcePassenger;
    [SerializeField] private Transform[] _animationsTransforms;
    [SerializeField] private TextMeshProUGUI[] _tNumber;
    
	private ObjType _type = ObjType.None; 
    private List<Obj> _currentObj = new List<Obj>();
    private List<Obj> _tempoList = new List<Obj>();
	private Coroutine _sortingRoutine = null;

    private Board _board;
	//------------------ AutoProperties -------------
	//------------------ Getter/Setter --------------
	public List<Obj> currentObj { get => this._currentObj; set => this._currentObj = value; }
	public PassengerGroupData forcePassenger { get => this._forcePassenger; set => this._forcePassenger = value; }

	//------------------ Unity Void -----------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarPart carPart) && carPart.car.stepMoving == StepMoving.onSpline && carPart.isHeadCart)
        {
            this.CheckSorting(carPart.car);
        }
    }
	//------------------ Void -----------------------
    public void Init(Board b)
    {
        this._board = b;
        spotList.Add(this);
    }
    private bool CheckSorting(Car car)
    {
        if (this._type == ObjType.None)
        {
            if (car.HaveMoreOneType(this.GetObjTypeException(),out ObjType t,out List<Obj> result))
            {
                if (result.Count > 0)
                {
                    car.StopMove(true);
                    this.SetType(t);
                    this._sortingRoutine = StartCoroutine(this.SortingRoutine(car,result));
                }
                return true;
            }
        }
        else
        {
            bool sameType = car.HaveTypeCart(this._type);
            if (sameType && (car.NeedThisType(this._type,out int nbObj)))
            {
                car.StopMove(true);
                this._sortingRoutine = StartCoroutine(this.FillRoutine(car,nbObj));
                return true;  
            }
            else if (car.HaveThisType(this._type,out List<Obj> result))
            {
                car.StopMove(true);
                this._sortingRoutine = StartCoroutine(this.SortingRoutine(car,result));
                return true;  
            }
        }

        return false;
    }
    //------------------ Void -----------------------
    IEnumerator SortingRoutine(Car car,List<Obj> objs)
    {
        foreach (Obj obj in objs)
        {
            obj.transform.SetParent(null);
            this.AddObjInList(obj);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        this._sortingRoutine = null;
        car.StopMove(false);
        this._board.autoSort.UpdateCartNb();
        yield return null;
        
    }
    IEnumerator FillRoutine(Car car, int nbFreePlace)
    {
        this._tempoList = this._currentObj.GetRange(0, nbFreePlace < this._currentObj.Count ? nbFreePlace : this._currentObj.Count);
        this.OpenDoor(true);
        List<Obj> t = new List<Obj>(this._tempoList);
        for (int i = 0; i < t.Count; i++)
        {
            this.GoToCart(car, t[i]);
            yield return new WaitForSeconds(0.10f);
        }

        yield return new WaitUntil(() => this._tempoList.Count == 0);

        //Set type && clean routine
        if (this._currentObj.Count == 0) this.SetType(ObjType.None);
        this._sortingRoutine = null;
        car.StopMove(false);
        this._board.autoSort.UpdateCartNb();
        this.OpenDoor(false);
    }
    private void GoToCart(Car car,Obj o)
    {
        this.RemoveObjInList(o);
        Vector3 target = car.AddObjAnimate(o);
        if(o.sequence != null) o.sequence.Kill();
        o.transform.DOKill();
        o.sequence = DOTween.Sequence();
        
        //dir
        Vector3 dir = (target - o.transform.position).normalized;
        Vector3 pos = target - dir * 1;
        float duration = this.GetDuration(this._stickmanSpeed, Vector3.Distance(pos, o.transform.position));
        o.sequence.AppendCallback((() =>
        {
            o.AgentEnable(false);
            o.transform.LookAt(pos);
            o.transform.DOMove(pos,duration);
        }));
        o.sequence.AppendInterval(duration+0.05f);
        o.sequence.AppendCallback((() =>
        {
            SoundContainer.PlaySound(SoundType.MoveObj);
            VibrationInterface.VibrateMedium();
            o.transform.localPosition = Vector3.zero;
            o.transform.localRotation = quaternion.identity;
            o.transform.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo);
        }));
        o.sequence.AppendInterval(0.02f);
        o.sequence.AppendCallback((() =>
        {
            this._tempoList.Remove(o);
        }));
    }
    private void AddObjInList(Obj obj)
    {
        obj.Spot = this;
        if (this._currentObj.Count == 0)
        {
            this._currentObj.Add(obj);
        }
        else
        {
            this._currentObj.Insert(0,obj);
        }
        
        Vector3 pos = this._startPos1.transform.position + UnityEngine.Random.Range(-0.5f, 0.5f) * this._startPos1.transform.right
                                                         + UnityEngine.Random.Range(0, 0.8f) * this._startPos1.transform.forward;
        
        if(obj.sequence != null) obj.sequence.Kill();
        obj.transform.DOKill();
        obj.sequence = DOTween.Sequence();
        SoundContainer.PlaySound(SoundType.MoveObj);
        VibrationInterface.VibrateMedium();
        obj.sequence.Join(obj.transform.DOJump(pos + this._startPos1.transform.forward * 2,1,1,0.25f));
        obj.sequence.OnComplete((() =>
        {
            obj.AgentEnable(true);
            obj.SetTargetAI(this._startPos1.transform.position + this._startPos1.transform.forward * -5f);
        }));
        foreach (TextMeshProUGUI textMeshProUGUI in this._tNumber)
        {
            textMeshProUGUI.text = this._currentObj.Count.ToString();
        }
    }
    private void RemoveObjInList(Obj obj)
    {
        obj.transform.SetParent(null);
        this._currentObj.Remove(obj);
        foreach (TextMeshProUGUI textMeshProUGUI in this._tNumber)
        {
            textMeshProUGUI.text = this._currentObj.Count.ToString();
        }
    }
    public void SetType(ObjType t, bool useTween = true)
    {
        this._type = t;
        List<Material> ms = this._meshs[0].materials.ToList();
        if(t == ObjType.None) ms[0] = this._palette.materialNone;
        else ms[0] = this._palette.colorDict[t].materials[0];
        foreach (MeshRenderer mesh in this._meshs)
        {
            mesh.SetMaterials(ms);
        }

        this._animationsTransforms[0].transform.DOScale(new Vector3(1, 0.35f, 1), 0.15f).SetLoops(2, LoopType.Yoyo);
        this._animationsTransforms[1].transform.DOScale(new Vector3(1, 0.35f, 1), 0.15f).SetLoops(2, LoopType.Yoyo);
        this._animationsTransforms[1].transform.DOLocalRotate(new Vector3(0, 180, 0), 0.15f, RotateMode.LocalAxisAdd)
            .SetLoops(4, LoopType.Incremental);
        this._animationsTransforms[2].transform.DOScale(1.25f, 0.25f).SetLoops(4, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
    private List<Obj> GetNearestObj(Vector3 target, int numberObj)
    {
        if (numberObj >= this._currentObj.Count)
        {
            return this._currentObj;
        }
        else
        {
            Dictionary<float, Obj> dict = new Dictionary<float, Obj>();
            foreach (Obj o in this._currentObj)
            {
                dict.Add(Vector3.Distance(o.transform.position,target),o);
            }
                
            List<Obj> result = new List<Obj>();
            int nb = 0;
            foreach (var pair in dict.OrderBy(item => item.Key))
            {
                if(nb >= numberObj) break;
                result.Add(pair.Value);
                nb++;
            }
            return result;
        }
    }
    public float GetDuration(float speed, float lenght) => lenght / speed;
    private bool CheckSpotHaveAlready(ObjType t)
    {
        return spotList.Find(item => item._type == t) != null ? true : false;
    }
    private List<ObjType> GetObjTypeException()
    {
        List<ObjType> result = new List<ObjType>();
        foreach (Spot spot in spotList)
        {
            if(spot == null) continue;
            result.Add(spot._type);
        }

        return result;
    }
    private void OpenDoor(bool on)
    {
        if (on)
        {
            this._animationsTransforms[^2].transform.DOLocalRotate(new Vector3(0, 90, 0), .15f);
            this._animationsTransforms[^1].transform.DOLocalRotate(new Vector3(0, -90, 0), .15f);
        }
        else
        {
            this._animationsTransforms[^2].transform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
            this._animationsTransforms[^1].transform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
        }
    }
}
