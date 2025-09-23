using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "FanHaptic/Reward/ChestReward", fileName = "ChestReward")]
public class ChestReward : BaseReward
{
    public TypeChest type;
    //--------------------------------------------- Fix ---------------------------------------------

    [SerializeReference,BoxGroup("FixAmount"), ShowIfGroup("FixAmount/_valueOption", Value = ValueOption.FixAmount)]
    public List<BaseRewardData> rewardList = new List<BaseRewardData>();

    //--------------------------------------------- Random ------------------------------------------

    [SerializeReference, BoxGroup("RandomAmount"), ShowIfGroup("RandomAmount/_valueOption", Value = ValueOption.RandomAmount)]
    public List<BaseRewardData> possibleRewardList = new List<BaseRewardData>();

    [SerializeField, BoxGroup("RandomAmount"), ShowIfGroup("RandomAmount/_valueOption", Value = ValueOption.RandomAmount)]
    public int nbRandomReward = 0;

    [SerializeField, BoxGroup("RandomAmount"), ShowIfGroup("RandomAmount/_valueOption", Value = ValueOption.RandomAmount)]
    public bool canHaveSameReward = false;

    //-----------------------------------------------------------------------------------------------
    
    public override bool CollectFixAmount()
    {
        bool allGood = true;
        foreach (BaseRewardData rewardData in this.rewardList)
        {
            if (rewardData.Collect() == false)
            {
                Debug.LogError($"Failed To Collect this reward : {rewardData}");
                allGood = false;
            }
        }

        return allGood;
    }

    public override bool CollectRandomAmount()
    {
        System.Random r = new System.Random();
        bool allGood = true;
        List<BaseRewardData> result = new List<BaseRewardData>();
        if (this.canHaveSameReward)
        {
            for (int i = 0; i < this.nbRandomReward; i++)
            {
                if (this.possibleRewardList.Count == 0) break;

                BaseRewardData data = this.possibleRewardList[r.Next(0, this.possibleRewardList.Count)];
                result.Add(data);
            }
        }
        else
        {
            List<BaseRewardData> copyBaseList = new List<BaseRewardData>(possibleRewardList);
            for (int i = 0; i < this.nbRandomReward; i++)
            {
                if (copyBaseList.Count == 0) break;

                BaseRewardData data = copyBaseList[r.Next(0, copyBaseList.Count)];
                while (result.Contains(data))
                {
                    data = copyBaseList[r.Next(0, copyBaseList.Count)];
                }

                copyBaseList.Remove(data);
                result.Add(data);
            }
        }

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

}
