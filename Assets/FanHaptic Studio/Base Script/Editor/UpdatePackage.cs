using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

public class UpdatePackage : MonoBehaviour
{
    private static Dictionary<string,string> packages = new Dictionary<string, string>()
    {
        {"com.eflatun.scenereference" , "git+https://github.com/starikcetin/Eflatun.SceneReference.git#upm" },
        { "com.unity.nuget.newtonsoft-json", "3.2.1" },
        { "com.unity.recorder", "4.0.2" },
        { "com.unity.memoryprofiler", "1.1.0" },
        { "com.unity.serialization", "3.1.1" },   
    };
    private static bool endLoad = false;
    static AddRequest Request;
    private static PackRequest PackRequest;
    private static ListRequest listRequest;

    [MenuItem("Window/Add Package Example"), RunAfterPackage("TestSDK")]
    static async void Add()
    {
        //EditorCoroutineUtility.StartCoroutineOwnerless(PackageRoutine());
        StreamReader file = File.OpenText("C:/Users/Julien/Documents/Repo/SDK-FanHaptic/SDK-FanHaptic/Packages/manifest.json");
        Char[] buffer;

        using (var sr = file) {
            buffer = new Char[(int)sr.BaseStream.Length];
            await sr.ReadAsync(buffer, 0, (int)sr.BaseStream.Length);
        }
        string result = new String(buffer);
        List<string> split = result.Split('}').ToList();
        
        foreach (KeyValuePair<string,string> p in packages)
        {
            split.Insert(split.Count - 2,",\r\n");
            split.Insert(split.Count - 2,'"' + p.Key + '"' + ":" + '"' + p.Value+ '"');
        }
        split.Add("\r\n}");
        split.Add("\r\n}");

        string r = "";
        foreach (string s in split)
        {
            r += s;
        }
        File.WriteAllText("C:/Users/Julien/Documents/Repo/SDK-FanHaptic/SDK-FanHaptic/Packages/manifest.json", r);
    }

    // static IEnumerator PackageRoutine()
    // {
    //     int index = 0;
    //     EditorUtility.DisplayProgressBar("Install Package", "Doing some work...", index / packages.Length);
    //     foreach (string p in packages)
    //     {
    //         endLoad = false;
    //         Request = Client.Add(p);
    //         EditorApplication.update += Progress;
    //         //yield return new WaitUntil(() => endLoad);
    //         while (endLoad == false)
    //         {
    //             yield return new WaitForEndOfFrame();
    //         }
    //         index += 1;
    //         EditorUtility.DisplayProgressBar("Install Package", "Doing some work...", index / packages.Length);
    //     }
    //     EditorUtility.ClearProgressBar();
    // }

    static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
                Debug.Log("Installed: " + Request.Result.packageId);
            else if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= Progress;
            endLoad = true;
        }
    }
}
