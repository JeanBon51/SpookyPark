using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayingPanel : Panel
{
    [SerializeField] private Button _bReload;
    [SerializeField] private TextMeshProUGUI _tLevel;

    private void Awake()
    {
        this._tLevel.text = $"Level {LevelContainer.GetLevelIndex() + 1}";
        LevelContainer.onLoadComplete.AddListener((() =>
        {
            this._tLevel.text = $"Level {LevelContainer.GetLevelIndex() + 1}";
        }));
        _bReload.onClick.AddListener((() =>
        {
            GameContainer.Instance.RetryGame();
        }));
    }
}
