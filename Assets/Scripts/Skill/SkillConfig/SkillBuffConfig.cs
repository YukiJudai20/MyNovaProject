using Sirenix.OdinInspector;
using UnityEngine;
public enum BuffTarget
{
    [LabelText("������")]None,
    [LabelText("���")]Player,
    [LabelText("����")]Enemy,
}
[System.Serializable]
public class SkillBuffConfig
{
    [LabelText("Buff����")]
    public BuffType buffType;

    [LabelText("BuffĿ��")] 
    public BuffTarget buffTarget;
    
    [LabelText("Buff����")]
    public float buffParam;
}

public enum BuffType
{
    [LabelText("������")] None,
    [LabelText("���Ըı�")] ValueChange,
    [LabelText("״̬�ı�")] StatusChange,
    [LabelText("����˺�")] Damage
}
