using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

public class PanelsGroup : MonoBehaviour
{
    public enum AnimationPanel
    {
        None,
        ForHorizontalPosition,
        ForVerticalPosition,
        SlideDown,
        SlideUp,
        SlideLeft,
        SlideRight,
        Popup,
        SlideDownWithFade,
        SlideUpWithFade,
        SlideLeftWithFade,
        SlideRightWithFade,
    }

    public enum TypePosition
    {
        NotSpecific,
        Horizontal,
        Vertical,
        Popup,
    }

    [System.Serializable]
    public class PanelSetting
    {
        public TypeMenu typePanel;
        public TypePosition typePosition;

        [ShowIf("typePosition", TypePosition.NotSpecific)]
        public AnimationPanel typeAnimation;

        public int order = 0;
        public bool startActive = false;
    }

    [System.Serializable]
    public class AnimationSetting
    {
        public AnimationPanel type;
        public Ease ease = Ease.Linear;
        public float time = 1f;
    }


    //-------------------------------------- For Animation ----------------------------------
    [TableList(ShowPaging = true), SerializeField]
    private PanelSetting[] _panelSettingArray;

    [TableList(ShowPaging = true), SerializeField]
    private AnimationSetting[] _animationSettingArray;

    private Dictionary<TypeMenu, PanelSetting> _panelSettingDict = new Dictionary<TypeMenu, PanelSetting>();

    private Dictionary<AnimationPanel, AnimationSetting> _animationSettingDict =
        new Dictionary<AnimationPanel, AnimationSetting>();

    //-------------------------------------- ------------------------------------------------

    [SerializeField] private Vector2 _baseResolution = new Vector2(1170, 2532);
    [SerializeField] private TypeMenu _basePanel;
    [ReadOnly, SerializeField] private Panel _currentPanel = null;
    [ReadOnly, SerializeField] private Panel _lastPanel = null;

    private RectTransform _rect;
    private Panel[] _panelArray = Array.Empty<Panel>();
    private GameState _gameState;

    public Panel[] panelArray => this._panelArray;
    public GameState gameState => this._gameState;

    public void Initialization(GameState gameState)
    {
        this._rect = this.GetComponent<RectTransform>();
        this._gameState = gameState;

        this._panelArray = this.GetComponentsInChildren<Panel>(true);
        this._panelArray.ForEach(item => item.Initialization(this));

        this._currentPanel = Array.Find(this._panelArray, item => item.type == this._basePanel);
        this.UpdateOrderPanel();

        this._panelSettingArray.ForEach(panelS => this._panelSettingDict.Add(panelS.typePanel, panelS));
        this._animationSettingArray.ForEach(animS => this._animationSettingDict.Add(animS.type, animS));
    }

    public void UdapteBaseResolution()
    {
        this._baseResolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        this.UpdateOrderPanel();
    }
    [Button]
    private void UpdateOrderPanelEditor()
    {
        this._panelArray = this.GetComponentsInChildren<Panel>(true);
        this.UpdateOrderPanel();
    }

    private void UpdateOrderPanel()
    {
        if (this._panelArray.Length == 0) this._panelArray = this.GetComponentsInChildren<Panel>(true);

        foreach (Panel panel in this._panelArray)
        {
            PanelSetting panelSetting = Array.Find(_panelSettingArray, item => item.typePanel == panel.type);
            if (panelSetting == null)
            {
                Debug.LogError($"Missing Panel Setting : {panel.type}");
                continue;
            }

            switch (panelSetting.typePosition)
            {
                case TypePosition.NotSpecific:
                    break;
                case TypePosition.Horizontal:
                    panel.rect.anchoredPosition = Vector2.right * (this._baseResolution.x * panelSetting.order);
                    break;
                case TypePosition.Vertical:
                    panel.rect.anchoredPosition = Vector2.up * (this._baseResolution.y * panelSetting.order);
                    break;
                case TypePosition.Popup:
                    panel.rect.anchoredPosition = Vector2.zero;
                    break;
            }

            panel.gameObject.SetActive(panelSetting.startActive);
        }
    }

    public Panel GetPanel(TypeMenu menu)
    {
        return Array.Find(this._panelArray, item => item.type == menu);
    }
    
