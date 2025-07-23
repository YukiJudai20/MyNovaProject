using Sirenix.OdinInspector;
using UnityEngine;
public enum BuffTarget
{
    [LabelText("无配置")]None,
    [LabelText("玩家")]Player,
    [LabelText("敌人")]Enemy,
}
[System.Serializable]
public class SkillBuffConfig
{
    [LabelText("Buff类型")]
    public BuffType buffType;

    [LabelText("Buff目标")] 
    public BuffTarget buffTarget;
    
    [LabelText("Buff参数")]
    public float buffParam;
}

public enum BuffType
{
    [LabelText("无配置")] None,
    [LabelText("属性改变")] ValueChange,
    [LabelText("状态改变")] StatusChange,
    [LabelText("造成伤害")] Damage
}
