using OM.AC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LosePanel : Panel
{
    //Button
    [SerializeField] private Button _retryLevel;
    [SerializeField] private Button _backHome;

    //Win Text
    [SerializeField] private ACAnimatorPlayer _showAnimation;
    [SerializeField] private TextMeshProUGUI _tLevel;

    public override void Initialization(PanelsGroup panelsGroup)
    {
        this._retryLevel.onClick.AddListener(GameContainer.Instance.RetryGame);
        this._backHome.onClick.AddListener(GameContainer.Instance.BackHome);
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            this.Show();
    }


    public void Show()
    {
        SoundContainer.PlaySound(SoundType.ShowLosePanel);
        this._tLevel.text = $"Level {LevelContainer.GetLevelIndex()+1}\n<size=110><color=red>Lose</size>";
        this._showAnimation.Play();
    }
}