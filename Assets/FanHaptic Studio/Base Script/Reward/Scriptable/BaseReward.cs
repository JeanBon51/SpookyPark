using Sirenix.OdinInspector;
using UnityEngine;

public enum ValueOption
{
    None,
    FixAmount,
    RandomAmount,
}

public abstract class BaseReward : ScriptableObject
{
    [SerializeField] protected ValueOption _valueOption;

    public ValueOption valueOption => this._valueOption;

    [Button]
    public virtual bool Collect()
    {
        switch (this._valueOption)
        {
            case ValueOption.None:
                Debug.LogError($"No value option set in : {this.name}");
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

    public abstract bool CollectFixAmount();

    public abstract bool CollectRandomAmount();
}