    [Button]
    public void SetPanel(TypeMenu menu)
    {
        if (this._panelSettingDict[menu].typePosition == TypePosition.Horizontal)
        {
            this.HorizontalAnim(menu);
            return;
        }

        if (this._panelSettingDict[menu].typePosition == TypePosition.Vertical)
        {
            this.VerticalAnim(menu);
            return;
        }

        if (this._panelSettingDict[menu].typePosition == TypePosition.Popup)
        {
            this.Popup(menu);
            return;
        }

        switch (this._panelSettingDict[menu].typeAnimation)
        {
            case AnimationPanel.ForHorizontalPosition:
                this.HorizontalAnim(menu);
                break;
            case AnimationPanel.ForVerticalPosition:
                this.VerticalAnim(menu);
                break;
            case AnimationPanel.SlideDown:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.SlideUp:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.SlideLeft:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.SlideRight:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.Popup:
                this.Popup(menu);
                break;
            case AnimationPanel.SlideDownWithFade:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.SlideUpWithFade:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.SlideLeftWithFade:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.SlideRightWithFade:
                this.SlideAnim(menu);
                break;
            case AnimationPanel.None:
                this.SetActivePanel(menu);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void HidePanel(TypeMenu menu)
    {
        if (this._panelSettingDict[menu].typePosition == TypePosition.Popup)
        {
            this.Popup(menu,true);
            return;
        }

        switch (this._panelSettingDict[menu].typeAnimation)
        {
            case AnimationPanel.SlideDown:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.SlideUp:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.SlideLeft:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.SlideRight:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.Popup:
                this.Popup(menu);
                break;
            case AnimationPanel.SlideDownWithFade:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.SlideUpWithFade:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.SlideLeftWithFade:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.SlideRightWithFade:
                this.SlideAnim(menu,true);
                break;
            case AnimationPanel.None:
                this.SetActivePanel(menu,true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetActivePanel(TypeMenu menu, bool hide = false)
    {
        if (hide)
        {
            Array.Find(this._panelArray, item => item.type == menu).gameObject.SetActive(false);
            return;
        }
        if (this._currentPanel != null && menu == this._currentPanel.type) return;
        this._lastPanel = this._currentPanel;
        this._currentPanel = Array.Find(this._panelArray, item => item.type == menu);
        this._currentPanel.gameObject.SetActive(true);
    }

    public void VerticalAnim(TypeMenu menu)
    {
        if (menu == this._currentPanel.type) return;
        if (this._panelSettingDict.ContainsKey(menu) == false)
        {
            Debug.LogError($"Mission Panel Setting For {menu}");
            return;
        }

        PanelSetting panelSetting = this._panelSettingDict[menu];

        if (this._animationSettingDict.ContainsKey(AnimationPanel.ForVerticalPosition) == false)
        {
            Debug.LogError($"Mission Animation Setting For {AnimationPanel.ForVerticalPosition}");
            return;
        }

        AnimationSetting animationSetting = this._animationSettingDict[AnimationPanel.ForVerticalPosition];

        foreach (Panel panel in this._panelArray)
        {
            if (this._panelSettingDict[panel.type].typePosition != TypePosition.Vertical) continue;
            panel.rect.DOAnchorPosY(
                this._baseResolution.y * this._panelSettingDict[panel.type].order +
                this._baseResolution.y * -panelSetting.order, animationSetting.time).SetEase(animationSetting.ease);
        }

        this._lastPanel = this._currentPanel;
        this._currentPanel = Array.Find(this._panelArray, item => item.type == menu);
    }

    public void HorizontalAnim(TypeMenu menu)
    {
        if (menu == this._currentPanel.type) return;
        if (this._panelSettingDict.ContainsKey(menu) == false)
        {
            Debug.LogError($"Mission Panel Setting For {menu}");
            return;
        }

        PanelSetting panelSetting = this._panelSettingDict[menu];

        if (this._animationSettingDict.ContainsKey(AnimationPanel.ForHorizontalPosition) == false)
        {
            Debug.LogError($"Mission Animation Setting For {AnimationPanel.ForHorizontalPosition}");
            return;
        }

        AnimationSetting animationSetting = this._animationSettingDict[AnimationPanel.ForHorizontalPosition];

        foreach (Panel panel in this._panelArray)
        {
            if (this._panelSettingDict[panel.type].typePosition != TypePosition.Horizontal) continue;
            panel.rect.DOAnchorPosX(
                this._baseResolution.x * this._panelSettingDict[panel.type].order +
                this._baseResolution.x * -panelSetting.order, animationSetting.time).SetEase(animationSetting.ease);
        }

        this._lastPanel = this._currentPanel;
        this._currentPanel.Hide();
        this._currentPanel = Array.Find(this._panelArray, item => item.type == menu);
        this._currentPanel.Show();
    }

    public void SlideAnim(TypeMenu menu, bool hide = false)
    {
        bool show = menu != this._currentPanel.type;
        if (hide)
        {
            if(show == false) return;
            show = false;
        }
        else
        {
            //if (show) EventInterface.SendEvent($"Show Panel : {menu}");
            //else EventInterface.SendEvent($"Hide Panel : {menu}");
        }

        Panel panel = Array.Find(this._panelArray, item => item.type == menu);
        if (this._panelSettingDict.ContainsKey(menu) == false)
        {
            Debug.LogError($"Mission Panel Setting For {menu}");
            return;
        }

        PanelSetting panelSetting = this._panelSettingDict[menu];

        if (this._animationSettingDict.ContainsKey(panelSetting.typeAnimation) == false)
        {
            Debug.LogError($"Mission Animation Setting For {panelSetting.typeAnimation}");
            return;
        }

        AnimationSetting animationSetting = this._animationSettingDict[panelSetting.typeAnimation];

        if (show)
        {
            panel.gameObject.SetActive(show);
            if (panelSetting.typeAnimation == AnimationPanel.SlideDownWithFade ||
                panelSetting.typeAnimation == AnimationPanel.SlideLeftWithFade ||
                panelSetting.typeAnimation == AnimationPanel.SlideRightWithFade ||
                panelSetting.typeAnimation == AnimationPanel.SlideUpWithFade)
                panel.canvasGroup.alpha = 0;
        }

        panel.rect.DOKill();
        switch (panelSetting.typeAnimation)
        {
            case AnimationPanel.SlideDown:
                panel.rect.DOAnchorPosY(show ? 0 : this._baseResolution.y, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideUp:
                panel.rect.DOAnchorPosY(show ? 0 : -this._baseResolution.y, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideLeft:
                panel.rect.DOAnchorPosX(show ? 0 : this._baseResolution.x, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideRight:
                panel.rect.DOAnchorPosX(show ? 0 : -this._baseResolution.x, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideDownWithFade:
                panel.canvasGroup.DOFade(show ? 1 : 0, animationSetting.time).SetEase(animationSetting.ease);
                panel.rect.DOAnchorPosY(show ? 0 : this._baseResolution.y, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideUpWithFade:
                panel.canvasGroup.DOFade(show ? 1 : 0, animationSetting.time).SetEase(animationSetting.ease);
                panel.rect.DOAnchorPosY(show ? 0 : -this._baseResolution.y, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideLeftWithFade:
                panel.canvasGroup.DOFade(show ? 1 : 0, animationSetting.time).SetEase(animationSetting.ease);
                panel.rect.DOAnchorPosX(show ? 0 : this._baseResolution.x, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
            case AnimationPanel.SlideRightWithFade:
                panel.canvasGroup.DOFade(show ? 1 : 0, animationSetting.time).SetEase(animationSetting.ease);
                panel.rect.DOAnchorPosX(show ? 0 : -this._baseResolution.x, animationSetting.time)
                    .SetEase(animationSetting.ease)
                    .OnComplete(() => panel.gameObject.SetActive(show));
                ;
                break;
        }

        if (show && this._currentPanel != panel)
        {
            this._lastPanel = this._currentPanel;
            this._currentPanel = panel;
            panel.Show();
        }
        else
        {
            this._currentPanel = _lastPanel;
            this._lastPanel = panel;
            panel.Hide();
        }
    }

    public void Popup(TypeMenu menu, bool hide = false)
    {
        bool show = menu != this._currentPanel.type;

        if (hide)
        {
            if(show == false) return;
            show = false;
        }
        else
        {
            //if (show) EventInterface.SendEvent($"Show Panel : {menu}");
            //else EventInterface.SendEvent($"Hide Panel : {menu}");   
        }

        Panel panel = Array.Find(this._panelArray, item => item.type == menu);
        if (this._panelSettingDict.ContainsKey(menu) == false)
        {
            Debug.LogError($"Mission Panel Setting For {menu}");
            return;
        }

        PanelSetting panelSetting = this._panelSettingDict[menu];

        if (this._animationSettingDict.ContainsKey(panelSetting.typeAnimation) == false)
        {
            Debug.LogError($"Mission Animation Setting For {panelSetting.typeAnimation}");
            return;
        }

        AnimationSetting animationSetting = this._animationSettingDict[panelSetting.typeAnimation];

        if (show)
        {
            panel.rect.localScale = Vector3.zero;
            panel.gameObject.SetActive(show);
        }

        panel.rect.DOKill();
        panel.rect.DOScale(show ? Vector3.one : Vector3.zero, animationSetting.time).SetEase(animationSetting.ease)
            .OnComplete(() => panel.gameObject.SetActive(show));

        if (show && this._currentPanel != panel)
        {
            this._lastPanel = this._currentPanel;
            this._currentPanel = panel;
            panel.Show();
        }
        else
        {
            this._currentPanel = _lastPanel;
            this._lastPanel = panel;
            panel.Hide();
        }
    }

    public bool ContainsPanel(TypeMenu menu)
    {
        return Array.Find(this._panelArray, item => item.type == menu) != null;
    }
    public bool ContainsPanel(TypeMenu menu, out Panel panel)
    {
        panel = Array.Find(this._panelArray, item => item.type == menu);
        return panel != null;
    }
}