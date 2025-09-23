using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurrencyTopBar : MonoBehaviour
{
     private TopBarContainer _topBarContainer;
     private List<CurrencyTopBar_Item> _currencyItemList = new List<CurrencyTopBar_Item>();

     public void Initialization(TopBarContainer topBarContainer)
     {
          this._topBarContainer = topBarContainer;
          this._currencyItemList = this.GetComponentsInChildren<CurrencyTopBar_Item>(true).ToList();
          foreach (CurrencyTopBar_Item c in this._currencyItemList)
          {
               c.Initialization();
          }
     }
}
