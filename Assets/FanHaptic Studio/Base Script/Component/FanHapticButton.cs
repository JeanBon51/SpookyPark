using System;
using AllIn1SpringsToolkit;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

namespace UnityEngine.UI
{
#if UNITY_EDITOR
    
    using UnityEditor;
    
    [CustomEditor(typeof(FanHapticButton))]
    public class FanHapticButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            this.baseInspector();
        }

        protected virtual void baseInspector()
        {
            GUILayout.Space(10);
            GUILayout.Label("------------------ Base Setting ----------------------------------------------------------------------------------------",new GUIStyle("AM HeaderStyle"));
            FanHapticButton button = target as FanHapticButton;
            button.transition = Selectable.Transition.None;
            button.interactable = false;
            button.isActive = EditorGUILayout.Toggle("Interactable", button.isActive);
            
            //Event
            var prop = serializedObject.FindProperty("m_OnClick");
            EditorGUILayout.PropertyField(prop, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
    
#endif
    
    
    [AddComponentMenu("FanHaptic Component/UI/FanHaptic Button", 0)]
    [RequireComponent(typeof(SpringValueVector3))]
    public class FanHapticButton : Button
    {
        private float _transitionTime = 0.17f;
        private float _scaleFactor = 0.8f;
        public bool isActive = true;

        private SpringValueVector3 _springVector3;
        private RectTransform _rect;
        protected override void Awake()
        {
            this._springVector3 = GetComponent<SpringValueVector3>();
            if (this._springVector3 == null) this._springVector3 = this.AddComponent<SpringValueVector3>();
            this._springVector3.Initialize();
            this._springVector3.SetTarget(this.transform.localScale);
            this._springVector3.SetUnifiedForceAndDragEnabled(false);
            this._springVector3.SetDrag(8.5f);
            this._springVector3.SetForce(300);
            this._rect = this.GetComponent<RectTransform>();
            this._springVector3.OnValueUpdate.AddListener((Scale =>
            {
                this.transform.localScale = Scale;
            }));
            base.Awake();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.interactable) this.interactable = false;
            base.OnPointerDown(eventData);
            if (this.isActive)
            {
                SoundContainer.PlaySound(SoundType.PressButton);
                this._springVector3.SetTarget(Vector3.one * this._scaleFactor);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (this.isActive /*&& this.CheckFingerOnButton(eventData)*/)
            {
                onClick?.Invoke();
                //this._springVector3.SetTarget(Vector3.one);
                VibrationInterface.VibrateMedium();
                SoundContainer.StopSound(SoundType.PressButton);
                SoundContainer.PlaySound(SoundType.ButtonCLick);
            }
            this._springVector3.SetTarget(Vector3.one);
        }

        private bool CheckFingerOnButton(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == this.gameObject) return true;
            foreach (Transform t in this.transform)
            {
                if (eventData.pointerCurrentRaycast.gameObject == t.gameObject) return true;
            }
            //if (Vector2.Distance(eventData.position, this._rect.pos) < this._rect.rect.width * 0.5f) return true;
            return false;
        }
        
        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            onClick.Invoke();
        }
    }
}