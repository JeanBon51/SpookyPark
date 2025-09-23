using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SaveReaderGUI : OdinValueDrawer<SaveReader>
{
    private static Dictionary<string, Func<string, object, object>> DictTypeGUI =
        new Dictionary<string, Func<string, object, object>>()
        {
            { "Int", ObjVariableGui.IntGui },
            { "Int32", ObjVariableGui.IntGui },
            { "Int64", ObjVariableGui.IntGui },
            { "Bool", ObjVariableGui.BoolGui },
            { "Single", ObjVariableGui.FloatGui },
            { "String", ObjVariableGui.StringGui },
        };

    public enum EnumType
    {
        None,
        Int,
        Float,
        Bool,
        String,
        Obj
    }

    private Dictionary<string, int> _dictInt = new Dictionary<string, int>();
    private Dictionary<string, float> _dictFloat = new Dictionary<string, float>();
    private Dictionary<string, bool> _dictBool = new Dictionary<string, bool>();
    private Dictionary<string, string> _dictString = new Dictionary<string, string>();
    private Dictionary<string, object> _dictObj = new Dictionary<string, object>();

    private Dictionary<string, string> _dictModifTempo = new Dictionary<string, string>();
    private Dictionary<string, object> _dictModifObjTempo = new Dictionary<string, object>();
    private Vector2 _scrollValue = Vector2.zero;
    private Vector2 _scrollValueObj = Vector2.zero;
    private EnumType _typeValue;
    private string _searchString = String.Empty;

    private EnumType typeValue
    {
        get { return _typeValue; }
        set
        {
            if (value != this._typeValue)
            {
                this.LoadSave();
                this._scrollValue = Vector2.zero;
                this._dictModifTempo.Clear();
                this._dictModifObjTempo.Clear();
                this._typeValue = value;
            }
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        this.LoadSave();
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUILayout.BeginHorizontal();
        this._searchString = EditorGUILayout.TextField(_searchString, new GUIStyle("SearchTextField"));
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if (this._searchString == "") this.ShowMenuType();
        else this.ShowResultSearch();
        if (GUILayout.Button("Open Save Folder"))
        {
            System.Diagnostics.Process.Start(PathManager.GetJsonDirectoryPath());
        }
        if (GUILayout.Button("Reset Memory Save"))
        {
            SaveDataJsonInterface.ClearLocalJsonReference();
        }
    }

    private void ShowMenuType()
    {
        typeValue = (EnumType)EditorGUILayout.EnumPopup("Type Value:", typeValue, new GUIStyle("PreviewPackageInUse"));
        GUILayout.Space(10);
        switch (this.typeValue)
        {
            case EnumType.Int:
                this.ShowDictPrimaryTypeGUI<int>(ref this._dictInt);
                break;
            case EnumType.Float:
                this.ShowDictPrimaryTypeGUI<float>(ref this._dictFloat);
                break;
            case EnumType.Bool:
                this.ShowDictPrimaryTypeGUI<bool>(ref this._dictBool);
                break;
            case EnumType.String:
                this.ShowDictPrimaryTypeGUI<string>(ref this._dictString);
                break;
            case EnumType.Obj:
                this.ShowDictObject(ref this._dictObj);
                break;
        }
    }

    private void ShowDictPrimaryTypeGUI<T>(ref Dictionary<string, T> dictionary)
    {
        if (dictionary.Count == 0)
        {
            GUILayout.Label("No Value Exist","CN StatusWarn");
            return;
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
        GUILayout.Label($"Value :", EditorStyles.boldLabel);
        GUILayout.Label("Save", EditorStyles.boldLabel, GUILayout.Width(50));
        GUILayout.Label("Del", EditorStyles.boldLabel, GUILayout.Width(25));
        EditorGUILayout.EndHorizontal();

        this._scrollValue = EditorGUILayout.BeginScrollView(this._scrollValue, new GUILayoutOption[] { GUILayout.ExpandHeight(false) });
        string[] keys = new string[dictionary.Count];
        dictionary.Keys.CopyTo(keys,0);
        foreach (string keyWithType in keys)
        {
            EditorGUILayout.BeginHorizontal();

            string key = keyWithType.Split('_')[0];
            GUILayout.Label(key, GUILayout.MaxWidth(250));

            if (this._dictModifTempo.ContainsKey(keyWithType) == false)
                this._dictModifTempo.Add(keyWithType, dictionary[keyWithType].ToString());

            if (dictionary[keyWithType] is int)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton(
                    (() => SaveDataJsonInterface.SetInt(key, int.Parse(this._dictModifTempo[keyWithType]))));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<int>(key);});
            }
            else if (dictionary[keyWithType] is bool)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton((() =>
                    SaveDataJsonInterface.SetBool(key, bool.Parse(this._dictModifTempo[keyWithType]))));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<bool>(key); });
            }
            else if (dictionary[keyWithType] is float)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton((() =>
                    SaveDataJsonInterface.SetFloat(key, float.Parse(this._dictModifTempo[keyWithType]))));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<float>(key); });
            }
            else if (dictionary[keyWithType] is string)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton((() => SaveDataJsonInterface.SetString(key, this._dictModifTempo[keyWithType])));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<string>(key); });
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("SaveAll"))
        {
            if (this._dictModifTempo.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in this._dictModifTempo)
                {
                    if (pair.Value != dictionary[pair.Key].ToString())
                    {
                        if (dictionary[pair.Key] is int && SaveDataJsonInterface.Exist<int>(pair.Key))
                            SaveDataJsonInterface.SetInt(pair.Key.Split('_')[0],
                                int.Parse(this._dictModifTempo[pair.Key]));
                        else if (dictionary[pair.Key] is bool && SaveDataJsonInterface.Exist<bool>(pair.Key))
                            SaveDataJsonInterface.SetBool(pair.Key.Split('_')[0],
                                bool.Parse(this._dictModifTempo[pair.Key]));
                        else if (dictionary[pair.Key] is float && SaveDataJsonInterface.Exist<float>(pair.Key))
                            SaveDataJsonInterface.SetFloat(pair.Key.Split('_')[0],
                                float.Parse(this._dictModifTempo[pair.Key]));
                        else if (dictionary[pair.Key] is string && SaveDataJsonInterface.Exist<string>(pair.Key))
                            SaveDataJsonInterface.SetString(pair.Key.Split('_')[0], this._dictModifTempo[pair.Key]);
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
    private void ShowDictObject(ref Dictionary<string, object> dictionary)
    {
        if (dictionary.Count == 0)
        {
            GUILayout.Label("No Value Exist","CN StatusWarn");
            return;
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
        GUILayout.Label($"Value :", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        this._scrollValueObj =
            EditorGUILayout.BeginScrollView(this._scrollValueObj, new GUILayoutOption[] { GUILayout.ExpandHeight(false) });

        string[] keys = new string[dictionary.Count];
        dictionary.Keys.CopyTo(keys,0);
        foreach (string key in keys)
        {
            EditorGUILayout.BeginHorizontal("GroupBox",
                new GUILayoutOption[] { GUILayout.ExpandHeight(false), GUILayout.MinHeight(0) });

            GUILayout.TextField(key.Split('_')[0], GUILayout.MaxWidth(100));

            this.ShowObjectGUI(key, dictionary[key]);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
    private void ShowObjectGUI(string key, object obj)
    {
        List<object> listVariable = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Select(field => field.GetValue(obj)).ToList();
        List<string> listNameVariable = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Select(field => field.Name).ToList();

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < listVariable.Count; i++)
        {
            object v = listVariable[i];
            string nameV = listNameVariable[i];
            if (DictTypeGUI.ContainsKey(v.GetType().Name))
            {
                if(this._dictModifObjTempo.ContainsKey(key + nameV) == false) this._dictModifObjTempo.Add(key + nameV,v);
                this._dictModifObjTempo[key + nameV] = DictTypeGUI[v.GetType().Name]?.Invoke(nameV, this._dictModifObjTempo[key + nameV]);
            }
            else
            {
                this.ShowObjectInClass(key, nameV, v);
                //Debug.LogError($"Missing Type GUI : {v.GetType().Name}");
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            for (int i = 0; i < listVariable.Count; i++)
            {
                object v = listVariable[i];
                string nameV = listNameVariable[i];
                if (this._dictModifObjTempo.ContainsKey(key + nameV))
                {
                    obj = this.ModifyVariable(obj, nameV, this._dictModifObjTempo[key + nameV]);
                }
                else
                {
                    this.SaveObjectInClass(key, nameV, v);
                    //Debug.LogError($"Missing Type GUI : {v.GetType().Name}");
                }
            }

            Type t = obj.GetType();
            SaveDataJsonInterface.SetObject(key.Split('_')[0],obj);
        }

        if (GUILayout.Button("Delete"))
        {
            SaveDataJsonInterface.Remove(key.Split('_')[0],obj);
            this.LoadSave();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
    private object ModifyVariable(object obj ,string nameVariable, object value)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(nameVariable);
        fieldInfo.SetValue(obj,value);
        return obj;
    }
    private void SaveObjectInClass(string key, string name, object obj)
    {
        List<object> listVariable = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Select(field => field.GetValue(obj)).ToList();
        List<string> listNameVariable = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Select(field => field.Name).ToList();

        for (int i = 0; i < listVariable.Count; i++)
        {
            object v = listVariable[i];
            string nameV = listNameVariable[i];
            if(this._dictModifObjTempo.ContainsKey(key + name + nameV))
            {
                obj = this.ModifyVariable(obj, nameV, this._dictModifObjTempo[key + name + nameV]);
            }
            else
            {
                this.SaveObjectInClass(key + name, nameV, v);
                //Debug.LogError($"Missing Type GUI : {v.GetType().Name}");
            }
        }

    }
    private void ShowObjectInClass(string key, string name, object obj)
    {
        List<object> listVariable = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Select(field => field.GetValue(obj)).ToList();
        List<string> listNameVariable = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Select(field => field.Name).ToList();

        EditorGUILayout.BeginHorizontal("GroupBox",
            new GUILayoutOption[] { GUILayout.ExpandHeight(false), GUILayout.MinHeight(0) });
        GUILayout.Label(name, GUILayout.ExpandWidth(false));
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < listVariable.Count; i++)
        {
            object v = listVariable[i];
            string nameV = listNameVariable[i];
            if (DictTypeGUI.ContainsKey(v.GetType().Name))
            {
                if(this._dictModifObjTempo.ContainsKey(key + name + nameV) == false) this._dictModifObjTempo.Add(key + name + nameV,v);
                this._dictModifObjTempo[key + name + nameV] = DictTypeGUI[v.GetType().Name]?.Invoke(nameV, this._dictModifObjTempo[key + name + nameV]);
            }
            else
            {
                this.ShowObjectInClass(key + name, nameV, v);
                //Debug.LogError($"Missing Type GUI : {v.GetType().Name}");
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    private void ShowResultSearch()
    {
        Dictionary<string, int> intSearch = new Dictionary<string, int>();
        Dictionary<string, bool> boolSearch = new Dictionary<string, bool>();
        Dictionary<string, float> floatSearch = new Dictionary<string, float>();
        Dictionary<string, string> stringSearch = new Dictionary<string, string>();
        Dictionary<string, object> objectSearch = new Dictionary<string, object>();

        this._dictInt.ForEach(pair =>
        {
            if (pair.Key.Contains(this._searchString)) intSearch.Add(pair.Key, pair.Value);
        });
        this._dictBool.ForEach(pair =>
        {
            if (pair.Key.Contains(this._searchString)) boolSearch.Add(pair.Key, pair.Value);
        });
        this._dictFloat.ForEach(pair =>
        {
            if (pair.Key.Contains(this._searchString)) floatSearch.Add(pair.Key, pair.Value);
        });
        this._dictString.ForEach(pair =>
        {
            if (pair.Key.Contains(this._searchString)) stringSearch.Add(pair.Key, pair.Value);
        });
        this._dictObj.ForEach(pair =>
        {
            if (pair.Key.Contains(this._searchString)) objectSearch.Add(pair.Key, pair.Value);
        });

        if (intSearch.Count != 0 || boolSearch.Count != 0 || floatSearch.Count != 0 || stringSearch.Count != 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.MaxWidth(250));
            GUILayout.Label($"Value :", EditorStyles.boldLabel);
            GUILayout.Label("Save", EditorStyles.boldLabel, GUILayout.Width(50));
            GUILayout.Label("Del", EditorStyles.boldLabel, GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();
        }

        this._scrollValue =
            EditorGUILayout.BeginScrollView(this._scrollValue, new GUILayoutOption[] { GUILayout.ExpandHeight(false) });

        this.ShowSearchDictPrimaryTypeGUI<int>(ref intSearch);
        this.ShowSearchDictPrimaryTypeGUI<bool>(ref boolSearch);
        this.ShowSearchDictPrimaryTypeGUI<float>(ref floatSearch);
        this.ShowSearchDictPrimaryTypeGUI<string>(ref stringSearch);
        this.ShowDictObject(ref objectSearch);

        EditorGUILayout.EndScrollView();
    }
    private void ShowSearchDictPrimaryTypeGUI<T>(ref Dictionary<string, T> dictionary)
    { 
        string[] keys = new string[dictionary.Count];
        dictionary.Keys.CopyTo(keys,0);
        foreach (string keyWithType in keys)
        {
            EditorGUILayout.BeginHorizontal();

            string key = keyWithType.Split('_')[0];
            GUILayout.Label(key, GUILayout.MaxWidth(250));

            if (this._dictModifTempo.ContainsKey(keyWithType) == false)
                this._dictModifTempo.Add(keyWithType, dictionary[keyWithType].ToString());

            if (dictionary[keyWithType] is int)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton(
                    (() => SaveDataJsonInterface.SetInt(key, int.Parse(this._dictModifTempo[keyWithType]))));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<int>(key); });
            }
            else if (dictionary[keyWithType] is bool)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton((() =>
                    SaveDataJsonInterface.SetBool(key, bool.Parse(this._dictModifTempo[keyWithType]))));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<bool>(key); });
            }
            else if (dictionary[keyWithType] is float)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton((() =>
                    SaveDataJsonInterface.SetFloat(key, float.Parse(this._dictModifTempo[keyWithType]))));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<float>(key); });
            }
            else if (dictionary[keyWithType] is string)
            {
                this._dictModifTempo[keyWithType] = GUILayout.TextField(this._dictModifTempo[keyWithType]);
                this.ShowSaveButton((() => SaveDataJsonInterface.SetString(key, this._dictModifTempo[keyWithType])));
                this.DeleteSaveButton(() => { SaveDataJsonInterface.Remove<string>(key); });
            }
            EditorGUILayout.EndHorizontal();
        }
        
    }
    public void ShowSaveButton(UnityAction onSave = null)
    {
        if (GUILayout.Button("Save", GUILayout.Width(50)))
        {
            onSave?.Invoke();
        }
    }
    public void DeleteSaveButton(UnityAction onDelete = null)
    {
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            onDelete?.Invoke();
            this._dictModifTempo.Clear();
            this.LoadSave();
        }
    }
    private void LoadSave()
    {
        this._dictInt.Clear();
        this._dictFloat.Clear();
        this._dictBool.Clear();
        this._dictString.Clear();
        this._dictObj.Clear();
        List<JProperty> listJToken = SaveDataJsonInterface.GetJsonFileAsJobject().Properties().ToList();
        if (listJToken.Count == 0) return;
        foreach (JProperty jProperty in listJToken)
        {
            string[] splitKey = jProperty.Name.Split('_');
            if (splitKey.Length < 2) continue;
            string t = splitKey[^1];
            switch (t)
            {
                case "i":
                    this._dictInt.Add(jProperty.Name, jProperty.Value.ToObject<int>());
                    break;
                case "f":
                    this._dictFloat.Add(jProperty.Name, jProperty.Value.ToObject<float>());
                    break;
                case "b":
                    this._dictBool.Add(jProperty.Name, jProperty.Value.ToObject<bool>());
                    break;
                case "s":
                    this._dictString.Add(jProperty.Name, jProperty.Value.ToObject<string>());
                    break;
                default:
                    try
                    {
                        this._dictObj.Add(jProperty.Name, jProperty.Value.ToObject(Type.GetType(t)));
                    }
                    catch
                    {
                        Debug.LogError($"Type doesn't exist : {t}");
                    }

                    break;
            }
        }
    }
}
