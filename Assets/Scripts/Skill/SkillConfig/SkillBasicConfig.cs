using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SkillTarget
{
    [LabelText("无配置")]None,
    [LabelText("单体")]Single,
    [LabelText("全体")]All,
    [LabelText("随机")]Random,
}

public enum SkillDamageType
{
    [LabelText("无配置")]None,
    [LabelText("单段伤害")]Single,
    [LabelText("多段伤害")]Multiple,
}
[System.Serializable]
public class SkillBasicConfig
{
    [LabelText("技能图标"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel("技能图标")]
    public Sprite skillIcon;
    [LabelText("绑定角色")]
    public GameObject gameObject;

    [LabelText("技能ID")]
    public int skillId;

    [LabelText("技能名称")]
    public string skillName;

    [LabelText("技能描述")]
    [TextArea(3, 10)]
    public string skillDescription;

    [LabelText("动画"), OnValueChanged("GetAnimLength")]
    public AnimationClip skillAnimation;

    [LabelText("动画时长"),ReadOnly]
    public float animDuration;
    [LabelText("动画帧数"),ReadOnly]
    public float animFrameCount;

    [LabelText("技能阶数")] 
    public int skillLevel;
    
    [LabelText("技能目标")]
    public SkillTarget skillTarget;
    
    [LabelText("技能伤害触发帧")]
    public int skillDamageTriggerFrame;

    [LabelText("技能伤害类型"),OnValueChanged("SetSkillDamageType")] 
    public SkillDamageType skillDamageType;

    private bool showSkillDamageCount;
    [LabelText("技能伤害段数"), ShowIf("showSkillDamageCount")]
    public int skillDamageCount;
    
    [LabelText("多段伤害触发间隔帧"), ShowIf("showSkillDamageCount")]
    public int skillDamageTriggerInterval;
    
    [LabelText("技能伤害倍率(若是多段伤害，则代表每段的倍率)")] 
    public int skillDamageParam;

    [LabelText("技能是否有位移")] 
    public bool skillAction;

    [LabelText("技能位移触发帧"),ShowIf("skillAction")] 
    public int actionTriggerFrame;
    
    [LabelText("技能位移持续帧"),ShowIf("skillAction")] 
    public int actionTotalFrame;

    [Title("融合技能")] 
    [LabelText("主要技能")] 
    public SkillDataConfig skill1;

    [LabelText("次要技能")] 
    public SkillDataConfig skill2;
    
    private void GetAnimLength()
    {
        animDuration = skillAnimation.length;
        animFrameCount = animDuration / 0.033f;
    }

    private void SetSkillDamageType(SkillDamageType skillDamageType)
    {
        showSkillDamageCount = skillDamageType == SkillDamageType.Multiple;
        if (skillDamageType == SkillDamageType.Single)
        {
            skillDamageCount = 1;
        }
    }
}
