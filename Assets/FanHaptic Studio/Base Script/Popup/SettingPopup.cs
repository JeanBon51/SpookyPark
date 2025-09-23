using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : BasePopup
{
    [SerializeField] private ToggleSwitch _toggleVibration;
    [SerializeField] private ToggleSwitch _toggleSoundFx;
    [SerializeField] private ToggleSwitch _toggleSoundMusic;
    [SerializeField] private FanHapticButton _buttonQuit;
    [SerializeField] private FanHapticButton _buttonRetry;
    // [SerializeField] private FanHapticButton _buttonRateUs;
    // [SerializeField] private FanHapticButton _buttonPrivacyPolicy;

    public override void Init(PopupContainer popupContainer)
    {
        base.Init(popupContainer);
        VibrationInterface.SetSave();
        this._toggleVibration.SetEnable(VibrationInterface.Vibration);
        this._toggleSoundFx.SetEnable(SoundContainer.ActiveSfxSound);
        this._toggleSoundMusic.SetEnable(SoundContainer.ActiveMusicSound);
        this._toggleVibration.OnSwitch.AddListener((activate => VibrationInterface.Vibration = activate));
        // this._buttonRateUs.onClick.AddListener(RateUs);
        // this._buttonPrivacyPolicy.onClick.AddListener(PrivacyPolice);
        this._toggleSoundFx.OnSwitch.AddListener((active =>
        {
            SoundContainer.ActiveSfxSound = active;
        }));
        this._toggleSoundMusic.OnSwitch.AddListener((active =>
        {
            SoundContainer.ActiveMusicSound = active;
            
        }));
        //this._buttonQuit.onClick.AddListener((() =>
        //{
        //    GameContainer.Instance.BackHome();
        //}));
        this._buttonRetry.onClick.AddListener((() =>
        {
            GameContainer.Instance.RetryGame();
            PopupContainer.HideAllPopup();
        }));
        this._buttonQuit.gameObject.SetActive(false);
    }

    public override void Show()
    {
        //this._buttonQuit.gameObject.SetActive(GameContainer.CurrentState == GameContainer.State.InGame && LevelContainer.GetLevelIndex() > 0);
        base.Show();
    }

    public void RateUs()
    {
#if UNITY_EDITOR
        Application.OpenURL ("https://play.google.com/store/apps/details?id=" + Application.productName);
        
#else
        Application.OpenURL ("market://details?id=" + Application.productName);
#endif
    }

    public void PrivacyPolice()
    {
        Application.OpenURL ("https://fanhapticstudio.com/privacy-policy");
    }
}