using Sirenix.OdinInspector;
using UnityEngine;


[System.Serializable]

public class SkillAudioConfig
{
    [LabelText("音效文件"),OnValueChanged("GetAnimLength")]
    public AudioClip soundEffect;

    [LabelText("音效时长"),ReadOnly] 
    public float audioLength;
    
    [LabelText("音效帧数"),ReadOnly] 
    public float audioFrameCount;

    [LabelText("音效生成帧")]
    public int triggerFrame;
    
    private void GetAnimLength()
    {
        audioLength = soundEffect.length;
        audioFrameCount = audioLength / 0.033f;
    }
}
