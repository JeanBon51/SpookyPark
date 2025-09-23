using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseCurrencyScriptable : SerializedScriptableObject
{
	
	[SerializeField] private CurrencyType _type;
	public CurrencyType type => this._type;
	public string nameCurrency => this._type.ToString();
}
