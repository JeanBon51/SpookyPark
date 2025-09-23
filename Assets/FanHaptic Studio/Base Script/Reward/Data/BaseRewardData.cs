using System;
using UnityEngine;

[System.Serializable]
public class BaseRewardData 
{
    [SerializeField] public ValueOption valueOption;
    [HideInInspector] public string typeObj = String.Empty;
    
    public virtual bool Collect()
    {
        switch (this.valueOption)
        {
            case ValueOption.None:
                Debug.LogError($"No value option set in : {this.ToString()}");
                return false;
                break;
            case ValueOption.FixAmount:
                return this.CollectFixAmount();
                break;
            case ValueOption.RandomAmount:
                return this.CollectRandomAmount();
                break;
        }
        return false;
    }

    public virtual bool CollectFixAmount()
    {
        return false;
    }

    public virtual bool CollectRandomAmount()
    {
        return false;
    }
}
