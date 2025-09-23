using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/GifBank")]
public class TutoGifDataBankScriptable : ScriptableObject {
    public Dictionary<string, TutorialGifData> dictionary { get => this._dictionary; }
    private Dictionary<string, TutorialGifData> _dictionary = new Dictionary<string, TutorialGifData>();

    [SerializeField] private TutorialGifBankEntry[] _entries = null;

    public void InitData() {
        this._entries.ForEach(entry => { this._dictionary[entry.key] = entry.data; });
    }
}

[System.Serializable]
public struct TutorialGifBankEntry {
    public string key;
    public TutorialGifData data;
}

[System.Serializable]
public struct TutorialGifData {
    public string title;
    public string description;
    public Sprite[] frames;
}