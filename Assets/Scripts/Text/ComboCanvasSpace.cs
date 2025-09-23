using System.Collections;
using System.Collections.Generic;
using AllIn1SpringsToolkit;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ComboCanvasSpace : CustomText
{
    [SerializeField] private AnchoredPositionSpringComponent _spring;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _minFont;
    
    public override void PlayText(Vector3 startPos, int valueText, UnityAction onStart = null,UnityAction onComplete = null)
    {
        // --- Set Text ---
        this._text.text = $"Combo<size=110><color=yellow>X{valueText}";;
        this._text.fontSize = this._minFont;
        
        // --- Set Spring ---
        int r = (UnityEngine.Random.Range(-1, 1) == 0 ? 1 : -1);
        this.transform.localRotation = Quaternion.Euler(0, 0, r * 10);
        this._spring.SetCurrentValue(startPos + Vector3.up);
        this._spring.SetTarget((Vector2)startPos + Vector2.up * 80 + Vector2.right * -(r * 80));
        
        // --- Set Scale 0 ----
        this.transform.localScale = Vector3.zero;
        
        // --- Start Event ----
        onStart?.Invoke();
        
        // --- Sequence Animation
        Sequence seq = DOTween.Sequence();
        seq.Join(this.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.6f);
        seq.AppendCallback(()=>this.transform.DOScale(0, 0.25f).SetEase(Ease.InBack));
        seq.AppendInterval(0.26f);
        seq.AppendCallback((() =>
        {
            onComplete?.Invoke();
            Destroy(this.gameObject);
        }));
        seq.Play();
    }
}
