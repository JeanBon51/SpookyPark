using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "FanHaptic/Currency/BigCurrency", fileName = "BigCurrencyScriptable")]
public class BigCurrencyScriptable : BaseCurrencyScriptable {
	public UnityEvent<BigInteger> onValueChange = new UnityEvent<BigInteger>();
	public UnityEvent onValueChange2 = new UnityEvent();

	public BigInteger value {
		get {
			if (InventoryInterface.CurrencySaveExistBigInteger(this.type)) {
				return InventoryInterface.GetCurrencySaveBigInteger(this.type);
			}
			else {
				return 0;
			}
		}
		protected set {
			InventoryInterface.SetCurrencySave(this.type,value);
			this.onValueChange?.Invoke(value);
			this.onValueChange2?.Invoke();
		}
	}

	[Button]
	public void AddCurrencyEditor(SetterBigInteger value) {
		this.AddCurrency(value.GetBigInteger());
	}

	[Button]
	public void RemoveCurrencyEditor(SetterBigInteger value) {
		this.RemoveCurrency(value.GetBigInteger());
	}

	public virtual bool AddCurrency(BigInteger amount) {
		this.value = this.value + amount;
		return true;
	}

	public virtual bool RemoveCurrency(BigInteger amount) {
		if (this.value - amount < 0)
			return false;
		else {
			this.value = this.value - amount;
			return true;
		}
	}

	[Button]
	public string GetStringValue() {
		return this.value.ToStringWithNotation();
	}

	public BigCurrencyScriptable(int value = 0) {
		this.value = value;
	}
}