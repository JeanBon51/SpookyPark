namespace UnityEngine.UI
{
#if UNITY_EDITOR

    using UnityEditor;

    [CustomEditor(typeof(PopupButton))]
    public class CustomPopupButtonEditor : FanHapticButtonInspector
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("------------------ Popup Setting ----------------------------------------------------------------------------------------",new GUIStyle("AM HeaderStyle"));
            PopupButton button = target as PopupButton;
            button.type = (PopupType)EditorGUILayout.EnumPopup("Popup Type", button.type);
            button.hideAllPopupOnClick = EditorGUILayout.Toggle("Hide All Popup OnClick", button.hideAllPopupOnClick);
            
            this.baseInspector();
        }
    }
#endif

    [AddComponentMenu("FanHaptic Component/UI/Popup Button", 1)]
    public class PopupButton : FanHapticButton
    {
        public PopupType type;
        public bool hideAllPopupOnClick = false;

        protected override void Awake()
        {
            base.Awake();
            this.onClick.AddListener(() =>
            {
                PopupContainer.StaticShowPopup(this.type, this.hideAllPopupOnClick);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onClick.RemoveAllListeners();
        }
    }
}