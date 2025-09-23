using System.ComponentModel;
using UnityEngine;
using UnityEngine.Diagnostics;

public partial class SROptions
{
    //------------------------------- CURRENCY ---------------------------------------------------------
    [Category("Currency")]
    public void Add100Gold() => InventoryInterface.AddCurrency(CurrencyType.Coin, 100);
    [Category("Currency")]
    public void Add100Trophy() => InventoryInterface.AddCurrency(CurrencyType.Trophy, 100);
    [Category("Currency")]
    public void Add1Life() => InventoryInterface.AddCurrency(CurrencyType.Life, 1);
    
    //------------------------------- SAVE ---------------------------------------------------------

    [Category("Save")]
    public void ResetSave()
    {
        SaveDataJsonInterface.DeleteJson();
        Application.Quit();
    }
    
    //--------------------------------- VIBRATION ---------------------------------------------------------
    // [Category("Vibration")]
    // public void LightVibration()
    // {
    //     VibrationInterface.VibrateLight();
    // }
    //
    // [Category("Vibration")]
    // public void MediumVibration()
    // {
    //     VibrationInterface.VibrateMedium();
    // }
    //
    // [Category("Vibration")]
    // public void HeavyVibration()
    // {
    //     VibrationInterface.VibrateHeavy();
    // }
    //
    // [Category("Vibration")]
    // public void SuccessVibration()
    // {
    //     VibrationInterface.VibrateSuccess();
    // }
    //
    // [Category("Vibration")]
    // public void FailureVibration()
    // {
    //     VibrationInterface.VibrateFailure();
    // }
    //
    // [Category("Vibration")]
    // public void WarningVibration()
    // {
    //     VibrationInterface.VibrateWarning();
    // }
    //
    // [Category("Vibration")]
    // public void SelectionVibration()
    // {
    //     VibrationInterface.VibrateSelection();
    // }
    
    //--------------------------------- Diagnostic ---------------------------------------------------------
    

    [Category("Diagnostic")]
    public void Crash()
    {
        Utils.ForceCrash(ForcedCrashCategory.FatalError);
    }

    [Category("Diagnostic")]
    public void ThrowException()
    {
        throw new System.Exception("Exception Test");
    }

    [Category("Diagnostic")]
    public void DebugException()
    {
        Debug.LogException(new System.Exception("Debug exception Test"));
    }
}