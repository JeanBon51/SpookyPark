using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public static class TextInterface
{
    public enum TypeText
    {
        Base,
        Combo,
    }

    // --- Key Load Prefab ---
    // World Space Prefab
    private static Dictionary<TypeText, string> _dictWorldTextPath = new Dictionary<TypeText, string>()
    {
        { TypeText.Base, "TextPrefab/Text_World" },
    };
    
    // Canvas Space Prefab
    private static Dictionary<TypeText, string> _dictCanvasTextPath = new Dictionary<TypeText, string>()
    {
        { TypeText.Base, "TextPrefab/Text_Canvas" },
        { TypeText.Combo, "TextPrefab/Combo_Canvas" },
    };
    
    // --- Load Asset Dict ---
    private static Dictionary<TypeText, CustomText> _dictLoadTextCanvas = new Dictionary<TypeText, CustomText>();
    private static Dictionary<TypeText, CustomText> _dictLoadTextWorld = new Dictionary<TypeText, CustomText>();

    // --- Play Text Function ---
    
    
    // World To Canvas
    public static CustomText PlayTextWorldToCanvas(TypeText typeText, Vector3 startPos, int valueText, Transform parent)
    {
        return PlayTextWorldToCanvas(typeText,startPos,valueText,parent,null,null);
    }
    public static CustomText PlayTextWorldToCanvas(TypeText typeText, Vector3 startPos, int valueText, Transform parent, UnityAction onStart)
    {
        return PlayTextWorldToCanvas(typeText,startPos,valueText,parent,onStart,null);
    }
    public static CustomText PlayTextWorldToCanvas(TypeText typeText, Vector3 startPos, int valueText, Transform parent,UnityAction onStart,UnityAction onComplete)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(startPos);
        return PlayText(typeText,false,pos,valueText,parent,onStart,onComplete);
    }
    
    // Canvas To World
    public static CustomText PlayTextCanvasToWorld(TypeText typeText, Vector3 startPos, int valueText)
    {
        return PlayTextCanvasToWorld(typeText, startPos, valueText, null,null,null);
    }
    public static CustomText PlayTextCanvasToWorld(TypeText typeText, Vector3 startPos, int valueText, Transform parent)
    {
        return PlayTextCanvasToWorld(typeText,startPos,valueText,parent,null,null);
    }
    public static CustomText PlayTextCanvasToWorld(TypeText typeText, Vector3 startPos, int valueText, Transform parent, UnityAction onStart)
    {
        return PlayTextCanvasToWorld(typeText,startPos,valueText,parent,onStart,null);
    }
    public static CustomText PlayTextCanvasToWorld(TypeText typeText, Vector3 startPos, int valueText, Transform parent,UnityAction onStart,UnityAction onComplete)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(startPos);
        return PlayText(typeText,true,pos,valueText,parent,onStart,onComplete);
    }
    
    // World
    public static CustomText PlayTextWorld(TypeText typeText, Vector3 startPos, int valueText)
    {
        return PlayTextWorld(typeText, startPos, valueText, null,null,null);
    }
    public static CustomText PlayTextWorld(TypeText typeText, Vector3 startPos, int valueText, Transform parent)
    {
        return PlayTextWorld(typeText,startPos,valueText,parent,null,null);
    }
    public static CustomText PlayTextWorld(TypeText typeText, Vector3 startPos, int valueText, Transform parent, UnityAction onStart)
    {
        return PlayTextWorld(typeText,startPos,valueText,parent,onStart,null);
    }
    public static CustomText PlayTextWorld(TypeText typeText, Vector3 startPos, int valueText, Transform parent,UnityAction onStart,UnityAction onComplete)
    {
        return PlayText(typeText,true,startPos,valueText,parent,onStart,onComplete);
    }
    
    // Canvas
    public static CustomText PlayTextCanvas(TypeText typeText, Vector3 startPos, int valueText, Transform parent)
    {
        return PlayTextCanvas(typeText,startPos,valueText,parent,null,null);
    }
    public static CustomText PlayTextCanvas(TypeText typeText, Vector3 startPos, int valueText, Transform parent, UnityAction onStart)
    {
        return PlayTextCanvas(typeText,startPos,valueText,parent,onStart,null);
    }
    public static CustomText PlayTextCanvas(TypeText typeText, Vector3 startPos, int valueText, Transform parent,UnityAction onStart,UnityAction onComplete)
    {
        return PlayText(typeText,false,startPos,valueText,parent,onStart,onComplete);
    }
    
    // --- Base Function ---
    private static CustomText PlayText(TypeText typeText,bool worldSpace, Vector3 startPos, int valueText, Transform parent, UnityAction onStart,UnityAction onComplete)
    {
        CustomText text = GameObject.Instantiate(GetText(typeText, worldSpace),startPos,Quaternion.identity,parent);
        text.PlayText(startPos,valueText,onStart,onComplete);
        return text;
    }

    // ------- Load Text Prefab -------
    private static CustomText GetText(TypeText typeText, bool worldSpace)
    {
        CustomText result = null;
        if (worldSpace)
        {
            result = GetAsset(typeText, ref _dictWorldTextPath, ref _dictLoadTextWorld);
        }
        else
        {
            result = GetAsset(typeText, ref _dictCanvasTextPath, ref _dictLoadTextCanvas);
        }

        return result;
    }
    private static CustomText GetAsset(TypeText typeText, ref Dictionary<TypeText, string> key, ref Dictionary<TypeText, CustomText> loadAsset)
    {
        CustomText result = null;
        if (key.ContainsKey(typeText) == false)
        {
            Debug.LogError($"No Prefab for {typeText}");
            return null;
        }

        if (loadAsset.ContainsKey(typeText) == false)
        {
            result = Resources.Load<CustomText>(key[typeText]);
            if (result == null)
            {
                Debug.LogError($"Error load {typeText}");
                return null;
            }

            loadAsset.Add(typeText, result);
        }
        else result = loadAsset[typeText];

        return result;
    }
}
