using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Palette")]
public class ColorPalette : ScriptableObject
{
    //Material Color
    [SerializeField] private List<PaletteItemColorData> _palette;
    [SerializeField] private List<Material> _baseCartColor;
    [SerializeField] private Material _materialNone;
    private Dictionary<ObjType, PaletteItemColorData> _colorDict = null;
    [SerializeField] public List<Material> hidenMaterials = new List<Material>();
    
    //Getter Mat
    public List<Material> baseCartColor => this._baseCartColor;
    public Material materialNone => this._materialNone;
    public Dictionary<ObjType, PaletteItemColorData> colorDict
    {
        get
        {
            if (this._colorDict == null || this._colorDict.Count == 0 || this._colorDict.Count != this._palette.Count)
            {
                this._colorDict = new Dictionary<ObjType, PaletteItemColorData>();
                foreach (PaletteItemColorData data in this._palette)
                {
                    this._colorDict.Add(data.type, data);
                }
            }

            return this._colorDict;
        }
    }

    [Button]
    public void ResetData()
    {
        this._colorDict = new Dictionary<ObjType, PaletteItemColorData>();
    }
}


[System.Serializable]
public class PaletteItemColorData
{
    public ObjType type;
    public List<Material> materials;
}


