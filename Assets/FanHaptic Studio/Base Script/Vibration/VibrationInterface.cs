using Lofelt.NiceVibrations;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
public class VibrationInterface
{
    private static string KeySave = "Vibration-Setting";
    
    private static bool _vibration = true;

    public static bool Vibration
    {
        get
        {
            return _vibration;
        }
        set
        {
            _vibration = value;
            VibrateMedium();
            SaveDataJsonInterface.SetBool(KeySave,value);
        }
    }
    
    private static int lightAmplitudeAtStart = 4;
    private static int mediumAmplitudeAtStart = 5;
    private static int heavyAmplitudeAtStart = 6;

    public static void SetSave()
    {
        if (SaveDataJsonInterface.Exist<bool>(KeySave)) _vibration = SaveDataJsonInterface.GetBool(KeySave);
    }
    public static bool HapticsSupported()
    {
        bool hapticsSupported = false;
#if UNITY_IOS
        DeviceGeneration generation = Device.generation;
        if ((generation == DeviceGeneration.iPhone3G) ||
            (generation == DeviceGeneration.iPhone3GS) ||
            (generation == DeviceGeneration.iPodTouch1Gen) ||
            (generation == DeviceGeneration.iPodTouch2Gen) ||
            (generation == DeviceGeneration.iPodTouch3Gen) ||
            (generation == DeviceGeneration.iPodTouch4Gen) ||
            (generation == DeviceGeneration.iPhone4) ||
            (generation == DeviceGeneration.iPhone4S) ||
            (generation == DeviceGeneration.iPhone5) ||
            (generation == DeviceGeneration.iPhone5C) ||
            (generation == DeviceGeneration.iPhone5S) ||
            (generation == DeviceGeneration.iPhone6) ||
            (generation == DeviceGeneration.iPhone6Plus) ||
            (generation == DeviceGeneration.iPhone6S) ||
            (generation == DeviceGeneration.iPhoneSE1Gen) ||
            (generation == DeviceGeneration.iPhone6SPlus))
        {
            hapticsSupported = false;
        }
        else
        {
            hapticsSupported = true;
        }
#else
        // pour l'instant sur Android on considère que l'haptique est toujours supporté, on verra si on porte le jeu sur ces tel de pouilleux comment on règle ça
        hapticsSupported = true;
#endif
        return hapticsSupported;
    }

    private static int lastVibration = 0;
    private static bool VibrationEnabled()
    {
        if (Vibration)
        {
            if (HapticsSupported() && lastVibration != Time.frameCount)
            {
                lastVibration = Time.frameCount;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static void VibrateSoft()
    {
        if (VibrationEnabled())
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        }
    }

    public static void VibrateLight()
    {
        if (VibrationEnabled())
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
    }

    public static void VibrateMedium()
    {
        if (VibrationEnabled())
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        }
    }

    public static void VibrateHeavy()
    {
        if (VibrationEnabled())  HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
    }

    public static void VibrateSuccess()
    {
        if (VibrationEnabled()) HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }

    public static void VibrateWarning()
    {
        if (VibrationEnabled()) HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
    }

    public static void VibrateFailure()
    {
        if (VibrationEnabled()) HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
    }

    public static void VibrateSelection()
    {
        if (VibrationEnabled()) HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

}