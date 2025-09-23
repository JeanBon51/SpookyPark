using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScriptTest : MonoBehaviour
{
    [SerializeField] private GameObject test;

    private List<GameObject> _objects = new List<GameObject>();
    [Button]
    public void CreateObj(int nb)
    {
        DateTime d = DateTime.Now;
        _objects = new List<GameObject>();
        for (int i = 0; i < nb; i++)
        {
            _objects.Add(test.Instantiate(Random.insideUnitSphere * 15f, Quaternion.identity));
        }
        Debug.Log($"Time : {(DateTime.Now - d).TotalSeconds}s" );
    }
    
    [Button]
    public void DestroyObj()
    {
        foreach (var obj in _objects)
        {
            obj.Destroy();
        }
    }
}
