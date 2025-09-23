using AllIn1SpringsToolkit;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TextWorldSpace : CustomText
{
    [SerializeField] private TransformSpringComponent _spring;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _minFont;

    public override void PlayText(Vector3 startPos, int valueText, UnityAction onStart = null,UnityAction onComplete = null)
    {
        // --- Set Text ---
        this._text.text = $"+{valueText}";
        this._text.fontSize = this._minFont;
        // --- Set Spring ---
        this._spring.SetCurrentValuePosition(startPos + Vector3.up);
        this._spring.SetTargetPosition(startPos+Vector3.up*0.75f);
        
        // --- Set Scale 0 ----
        this.transform.localScale = Vector3.zero;
        
        // --- Start Event ----
        onStart?.Invoke();
        
        // --- Sequence Animation
        Sequence seq = DOTween.Sequence();
        seq.Join(this.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.25f);
        seq.Join(this.transform.DOScale(0, 0.25f).SetEase(Ease.InBack));
        seq.AppendCallback((() =>
        {
            onComplete?.Invoke();
            Destroy(this.gameObject);
        }));
        seq.Play();
    }
}


// this._tScore.enableVertexGradient = false;
// this._tScore.color = c;
// this._tScore.text = $"+{valueText}";
// float fontValue = this._maxFont * this._curveFont.Evaluate(nbCombo / 10f);
// this._tScore.fontSize = fontValue < this._minFont2 ? this._minFont2 : fontValue;
// this._spring.SetCurrentValuePosition(startPos);
// this._spring.SetTargetPosition(startPos+Vector3.up*0.75f);
// this.transform.transform.localScale = Vector3.zero;
// this._spring.SetCurrentValueScale(Vector3.zero);
// this._spring.SetTargetScale(Vector3.one);
// Sequence seq = DOTween.Sequence();
// seq.AppendInterval(0.25f);
// seq.AppendCallback((() =>
// {
//     this._spring.SetTargetScale(Vector3.zero);
// }));
// seq.AppendInterval(0.15f);
// seq.AppendCallback((() =>
// {
//     Destroy(this.gameObject);
// }));
// seq.Play();