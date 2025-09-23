using System;
using UnityEngine;

public class TabsGroup : MonoBehaviour
{
    [SerializeField] private TypeMenu _baseTabs = TypeMenu.Home;

    private Tabs[] _tabsArray = Array.Empty<Tabs>();
    private GameState _gameState;
    private Tabs _currentTabsSelected = null;
    
    
    public void Initialization(GameState gameState)
    {
        this._gameState = gameState;
        this._tabsArray = this.GetComponentsInChildren<Tabs>(true);
        this._tabsArray.ForEach(item => item.Initialization(this));
        this.SetTabs(this._baseTabs);
    }

    private void SetTabs(TypeMenu menu)
    {
        this._currentTabsSelected = Array.Find(this._tabsArray, item => item.type == menu);
        if(this._currentTabsSelected == null) Debug.LogError($"Missing Tabs : {menu}");
        else
        {
            this._currentTabsSelected.SetFocus(true);
        }
    }

    public void ShowPanel(TypeMenu menu)
    {
        Tabs t = Array.Find(this._tabsArray, item => item.type == menu);
        if (t == this._currentTabsSelected) return;
        if(this._currentTabsSelected != null) this._currentTabsSelected.AnimationUnseletectTabs();
        t.AnimationSelectTabs();
        this._gameState.ShowPanel(menu);
        this._currentTabsSelected = t;
    }
}
