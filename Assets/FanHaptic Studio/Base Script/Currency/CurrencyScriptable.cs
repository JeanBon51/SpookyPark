using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "FanHaptic/Currency/IntCurrency", fileName = "CurrencyScriptable")]
public class CurrencyScriptable : BaseCurrencyScriptable
{
    public UnityEvent<int> onValueChange = new UnityEvent<int>();
    public UnityEvent onValueChange2 = new UnityEvent();

    private void OnValidate()
    {
        InventoryInterface.onCurrencyValueChange.RemoveListener(OnUpdateEvent);
        InventoryInterface.onCurrencyValueChange2.RemoveListener(OnUpdateEvent);
        InventoryInterface.onCurrencyValueChange.AddListener(OnUpdateEvent);
        InventoryInterface.onCurrencyValueChange2.AddListener(OnUpdateEvent);
    }
    
    private void OnUpdateEvent(CurrencyType type)
    {
        if(type == this.type) onValueChange2?.Invoke();
    }
    private void OnUpdateEvent(CurrencyType type, int amount)
    {
        if(type == this.type) onValueChange?.Invoke(amount);
    }

    public int value
    {
        get
        {
            if (InventoryInterface.CurrencySaveExist(this.type))
            {
                return InventoryInterface.GetCurrencySave(this.type);
            }
            else
            {
                return 0;
            }
        }
        protected set
        {
            InventoryInterface.SetCurrencySave(this.type, value);
            onValueChange?.Invoke(value);
            onValueChange2?.Invoke();
        }
    }

    [Button]
    public virtual bool AddCurrency(int amount)
    {
        return InventoryInterface.AddCurrency(this.type, amount);;
    }
    [Button]
    public virtual bool RemoveCurrency(int amount)
    {
        return InventoryInterface.RemoveCurrency(this.type, amount);
    }
    [Button]
    public virtual int GetCurrency()
    {
        return InventoryInterface.GetCurrencySave(CurrencyType.Life);
    }
}
