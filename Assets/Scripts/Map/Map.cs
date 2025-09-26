using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Map : MonoBehaviour{
    
    [System.Serializable]
    private class SettingColor
    {
        public Material mat;
        [HideIf("isTexture")]public Color Color;
        public bool isTexture = false;
        [ShowIf("isTexture")] public Texture text;
    }
    [System.Serializable]
    private class SettingMat
    {
        public List<SettingColor> _settingColors;
    }
    
    [SerializeField] private Board _board = null;
    [SerializeField] private List<SettingMat> _settingMat = null;
    public void InitMap(int levelIndex, int seed)
    {
        this._board ??= this.GetComponentInChildren<Board>();
		this._board.Init(levelIndex, seed);
        //this.SetMaterials();
    }

    private void SetMaterials()
    {
        int index = this.GetIndex();
        for (int i = 0; i < this._settingMat[index]._settingColors.Count; i++)
        {
            SettingColor sc = this._settingMat[index]._settingColors[i];
            if (sc.isTexture) sc.mat.mainTexture = sc.text;
            else sc.mat.color = sc.Color;
        }
    }
    
    private int GetIndex()
    {
        int level = LevelContainer.GetLevelIndex()+1;
        int index = Mathf.FloorToInt(level / 10f);
        while (index >= this._settingMat.Count)
        {
            index -= this._settingMat.Count;
        }

        return index;
    }
}
