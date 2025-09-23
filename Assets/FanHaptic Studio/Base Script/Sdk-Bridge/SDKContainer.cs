using Sirenix.OdinInspector;
using SRDebugger;
using UnityEngine;

public class SDKContainer : MonoBehaviour
{
    public IBugReporterHandler test;
    private void Awake()
    {
        EventInterface.InitEventInterface();
    }
    
    [Button]
    public void ShowBugReportSheet()
    {
        SRDebug.Instance.ShowBugReportSheet(); 
    }
    
    [Button]
    public void GetBugReportSheet()
    {
        SRDebug.Instance.SetBugReporterHandler(test); 
    }
}
