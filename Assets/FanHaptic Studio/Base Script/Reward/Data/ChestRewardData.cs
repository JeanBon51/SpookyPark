using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public enum TypeChest
{
    Basic,
    Rare,
    Premium,
}

[System.Serializable]
public class ChestRewardData : BaseRewardData
{
    public Guid id = Guid.Empty;
    public TypeChest type = TypeChest.Basic;

    //--------------------------------------------- Fix ---------------------------------------------

    [SerializeReference, OnValueChanged("GenerateStringDataList"), BoxGroup("FixAmount"),
     ShowIfGroup("FixAmount/valueOption", Value = ValueOption.FixAmount)]
    private List<BaseRewardData> _rewardList = new List<BaseRewardData>();

    //--------------------------------------------- Random ------------------------------------------

    [SerializeReference, OnValueChanged("GenerateStringDataList"), BoxGroup("RandomAmount"),
     ShowIfGroup("RandomAmount/valueOption", Value = ValueOption.RandomAmount)]
    private List<BaseRewardData> _possibleRewardList = new List<BaseRewardData>();

    [SerializeField, BoxGroup("RandomAmount"),
     ShowIfGroup("RandomAmount/valueOption", Value = ValueOption.RandomAmount)]
    public int nbRandomReward = 0;

    [SerializeField, BoxGroup("RandomAmount"),
     ShowIfGroup("RandomAmount/valueOption", Value = ValueOption.RandomAmount)]
    public bool canHaveSameReward = false;

    private List<BaseRewardData> _rewardRandomSet = new List<BaseRewardData>();

    //-------------------------------------------- For Save --------------------------------------------

    [HideInInspector] public List<JObject> rewardStringList = new List<JObject>();
    [HideInInspector] public List<JObject> possibleRewardStringList = new List<JObject>();
    //--------------------------------------------------------------------------------------------------

    public ChestRewardData()
    {
    }

    public ChestRewardData(ChestReward chestReward)
    {
        this.type = chestReward.type;
        this.valueOption = chestReward.valueOption;
        this._rewardList = new List<BaseRewardData>(chestReward.rewardList);
        this._possibleRewardList = new List<BaseRewardData>(chestReward.possibleRewardList);
        this.nbRandomReward = chestReward.nbRandomReward;
        this.canHaveSameReward = chestReward.canHaveSameReward;
        this.GenerateStringDataList();
    }

    public ChestRewardData(ChestRewardData chestReward)
    {
        this.type = chestReward.type;
        this.valueOption = chestReward.valueOption;
        this._rewardList = new List<BaseRewardData>(chestReward._rewardList);
        this._possibleRewardList = new List<BaseRewardData>(chestReward._possibleRewardList);
        this.nbRandomReward = chestReward.nbRandomReward;
        this.canHaveSameReward = chestReward.canHaveSameReward;
        this.GenerateStringDataList();
    }

    public void GenerateID()
    {
        if (this.id == Guid.Empty) this.id = Guid.NewGuid();
        this.typeObj = this.GetType().Name;
        this.ClearRewardList();
    }

    public override bool Collect()
    {
        bool result = base.Collect();
        if (result)
        {
            Debug.Log($"Chest <color=lime>Collected</color> : {this.id}");
            if (this.id != Guid.Empty)
            {
                if (InventoryInterface.ContainsChest(this))
                {
                    InventoryInterface.RemoveChest(this);
                }
            }
        }
        return result;
    }

    public override bool CollectFixAmount()
    {
        bool allGood = true;
        foreach (BaseRewardData rewardData in this._rewardList)
        {
            if (rewardData is CurrencyRewardData)
            {
                CurrencyRewardData data = rewardData as CurrencyRewardData;
                data.Collect();
            }
            else if (rewardData.Collect() == false)
            {
                Debug.LogError($"Failed To Collect this reward : {rewardData}");
                allGood = false;
            }
        }

        return allGood;
    }

    public override bool CollectRandomAmount()
    {
        bool allGood = true;
        List<BaseRewardData> result = this.GetRewardData();

        foreach (BaseRewardData data in result)
        {
            if (data.Collect() == false)
            {
                Debug.LogError($"Failed To Collect this reward : {data}");
                allGood = false;
            }
        }

        return allGood;
    }

    public List<BaseRewardData> GetRewardData()
    {
        if (this.valueOption == ValueOption.FixAmount)
        {
            return this._rewardList;
        }
        else if (this.valueOption == ValueOption.RandomAmount)
        {
            System.Random r = new System.Random();
            if (this._rewardRandomSet.Count == 0)
            {
                if (this.canHaveSameReward)
                {
                    for (int i = 0; i < this.nbRandomReward; i++)
                    {
                        if (this._possibleRewardList.Count == 0) break;

                        BaseRewardData data = this._possibleRewardList[r.Next(0, this._possibleRewardList.Count)];
                        this._rewardRandomSet.Add(data);
                    }
                }
                else
                {
                    List<BaseRewardData> copyBaseList = new List<BaseRewardData>(_possibleRewardList);
                    for (int i = 0; i < this.nbRandomReward; i++)
                    {
                        if (copyBaseList.Count == 0) break;

                        BaseRewardData data = copyBaseList[r.Next(0, copyBaseList.Count)];
                        while (this._rewardRandomSet.Contains(data))
                        {
                            data = copyBaseList[r.Next(0, copyBaseList.Count)];
                        }

                        copyBaseList.Remove(data);
                        this._rewardRandomSet.Add(data);
                    }
                }
            }

            return this._rewardRandomSet;
        }

        return new List<BaseRewardData>();
    }
    
    private void GenerateStringDataList()
    {
        this.rewardStringList.Clear();
        this.possibleRewardStringList.Clear();
        foreach (BaseRewardData reward in this._rewardList)
        {
            rewardStringList.Add(JObject.Parse(JsonConvert.SerializeObject(reward)));
        }

        foreach (BaseRewardData reward in this._possibleRewardList)
        {
            possibleRewardStringList.Add(JObject.Parse(JsonConvert.SerializeObject(reward)));
        }
    }

    public void GenerateBaseList()
    {
        this.ClearRewardList();
        foreach (JObject s in this.rewardStringList)
        {
            string nameType = (string)s["typeObj"];
            Type t = Type.GetType(nameType);
            var result = s.ToObject(Type.GetType(nameType));
            if (result is BaseRewardData)
            {
                BaseRewardData dd = result as BaseRewardData;
                this._rewardList.Add(dd);
            }
        }
    }

    private void ClearRewardList()
    {
        this._rewardList.Clear();
        this._possibleRewardList.Clear();
    }
}