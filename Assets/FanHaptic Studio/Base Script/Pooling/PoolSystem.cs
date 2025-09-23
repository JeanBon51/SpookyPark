
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class PoolSystem
{
    public static Dictionary<string,List<Object>> DictPool => dictPool;
    private static Dictionary<string,List<Object>> dictPool = new Dictionary<string, List<Object>>();

    public static void ClearPool()
    {
        if(dictPool == null || dictPool.Count == 0) return;
        dictPool = new Dictionary<string, List<Object>>();
    }
    public static T Instantiate<T>(this T obj) where T : Object => obj.Instantiate(Vector3.zero, Quaternion.identity, null);
    public static T Instantiate<T>(this T obj,Vector3 position) where T : Object => obj.Instantiate(position, Quaternion.identity, null);
    public static T Instantiate<T>(this T obj,Vector3 position,Quaternion rotation) where T : Object => obj.Instantiate(position, rotation, null);
    public static T Instantiate<T>(this T obj,Vector3 position,Quaternion rotation,Transform parent) where T : Object
    {
        if (HasFreeObject(obj, out Object result))
        {
            GameObject g = result.GameObject();
            g.SetActive(true);
            g.transform.SetPositionAndRotation(position,rotation);
            SceneManager.MoveGameObjectToScene(g, SceneManager.GetActiveScene());
            g.transform.SetParent(parent);
            return (T) result;
        }
        else
        {
            T newObj = Object.Instantiate(obj,position,rotation,parent);
            return newObj;
        }
    }
    public static void Destroy<T>(this T obj) where T : Object
    {
        if(obj == null) return;
        if(obj is IPoolingInterface) ((IPoolingInterface)obj).OnDestroyPool();
        obj.GameObject().SetActive(false);
        obj.GameObject().transform.SetParent(null);
        Object.DontDestroyOnLoad(obj);
        AddToPool(obj);
    }
    private static string GetPoolKey(this Object obj)
    {
        if(obj is ParticleSystem || obj is GameObject) return obj.name.Split(new []{' ','(',')'})[0];
        else return obj.GetType().ToString();
    }
    private static bool HasFreeObject<T>(T obj, out Object result)  where T : Object
    {
        result = null;
        if (dictPool == null || dictPool.Count == 0) return false;
        string key = obj.GetPoolKey();
        if (dictPool.ContainsKey(key) && dictPool[key].Count > 0)
        {
            if (dictPool[key][0] == null)
            {
                dictPool[key].RemoveAll(item => item == null);
                if(dictPool[key].Count <= 0) return false;
            }
            result = dictPool[key][0];
            dictPool[key].RemoveAt(0);
            return true;
        }
        else
        {
            return false;
        }
    }
    private static void AddToPool(Object obj)
    {
        string key = obj.GetPoolKey();
        if (dictPool.ContainsKey(key))
        {
            if (dictPool[key].Contains(obj) == false)
            {
                dictPool[key].Add(obj);
            }
        }
        else
        {
            dictPool.Add(key,new List<Object>(){obj});
        }
    }
    private static void RemoveFromPool(Object obj)
    {
        string key =  obj.GetPoolKey();
        if (dictPool.ContainsKey(key) && dictPool[key] != null && dictPool[key].Contains(obj))
        {
            dictPool[key].Remove(obj);
        }
    }
}
