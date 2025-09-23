using System.Collections.Generic;
using UnityEngine;

public class EventInterface : MonoBehaviour
{

    public static void InitEventInterface()
    {
    }
    
    public static void SendEvent(string eventName, IDictionary<string, object> properties = null)
    {
    }
    
    public static void SendUserProperty(string propertyName, IDictionary<string, object> properties = null)
    {
    }
    
    public static void AppendUserProperty(string propertyName, IDictionary<string, object> properties = null)
    {
    }
    
    public static void RemoveUserProperty(string propertyName, IDictionary<string, object> properties = null)
    {
    }


    public static void StartLevel()
    {
    }
    
    public static void WinLevel(int nbMove, int MaxMove)
    {
    }
    public static void LoseLevel()
    {
    }
    
    public static void BackHome()
    {
    }
}
