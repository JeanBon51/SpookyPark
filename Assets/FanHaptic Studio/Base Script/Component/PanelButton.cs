namespace UnityEngine.UI
{
#if UNITY_EDITOR
    
    using UnityEditor;
    
    [CustomEditor(typeof(PanelButton))]
    public class CustomPanelButtonEditor : FanHapticButtonInspector
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("------------------ Panel Setting ----------------------------------------------------------------------------------------",new GUIStyle("AM HeaderStyle"));
            PanelButton button = target as PanelButton;
            button.type = (TypeMenu)EditorGUILayout.EnumPopup("Panel Type", button.type);
            this.baseInspector();
        }
    }
#endif
    [AddComponentMenu("FanHaptic Component/UI/Panel Button", 0)]
    public class PanelButton : FanHapticButton
    {
        public TypeMenu type;
        
        protected override void Awake()
        {
            base.Awake();
            this.onClick.AddListener(() =>
                UIContainer.ShowPanel(this.type));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onClick.RemoveAllListeners();
        }
    }
}