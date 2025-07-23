using Sirenix.OdinInspector;
using UnityEngine;


[System.Serializable]

public class SkillAudioConfig
{
    [LabelText("��Ч�ļ�"),OnValueChanged("GetAnimLength")]
    public AudioClip soundEffect;

    [LabelText("��Чʱ��"),ReadOnly] 
    public float audioLength;
    
    [LabelText("��Ч֡��"),ReadOnly] 
    public float audioFrameCount;

    [LabelText("��Ч����֡")]
    public int triggerFrame;
    
    private void GetAnimLength()
    {
        audioLength = soundEffect.length;
        audioFrameCount = audioLength / 0.033f;
    }
}
