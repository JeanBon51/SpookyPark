using System;
using UnityEngine;

[CreateAssetMenu(menuName = "FanHaptic/Palette/CurrencyPalette", fileName = "CurrencyPalette")]
public class CurrencyPaletteScriptable : ScriptableObject
{
    [SerializeField] private CurrencyPalette[] _palettes = Array.Empty<CurrencyPalette>();
    
    public CurrencyPalette GetPalette(CurrencyType type)
    {
        CurrencyPalette result = Array.Find(this._palettes, item => item.type == type);
        if (result == null)
        {
            Debug.LogError($"Missing Currency : {type} in Palette");
        }

        return result;
    }
}

[System.Serializable]
public class CurrencyPalette : BasePalette
{
    public CurrencyType type;
}
