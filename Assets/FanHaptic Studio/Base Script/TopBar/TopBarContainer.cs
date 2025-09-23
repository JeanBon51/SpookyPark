using UnityEngine;

public class TopBarContainer : MonoBehaviour {
    private CurrencyTopBar _currencyTopBar;
    private UIContainer _uiContainer;
    
    public void Initialization(UIContainer uiContainer)
    {
        this._uiContainer = uiContainer;
        this._currencyTopBar = this.GetComponentInChildren<CurrencyTopBar>(true);
        this._currencyTopBar.Initialization(this);
    }
}
