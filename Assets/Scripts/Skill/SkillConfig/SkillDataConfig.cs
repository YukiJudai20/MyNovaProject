using Sirenix.OdinInspector;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[CreateAssetMenu(fileName = "SkillConfig", menuName = "SkillConfig", order = 0)]
public class SkillDataConfig : ScriptableObject
{
    public SkillBasicConfig basicCfg;
    // public SkillLogicConfig skillLogicCfg;
    public List<SkillEffectConfig> effectCfgList;
    public List<SkillAudioConfig> audioCfgList;
    // public List<SkillPhysicsConfig> physicsCfgList;
    // public List<SkillActionConfig> actionCfgList;
    public List<SkillBuffConfig> buffCfgList;


#if UNITY_EDITOR
    public static SkillDataConfig SaveSkillData(SkillBasicConfig basicCfg, List<SkillEffectConfig> effectCfgList
        , List<SkillAudioConfig> audioCfgList, List<SkillBuffConfig> buffCfgList)
    {
        //通过代码创建SkillDataConfig的实例，并对字段进行赋值储存
        SkillDataConfig skillDataCfg = ScriptableObject.CreateInstance<SkillDataConfig>();
        skillDataCfg.basicCfg = basicCfg;
        skillDataCfg.effectCfgList = effectCfgList;
        skillDataCfg.audioCfgList = audioCfgList;
        skillDataCfg.buffCfgList = buffCfgList;
        //把当前实例储存为.asset资源文件，当作技能配置
        JsonTools.SaveSkillToJson(skillDataCfg);
        string assetPath = "Assets/GameData/SkillData/" + basicCfg.skillId + ".asset";
        //如果资源对象已存在，先进行删除，在进行创建
        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.CreateAsset(skillDataCfg, assetPath);
        return skillDataCfg;
    }

    [Button("配置技能", ButtonSizes.Large), GUIColor("green")]
    public void ShowSkillWindowButtonClick()
    {
        GameEditorWindow window = GameEditorWindow.ShowAssetBundleWindow();
        SkillEditorWindow skillWindow = window.CreateSkillEditor();
        skillWindow.LoadSkillData(this);
    }
    public void SaveAsset()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
    
    //在游戏中保存技能数据 
    public static SkillDataConfig GameSaveSkillData(SkillBasicConfig basicCfg, List<SkillEffectConfig> effectCfgList
        , List<SkillAudioConfig> audioCfgList, List<SkillBuffConfig> buffCfgList)
    {
        //通过代码创建SkillDataConfig的实例，并对字段进行赋值储存
        SkillDataConfig skillDataCfg = ScriptableObject.CreateInstance<SkillDataConfig>();
        skillDataCfg.basicCfg = basicCfg;
        skillDataCfg.effectCfgList = effectCfgList;
        skillDataCfg.audioCfgList = audioCfgList;
        skillDataCfg.buffCfgList = buffCfgList;
        //把当前技能配置保存为json
        JsonTools.SaveSkillToJson(skillDataCfg);
        return skillDataCfg;
    }
}
