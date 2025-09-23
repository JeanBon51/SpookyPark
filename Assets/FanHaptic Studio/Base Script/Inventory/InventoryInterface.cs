using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

public enum CurrencyType
{
    None,
    Coin,
    Trophy,
    Life
}
public static class InventoryInterface
{
    //--------------------------------------------------------- ALL SAVE KEY --------------------------------------------------------------------------
    private const string keySaveChest = "Chest";
    private const string keySaveCurrency = "Currency";
    private const string keySavePower = "Power";
    private static string GetCurrencyKey(CurrencyType type) => $"{type.ToString()}-{keySaveCurrency}";
   
    //--------------------------------------------------------- CURRENCY ------------------------------------------------------------------------------
    
    public static UnityEvent<CurrencyType,int> onCurrencyValueChange = new UnityEvent<CurrencyType,int>();
    public static UnityEvent<CurrencyType> onCurrencyValueChange2 = new UnityEvent<CurrencyType>();
    
                        //INT
    public static bool CurrencySaveExist(CurrencyType type) => SaveDataJsonInterface.Exist<int>(GetCurrencyKey(type));
    public static int GetCurrencySave(CurrencyType type) => SaveDataJsonInterface.GetInt(GetCurrencyKey(type));
    public static void SetCurrencySave(CurrencyType type, int value) => SaveDataJsonInterface.SetInt(GetCurrencyKey(type), value);
    
                        //BIG INT
    public static bool CurrencySaveExistBigInteger(CurrencyType type) => SaveDataJsonInterface.Exist<BigInteger>(GetCurrencyKey(type));
    public static BigInteger GetCurrencySaveBigInteger(CurrencyType type) => SaveDataJsonInterface.GetBigInteger(GetCurrencyKey(type));
    public static void SetCurrencySave(CurrencyType type, BigInteger value) => SaveDataJsonInterface.SetBigInteger(GetCurrencyKey(type), value);
    
    public static bool AddCurrency(CurrencyType type,int amount)
    {
        int value = 0;
        if (CurrencySaveExist(type))
        {
            value = GetCurrencySave(type);
        }
        value = value + amount;
        SetCurrencySave(type,value);
        onCurrencyValueChange?.Invoke(type,value);
        onCurrencyValueChange2?.Invoke(type);
        return true;
    }
    public static bool RemoveCurrency(CurrencyType type, int amount)
    {
        int value = 0;
        if (CurrencySaveExist(type))
        {
            value = GetCurrencySave(type);
        }
        if (value - amount < 0) return false;
        else
        {
            value = value - amount;
            SetCurrencySave(type,value);
            onCurrencyValueChange?.Invoke(type,value);
            onCurrencyValueChange2?.Invoke(type);
            return true;
        }
    }
    
    public static bool AddCurrency(CurrencyType type,BigInteger amount)
    {
        BigInteger value = 0;
        if (CurrencySaveExistBigInteger(type))
        {
            value = GetCurrencySaveBigInteger(type);
        }
        value = value + amount;
        SetCurrencySave(type,value);
        return true;
    }
    public static bool RemoveCurrency(CurrencyType type, BigInteger amount)
    {
        BigInteger value = 0;
        if (CurrencySaveExistBigInteger(type))
        {
            value = GetCurrencySaveBigInteger(type);
        }
        if (value - amount < 0) return false;
        else
        {
            value = value - amount;
            SetCurrencySave(type,value);
            return true;
        }
    }
    
    //--------------------------------------------------------- CHEST ---------------------------------------------------------------------------------

    public static void AddChest(ChestReward chestReward)
    {
        Dictionary<Guid, ChestRewardData> chestDict = new Dictionary<Guid, ChestRewardData>();
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            chestDict = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest);
        }

        ChestRewardData data = new ChestRewardData(chestReward);
        data.GenerateID();
        chestDict.Add(data.id,data);
        SaveDataJsonInterface.SetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest, chestDict);
        Debug.Log($"<color=lime>ADD</color> Chest ID : {data.id} ");
    }
    public static void AddChest(ChestRewardData chestReward)
    {
        Dictionary<Guid, ChestRewardData> chestDict = new Dictionary<Guid, ChestRewardData>();
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            chestDict = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest);
        }

        ChestRewardData data = new ChestRewardData(chestReward);
        data.GenerateID();
        chestDict.Add(data.id,data);
        SaveDataJsonInterface.SetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest, chestDict);
        Debug.Log($"<color=lime>ADD</color> Chest ID : {data.id} ");
    }
    public static void AddChest(List<ChestReward> listChestReward)
    {
        Dictionary<Guid, ChestRewardData> chestDict = new Dictionary<Guid, ChestRewardData>();
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            chestDict = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest);
        }

        foreach (ChestReward chest in listChestReward)
        {
            ChestRewardData data = new ChestRewardData(chest);
            data.GenerateID();
            chestDict.Add(data.id,data);
            Debug.Log($"<color=lime>ADD</color> Chest ID : {data.id} ");
        }
        SaveDataJsonInterface.SetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest, chestDict);
    }
    public static void AddChest(List<ChestRewardData> listChestReward)
    {
        Dictionary<Guid, ChestRewardData> chestDict = new Dictionary<Guid, ChestRewardData>();
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            chestDict = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest);
        }

        foreach (ChestRewardData chest in listChestReward)
        {
            ChestRewardData data = new ChestRewardData(chest);
            data.GenerateID();
            chestDict.Add(data.id,data);
            Debug.Log($"<color=lime>ADD</color> Chest ID : {data.id} ");
        }
        SaveDataJsonInterface.SetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest, chestDict);
    }
    public static bool ContainsChest(ChestRewardData chestReward) => ContainsChest(chestReward.id);
    public static bool ContainsChest(Guid idChest)
    {
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            Dictionary<Guid,ChestRewardData> chestDict = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest);
            return chestDict.ContainsKey(idChest);
        }
        return false;
    }
    public static void RemoveChest(ChestRewardData chestReward)
    {
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            Dictionary<Guid,ChestRewardData> chestDict = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest);
            chestDict.Remove(chestReward.id);
            SaveDataJsonInterface.SetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest, chestDict);
            Debug.Log($"<color=red>Remove</color> Chest ID : {chestReward.id}");
        }
    } 
    public static List<ChestRewardData> GetAllChest()
    {
        if (SaveDataJsonInterface.Exist<Dictionary<Guid,ChestRewardData>>(keySaveChest))
        {
            List<ChestRewardData> result = SaveDataJsonInterface.GetObject<Dictionary<Guid,ChestRewardData>>(keySaveChest).Values.ToList();
            result.ForEach(item => item.GenerateBaseList());
            return result;
        }

        return new List<ChestRewardData>();
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------
}
