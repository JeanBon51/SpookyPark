using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyTopBar_Item : MonoBehaviour
{
    [SerializeField] private Image _iconCurrency;
    [SerializeField] private TextMeshProUGUI _amountCurrency;
    [SerializeField] private Button _buttonCross;
    [SerializeField] private bool _bigInterger = false;

    [SerializeField, HideIf("_bigInterger")]
    private CurrencyScriptable _currentCurrency;

    [SerializeField, ShowIf("_bigInterger")]
    private BigCurrencyScriptable _currentBigCurrency;

    public void Initialization()
    {
        if (this._bigInterger) this.InitBigCurrency();
        else this.InitSimpleCurrency();
    }

    private void InitSimpleCurrency()
    {
        this._amountCurrency.text = this._currentCurrency.value.ToString();
        this._currentCurrency.onValueChange.AddListener(UpdateAmount);
        InventoryInterface.onCurrencyValueChange.AddListener(UpdateAmount);
    }

    private void InitBigCurrency()
    {
        //this._iconCurrency.sprite = this._currentBigCurrency.iconCurrency;
        this._amountCurrency.text = this._currentBigCurrency.GetStringValue();
        this._currentBigCurrency.onValueChange.AddListener((i) =>
        {
            this._amountCurrency.text = this._currentBigCurrency.GetStringValue();
        });
    }

    private void UpdateAmount(CurrencyType type, int newAmout)
    {
        if (type == this._currentCurrency.type)
            this._amountCurrency.text = newAmout.ToString();
    }

    private void UpdateAmount(int newAmout)
    {
        this._amountCurrency.text = newAmout.ToString();
    }

    private void UpdateTimerText(string text)
    {
        this._amountCurrency.text = text;
    }
}