using UnityEngine;
using UnityEditor;

public class ObjVariableGui 
{
    public static object IntGui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName,GUILayout.ExpandWidth(false));
        variable = EditorGUILayout.IntField((int)variable);
                
        EditorGUILayout.EndHorizontal();
        return variable;
    }
    public static object BoolGui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName,GUILayout.ExpandWidth(false));
        variable = EditorGUILayout.Toggle((bool)variable);
                
        EditorGUILayout.EndHorizontal();
        return variable;
    }
    public static object FloatGui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName,GUILayout.ExpandWidth(false));
        variable = EditorGUILayout.FloatField((float)variable);
                
        EditorGUILayout.EndHorizontal();
        return variable;
    }
    public static object StringGui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName,GUILayout.ExpandWidth(false));
        variable = GUILayout.TextField(variable.ToString());
                
        EditorGUILayout.EndHorizontal();
        return variable;
    }
    public static void Vector2Gui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName,GUILayout.ExpandWidth(false));
        //EditorGUILayout.Vector2Field();
                
        EditorGUILayout.EndHorizontal();
    }
    public static void Vector3Gui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName);
        GUILayout.TextField(variable.ToString());
                
        EditorGUILayout.EndHorizontal();
    }
    public static void ColorGui(string variableName, object variable)
    {
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label(variableName);
        GUILayout.TextField(variable.ToString());
                
        EditorGUILayout.EndHorizontal();
    }
    
}
