using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CustomText : MonoBehaviour
{
   public abstract void PlayText(Vector3 startPos, int valueText, UnityAction onStart = null,UnityAction onComplete = null);
   
}
