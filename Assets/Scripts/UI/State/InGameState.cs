public class InGameState : GameState
{
    public override void ShowState()
    {
        base.ShowState();
    }

    public override void HideState()
    {
        base.HideState();
        this.Reset();
    }

    public void Reset()
    {
        this.PanelGroup.SetPanel(TypeMenu.Playing);
        this.PanelGroup.HidePanel(TypeMenu.Lose);
        this.PanelGroup.HidePanel(TypeMenu.Win);
    }
}
