using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Parking : MonoBehaviour
{
    //------------------ Variables ------------------
	[SerializeField] private Transform _carParent = null;
	[SerializeField] private Transform _obstacleParent = null;
	[SerializeField] private Transform _limitParent = null;

    private Board _board;
	//------------------ AutoProperties -------------
	//------------------ Getter/Setter --------------
	public Transform carParent { get => this._carParent; }
	public Transform obstacleParent { get => this._obstacleParent; }
	public Transform limitParent { get => this._limitParent; }
    //------------------ Unity Void -----------------
    //------------------ Void -----------------------
    public void Init(Board board)
    {
        this._board = board;
    }
    
    //Ranger plus tard
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
}
