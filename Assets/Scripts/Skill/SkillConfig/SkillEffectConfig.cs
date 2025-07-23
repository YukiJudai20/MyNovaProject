using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public enum EffectPos
{
    [LabelText("无配置")]None,
    [LabelText("玩家")]Player,
    [LabelText("敌人")]Enemy,
}

public enum EffectGenerateType
{
    [LabelText("无配置")]None,
    [LabelText("从自身飞向目标")]SelfToTarget,
    [LabelText("在目标身上生成")]Target,
}
[System.Serializable]
public class SkillEffectConfig
{
    [LabelText("特效预制体"),OnValueChanged("SetEffectPath")]
    [PreviewField(60,ObjectFieldAlignment.Right)]
    public GameObject effectPrefab;

    [LabelText("特效资源路径")] 
    public string effectPath;

    [LabelText("特效生成位置")] 
    public EffectPos effectPos;
    
    [LabelText("特效生成方式"),OnValueChanged("ShowEffectMoveTime")] 
    public EffectGenerateType effectGenerateType;

    private bool showEffectMoveTime;

    [LabelText("特效飞行帧数"), ShowIf("showEffectMoveTime")]
    public int effectMoveFrame;

    [LabelText("特效偏移")] 
    public Vector3 effectOffset;
    
    [LabelText("触发帧")]
    public float triggerFrame;

    private void SetEffectPath()
    {
        if (effectPrefab == null)
        {
            return;
        }
#if UNITY_EDITOR
        string fullPath = AssetDatabase.GetAssetPath(effectPrefab);
        effectPath = PathToProjectRelative(fullPath);
#endif
    }

    private string PathToProjectRelative(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            return "无效路径";
        }

        if (fullPath.StartsWith("Assets/"))
        {
            return fullPath;
        }

        int assetsIndex = fullPath.IndexOf("Assets/");
        return assetsIndex >= 0 ? fullPath.Substring(assetsIndex) : fullPath;
    }

    private void ShowEffectMoveTime()
    {
        showEffectMoveTime = effectGenerateType == EffectGenerateType.SelfToTarget;
    }
}
