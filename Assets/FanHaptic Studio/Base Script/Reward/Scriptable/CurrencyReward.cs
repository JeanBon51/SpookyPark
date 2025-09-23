using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "FanHaptic/Reward/CurrencyReward", fileName = "CurrencyReward")]
public class CurrencyReward : BaseReward
{
    [SerializeField] private BaseCurrencyScriptable _currency;

    //Fix
    [SerializeField, BoxGroup("FixAmount"),ShowIf("_isInt"), ShowIfGroup("FixAmount/_valueOption", Value = ValueOption.FixAmount)] private int _amount;
    [SerializeField, BoxGroup("FixAmount"),HideIf("_isInt"), ShowIfGroup("FixAmount/_valueOption", Value = ValueOption.FixAmount)] private SetterBigInteger _amountBigInteger;
    
    //Random
    [SerializeField,BoxGroup("RandomAmount"),ShowIf("_isInt"), ShowIfGroup("RandomAmount/_valueOption", Value = ValueOption.RandomAmount)] private Vector2Int _amountRange;
    [SerializeField,BoxGroup("RandomAmount"),HideIf("_isInt"), ShowIfGroup("RandomAmount/_valueOption", Value = ValueOption.RandomAmount)] private SetterBigInteger _minBigInt;
    [SerializeField,BoxGroup("RandomAmount"),HideIf("_isInt"), ShowIfGroup("RandomAmount/_valueOption", Value = ValueOption.RandomAmount)] private SetterBigInteger _maxBigInt;

    private float _multiplicateFactor = 1;
    private bool _isInt = false;

    public int amount => this._amount;
    public Vector2Int amountRange => this._amountRange;
    public float multiplicateFactor => this._multiplicateFactor;

    public BaseCurrencyScriptable currency => this._currency;
    public void SetFactor(float newFactor) => this._multiplicateFactor = newFactor;
    
    private void OnValidate()
    {
        if (this._currency != null)
        {
            if (this._currency is BigCurrencyScriptable) this._isInt = false;
            else this._isInt = true;
        }
    }

    public override bool CollectFixAmount()
    {
        if (this._isInt)
        {
            CurrencyScriptable c = this._currency as CurrencyScriptable;
            return c.AddCurrency((int)(this._amount * this._multiplicateFactor));
        }
        else
        {
            BigCurrencyScriptable bigC = this._currency as BigCurrencyScriptable;
            SetterBigInteger bigInteger = new SetterBigInteger(this._amountBigInteger);
            bigInteger.value = (int)(bigInteger.value * this._multiplicateFactor);
            return bigC.AddCurrency(bigInteger.GetBigInteger());
        }

        return false;
    }

    public override bool CollectRandomAmount()
    {
        if (this._isInt)
        {
            System.Random r = new System.Random();
            CurrencyScriptable c = this._currency as CurrencyScriptable;
            return c.AddCurrency(r.Next(this._amountRange.x,this._amountRange.y));
        }
        else
        { 
            //BigCurrencyScriptable bigC = this._currency as BigCurrencyScriptable;
            //return bigC.AddCurrency(this._amountBigInteger.GetBigInteger());
            return false;
        }
    }
}
