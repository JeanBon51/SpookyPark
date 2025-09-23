using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class FanHapticEditor : OdinMenuEditorWindow
{
    [MenuItem("FanHaptic Tool/Launch From Main Scene")]
    private static void PlayAtMainScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Scenes/Main.unity");
        EditorApplication.isPlaying = true;
    }
    
    [MenuItem("FanHaptic Tool/FanHaptic Editor")]
    private static void OpenWindow()
    {
        GetWindow<FanHapticEditor>().Show();
    }
    protected override void Initialize()
    {
        this.WindowPadding = Vector4.zero;
    }

    protected override object GetTarget()
    {
        return Selection.activeObject;
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;

        tree.Add("Save Reader", new SaveReader());
        // tree.Add("Utilities", new TextureUtilityEditor());
        tree.AddAllAssetsAtPath("InApp", "Assets/InApp", typeof(ScriptableObject), true, false);

        return tree;
    }
}
