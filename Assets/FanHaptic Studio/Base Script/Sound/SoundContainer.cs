using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundType
{
    None,
    PressButton,
    ButtonCLick,
    ShowWinPanel,
    ShowLosePanel,
    AppearSFX,
    UnlockFeature,
    MoveObj,
    CollideWithCar,
    MoveCarInput
}
[System.Serializable]
public class SoundSettings
{
    public SoundType type;
    public AudioClip audioClip;
    [Range(0,256)] public int priroty = 128;
    [Range(0f,1f)] public float volume = 1;
    [Range(-3f,3f)] public float pitch = 1;
    public bool loop = false;
    public bool isSFX = true;
}

public class SoundContainer : MonoBehaviour
{
    private static SoundContainer Instance;

    public static bool ActiveSfxSound
    {
        get
        {
            return  SaveDataJsonInterface.GetBool("ActiveSfxSound");
        }
        set
        {
            SaveDataJsonInterface.SetBool("ActiveSfxSound",value);
        }
    }
    public static bool ActiveMusicSound
    {
        get
        {
            return SaveDataJsonInterface.GetBool("ActiveMusicSound");
        }
        set
        {
            SaveDataJsonInterface.SetBool("ActiveMusicSound",value);
        }
    }
    
    
    public static float soundVolume
    {
        get
        {
            return SaveDataJsonInterface.Exist<float>("SoundVolume") ? SaveDataJsonInterface.GetFloat("SoundVolume") : 1f;
        }
        set
        {
            SaveDataJsonInterface.SetFloat("SoundVolume", value);
        }
    }

    public static void PlaySound(SoundType type)
    {
        if(Instance != null)Instance._PlaySound(type);
    }
    public static void PlaySoundPriority(SoundType type)
    {
        if(Instance != null)Instance._PlaySoundPriority(type);
    }
    public static void StopSound(SoundType type)
    {
        if(Instance != null)Instance._StopSound(type);
    }
    
    //Audio Source
    private List<AudioSource> _audioList = new List<AudioSource>();

    //Sound Settings
    [SerializeField] private List<SoundType> _exception = new List<SoundType>();
    [SerializeField,TableList]private SoundSettings[] _soundSettings;
    private Dictionary<SoundType, SoundSettings> _soundDictionary = new Dictionary<SoundType, SoundSettings>();
    private Dictionary<SoundType, AudioSource> _dictSoundIsPlaying = new Dictionary<SoundType, AudioSource>();
    
    [Button]
    public void Init()
    {
        if(SaveDataJsonInterface.Exist<bool>("ActiveSfxSound") == false)SaveDataJsonInterface.SetBool("ActiveSfxSound",true);
        if(SaveDataJsonInterface.Exist<bool>("ActiveMusicSound") == false)SaveDataJsonInterface.SetBool("ActiveMusicSound",true);
        Instance = this;
        this._audioList = this.GetComponents<AudioSource>().ToList();
        this._soundSettings.ForEach(item =>
        {
            this._soundDictionary.Add(item.type, item);
        });
    }

    [Button]
    private void UpdateDico()
    {
        this._soundDictionary.Clear();
        this._soundSettings.ForEach(item =>
        {
            this._soundDictionary.Add(item.type, item);
        });
    }

    [Button]
    private void _PlaySound(SoundType type)
    {
        if(this._dictSoundIsPlaying.ContainsKey(type)) return;
        if (this._soundDictionary.ContainsKey(type) == false)
        {
            Debug.LogError($"Missing Sound Type: {type}");
            return;
        }
        
        SoundSettings s = this._soundDictionary[type];
        
        if(s.isSFX && ActiveSfxSound == false) return;
        if(s.isSFX == false && ActiveMusicSound == false) return;
        
        AudioSource audioSource = this.GetAudioSource();

        audioSource.pitch = s.pitch;
        audioSource.volume = s.volume * soundVolume;
        audioSource.priority = s.priroty;
        audioSource.clip = s.audioClip;
        audioSource.loop = s.loop;
        audioSource.Play();
        if(this._exception.Contains(type) == false)
        this._dictSoundIsPlaying.Add(type,audioSource);
    }
    private void _PlaySoundPriority(SoundType type)
    {
        SoundSettings s = this._soundDictionary[type];
        
        if(s.isSFX && ActiveSfxSound == false) return;
        if(s.isSFX == false && ActiveMusicSound == false) return;
        
        AudioSource audioSource = this.GetAudioSource();
        audioSource.pitch = s.pitch;
        audioSource.volume = s.volume;
        audioSource.priority = s.priroty;
        audioSource.clip = s.audioClip;
        audioSource.loop = s.loop;
        audioSource.Play();
        if(this._exception.Contains(type) == false)
        this._dictSoundIsPlaying.Add(type,audioSource);
    }

    private void _StopSound(SoundType type)
    {
        if (this._soundDictionary.ContainsKey(type) == false)
        {
            Debug.LogError($"Missing Sound Type: {type}");
            return;
        }
        SoundSettings s = this._soundDictionary[type];
        foreach (AudioSource audioSource in this._audioList)
        {
            if (audioSource.isPlaying && audioSource.clip == s.audioClip)
            {
                audioSource.Stop();
                break;
            }
        }
    }

    private AudioSource GetAudioSource()
    {
        AudioSource result = null;
        foreach (AudioSource source in this._audioList)
        {
            if (source.isPlaying == false)
            {
                result = source;
                break;
            }
        }

        if (result == null)
        {
            result = this.AddComponent<AudioSource>();
            this._audioList.Add(result);
        }
        
        return result;
    }

    private void LateUpdate()
    {
        for (int i = 0; i < this._dictSoundIsPlaying.Count; i++)
        {
            if (this._dictSoundIsPlaying.Values.ToList()[i].isPlaying == false || this._dictSoundIsPlaying.Values.ToList()[i].time >= this._dictSoundIsPlaying.Values.ToList()[i].clip.length *0.01f)
            {
                this._dictSoundIsPlaying.Remove(this._dictSoundIsPlaying.Keys.ToList()[i]);
            }
        }
    }
}