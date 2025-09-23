using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PoolReader : SerializedMonoBehaviour
{
    public Dictionary<string, List<Object>> dictPool = new Dictionary<string, List<Object>>();
    public void LateUpdate()
    {
        dictPool = PoolSystem.DictPool;
    }
}
