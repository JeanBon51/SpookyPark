using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class CheckEnd : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CarPart carPart))
        {
            if (carPart.car.stepMoving == StepMoving.onSpline && carPart.car.IsFull())
            {
                carPart.car.RemoveSpline(this._splineContainer);
            }
        }
            
    }
}
