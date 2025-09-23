using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FanHaptic Component/UI/Infinity Scroll Raw Image")]
public class InfinityScrollRawImage : MonoBehaviour
{
   [SerializeField] private RawImage _image;
   [SerializeField] private Vector2 _scrollDir = new Vector2(1,1);
   [SerializeField] private float _speed = 0.15f;

   private void LateUpdate()
   {
      this._image.uvRect = new Rect(this._image.uvRect.position + this._scrollDir * this._speed * Time.deltaTime, this._image.uvRect.size);
   }
}
