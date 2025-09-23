using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class CameraContainer : MonoBehaviour
{
    private static CameraContainer Instance;
    private Camera _camera;

    //----------------- Static Function -----------------
    public static void Shake(float strength = 0.2f)
    {
        Instance._camera.DOKill(true);
        Instance._camera.DOShakeRotation(0.5f, strength, 10, 90).OnComplete((() =>
        {
            Instance._camera.transform.rotation = quaternion.identity;
        }));
    }
	public static void SetCameraFOV(float FOV) {
		Instance._camera.fieldOfView = FOV;
	}

	//----------------- Unity Function -----------------
	private void Awake()
    {
        Instance = this;
        this._camera = this.GetComponent<Camera>();
    }
    
    //----------------- Function ------------------------
    [Button]
    public void Button(float strength = 0.2f)
    {
        this._camera.DOKill(true);
		this._camera.DOShakeRotation(0.5f, strength, 10, 90).OnComplete((() =>
        {
			this._camera.transform.rotation = quaternion.identity;
        }));
    }
}
