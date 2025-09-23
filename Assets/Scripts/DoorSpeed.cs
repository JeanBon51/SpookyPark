using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpeed : MonoBehaviour
{
    [SerializeField] private bool _increasSpeed = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Car car))
        {
            if (_increasSpeed)
            {
                //car._speed *= 3f;
            }
            else
            {
                //car._speed /= 3f;
            }
        }
            
    }
}
