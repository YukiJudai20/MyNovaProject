using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SkillTarget
{
    [LabelText("������")]None,
    [LabelText("����")]Single,
    [LabelText("ȫ��")]All,
    [LabelText("���")]Random,
}

public enum SkillDamageType
{
    [LabelText("������")]None,
    [LabelText("�����˺�")]Single,
    [LabelText("����˺�")]Multiple,
}
[System.Serializable]
public class SkillBasicConfig
{
    [LabelText("����ͼ��"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel("����ͼ��")]
    public Sprite skillIcon;
    [LabelText("�󶨽�ɫ")]
    public GameObject gameObject;

    [LabelText("����ID")]
    public int skillId;

    [LabelText("��������")]
    public string skillName;

    [LabelText("��������")]
    [TextArea(3, 10)]
    public string skillDescription;

    [LabelText("����"), OnValueChanged("GetAnimLength")]
    public AnimationClip skillAnimation;

    [LabelText("����ʱ��"),ReadOnly]
    public float animDuration;
    [LabelText("����֡��"),ReadOnly]
    public float animFrameCount;

    [LabelText("���ܽ���")] 
    public int skillLevel;
    
    [LabelText("����Ŀ��")]
    public SkillTarget skillTarget;
    
    [LabelText("�����˺�����֡")]
    public int skillDamageTriggerFrame;

    [LabelText("�����˺�����"),OnValueChanged("SetSkillDamageType")] 
    public SkillDamageType skillDamageType;

    private bool showSkillDamageCount;
    [LabelText("�����˺�����"), ShowIf("showSkillDamageCount")]
    public int skillDamageCount;
    
    [LabelText("����˺��������֡"), ShowIf("showSkillDamageCount")]
    public int skillDamageTriggerInterval;
    
    [LabelText("�����˺�����(���Ƕ���˺��������ÿ�εı���)")] 
    public int skillDamageParam;

    [LabelText("�����Ƿ���λ��")] 
    public bool skillAction;

    [LabelText("����λ�ƴ���֡"),ShowIf("skillAction")] 
    public int actionTriggerFrame;
    
    [LabelText("����λ�Ƴ���֡"),ShowIf("skillAction")] 
    public int actionTotalFrame;

    [Title("�ںϼ���")] 
    [LabelText("��Ҫ����")] 
    public SkillDataConfig skill1;

    [LabelText("��Ҫ����")] 
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
