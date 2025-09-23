using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandFollow : MonoBehaviour
{
    private Tween tween;
    void Update()
    {
        this.GetComponent<RectTransform>().anchoredPosition =  new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.GetMouseButtonDown(0)) {
            if (this.tween != null) this.tween.Kill();
            this.tween = this.transform.DORotate(new Vector3(0,0,25), 0.15f).OnComplete((() => {
                this.transform.rotation = Quaternion.Euler(0, 0, 25);
            }));
        } else if (Input.GetMouseButtonUp(0)) {
			if (this.tween != null) this.tween.Kill();
			this.transform.DORotate(new Vector3(0, 0, 0), 0.15f).OnComplete((() => {
				this.transform.rotation = Quaternion.Euler(0, 0, 0);
			}));
		}
	}
}