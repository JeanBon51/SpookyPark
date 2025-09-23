using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class CurrencyRewardData : BaseRewardData
{
    [SerializeField] public CurrencyType _currencyType;
    [SerializeField] public bool isBigInteger;

    //--------------------------------------------- Fix ---------------------------------------------
    [SerializeField, BoxGroup("FixAmount"), HideIf("isBigInteger"), ShowIfGroup("FixAmount/valueOption", Value = ValueOption.FixAmount)]
    public int amount;

    [SerializeField, BoxGroup("FixAmount"), ShowIf("isBigInteger"), ShowIfGroup("FixAmount/valueOption", Value = ValueOption.FixAmount)]
    public SetterBigInteger amountBigInteger;

    //--------------------------------------------- Random ---------------------------------------------
    [SerializeField, BoxGroup("RandomAmount"), HideIf("isBigInteger"), ShowIfGroup("RandomAmount/valueOption", Value = ValueOption.RandomAmount)]
    public Vector2Int amountRange;

    [SerializeField, BoxGroup("RandomAmount"), ShowIf("isBigInteger"), ShowIfGroup("RandomAmount/valueOption", Value = ValueOption.RandomAmount)]
    public SetterBigInteger minBigInt;

    [SerializeField, BoxGroup("RandomAmount"), ShowIf("isBigInteger"), ShowIfGroup("RandomAmount/valueOption", Value = ValueOption.RandomAmount)]
    public SetterBigInteger maxBigInt;

    private int _amountRandom = 0;
    
    //------------------------------------------------------------------------------------------

    public CurrencyRewardData()
    {
        this.typeObj = this.GetType().Name;
    }
    public CurrencyRewardData(CurrencyReward currencyReward)
    {
        this.typeObj = this.GetType().Name;
        this.amount = currencyReward.amount;
        this.amountRange = currencyReward.amountRange;
        this._currencyType = currencyReward.currency.type;
        this.multiplicateFactor = currencyReward.multiplicateFactor;
        this.valueOption = currencyReward.valueOption;
    }
    
    public float multiplicateFactor = 1;
    public void SetFactor(float newFactor) => this.multiplicateFactor = newFactor;

    public override bool Collect()
    {
        bool result = base.Collect();
        if(result) Debug.Log($"<color=lime>Collect</color> Currency : {this._currencyType}");
        return result;
    }

    public override bool CollectFixAmount()
    {
        if (this.isBigInteger)
        {
            SetterBigInteger bigInteger = new SetterBigInteger(this.amountBigInteger);
            bigInteger.value = (int)(bigInteger.value * this.multiplicateFactor);
            return InventoryInterface.AddCurrency(this._currencyType,bigInteger.GetBigInteger());
        }
        else
        {
            return InventoryInterface.AddCurrency(this._currencyType, (int)(this.amount * this.multiplicateFactor));
        }

        return false;
    }

    public override bool CollectRandomAmount()
    {
        if (this.isBigInteger)
        {
            //BigCurrencyScriptable bigC = this._currency as BigCurrencyScriptable;
            //return bigC.AddCurrency(this._amountBigInteger.GetBigInteger());
            return false;
        }
        else
        {
            System.Random r = new System.Random();
            return InventoryInterface.AddCurrency(this._currencyType,this.GetAmount());
        }
    }

    public int GetAmount()
    {
        if (valueOption == ValueOption.FixAmount)
        {
            return amount;
        }
        else
        {
            System.Random r = new System.Random();
            if (this._amountRandom == 0) this._amountRandom = r.Next(this.amountRange.x, this.amountRange.y);
            return this._amountRandom;
        }

        return 0;
    }
}