using UnityEngine;

public class GameState : MonoBehaviour
{
    private PanelsGroup _panelsGroup;
    [SerializeField] private GameContainer.State _state;
    public GameContainer.State state => this._state;
    public PanelsGroup PanelGroup => this._panelsGroup;
    protected UIContainer _uiContainer;
    
    
    public virtual void Initialization(UIContainer uiContainer)
    {
        this._uiContainer = uiContainer; 
        GameContainer.AddGameStateDict(this._state, this);
        
        this._panelsGroup= this.GetComponentInChildren<PanelsGroup>(true);
        this._panelsGroup.Initialization(this);
    }

    private void OnDestroy()
    {
        GameContainer.RemoveGameStateDict(this._state, this);
    }

    public virtual void ShowState()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void HideState()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void ShowPanel(TypeMenu menu)
    {
        if (this.PanelGroup.ContainsPanel(menu))
        {
            this.PanelGroup.SetPanel(menu);
        }
    }
}
