#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class SkillEditorWindow : OdinEditorWindow
{

    [TabGroup("基础设定")]
    [LabelText("技能基础设定")]
    public SkillBasicConfig basicConfig = new SkillBasicConfig();

    // [TabGroup("逻辑帧设定")]
    // [LabelText("技能逻辑帧设定")]
    // public SkillLogicConfig logicConfig = new SkillLogicConfig();

    [TabGroup("特效")]
    [LabelText("技能特效列表")]
    public List<SkillEffectConfig> effectList = new List<SkillEffectConfig>();

    [TabGroup("音效")]
    [LabelText("技能音效列表")]
    public List<SkillAudioConfig> audioList = new List<SkillAudioConfig>();

    // [TabGroup("物理")]
    // [LabelText("物理效果列表")]
    // public List<SkillPhysicsConfig> physList = new List<SkillPhysicsConfig>();
    //
    // [TabGroup("tab2","位移")]
    // [LabelText("位移效果列表")]
    // public List<SkillActionConfig> actionList = new List<SkillActionConfig>();

    [TabGroup("Buff")]
    [LabelText("Buff列表")]
    public List<SkillBuffConfig> buffList = new List<SkillBuffConfig>();

    [ButtonGroup]
    [GUIColor(0, 1, 0)]
    [Button("保存技能")]
    public void SaveSkill()
    {
        // 这里添加保存技能的逻辑
        // SkillDataConfig.SaveSkillData(basicConfig, logicConfig, effectList,
        //     audioList, physList,actionList,buffList);
        SkillDataConfig.SaveSkillData(basicConfig, effectList,
            audioList, buffList);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Skill Save",
            $"技能{basicConfig.skillId}数据保存成功!", "OK");
        //Close();
    }

    [ButtonGroup]
    [GUIColor(1, 1, 0)]
    [Button("测试技能")]
    public void TestSkill()
    {
        // 测试技能
        // 加载玩家预制体
        GameObject playerPrefab = null;
        // SkillDataConfig skillDataConfig = SkillDataConfig.SaveSkillData(basicConfig, logicConfig, effectList,
        //     audioList, physList,actionList,buffList);
        SkillDataConfig skillDataConfig=SkillDataConfig.SaveSkillData(basicConfig, effectList,
            audioList, buffList);
        if (skillDataConfig.basicCfg.gameObject != null)
        {
            playerPrefab = skillDataConfig.basicCfg.gameObject;
        }

        var playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerInstance.name = $"Test-Skill{skillDataConfig.basicCfg.skillId}-Player";
        // 找到director 把timeline赋值给director
        GameObject directorGO = GameObject.Find("Director");
        if(directorGO == null)
        {
            EditorUtility.DisplayDialog("Test Error",
$"未找到Director!", "ERROR");
        }
        string timelinePath = "Assets/GameData/Timeline/Skill_" + skillDataConfig.basicCfg.skillId + "_Timeline.playable";
        var timelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(timelinePath);
        //var timelineAsset = Resources.Load<TimelineAsset>("Skill_" + skillDataConfig.basicCfg.skillId + "_Timeline");
        PlayableDirector director = directorGO.GetComponent<PlayableDirector>();
        director.playableAsset = timelineAsset;

        // 6. 绑定Animator
        var animator = playerInstance.GetComponent<Animator>();

        if (animator != null)
        {
            TimelineGenerator.BindAnimatorToTrack(directorGO.GetComponent<PlayableDirector>(), animator);
        }
        
        //绑定AudioSource
        AudioSource audioSource = directorGO.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            TimelineGenerator.BindAudioSourceToTrack(directorGO.GetComponent<PlayableDirector>(), audioSource);
        }
        director.Play();
    }

    // [ButtonGroup]
    // [GUIColor(0, 1, 1)]
    // [Button("生成TimeLine")]
    // public void GenerateTimeLine()
    // {
    // //     SkillDataConfig skillDataConfig= SkillDataConfig.SaveSkillData(basicConfig, logicConfig, effectList,
    // // audioList, physList,actionList,buffList);
    //     SkillDataConfig skillDataConfig=SkillDataConfig.SaveSkillData(basicConfig, effectList,
    //         audioList, buffList);
    //     TimelineGenerator.GenerateTimelineFromSkill(skillDataConfig);
    //     AssetDatabase.Refresh();
    //
    //     EditorUtility.DisplayDialog("GenerateTimeLine",
    // $"技能{basicConfig.skillId}数据生成TimeLine成功!", "OK");
    // }
    
    [ButtonGroup]
    [GUIColor(0, 1, 1)]
    [Button("融合技能")]
    public void FusionSkill()
    {
        //     SkillDataConfig skillDataConfig= SkillDataConfig.SaveSkillData(basicConfig, logicConfig, effectList,
        // audioList, physList,actionList,buffList);
        if (basicConfig.skill1 != null && basicConfig.skill2 != null)
        {
            
        }
        else
        {
            EditorUtility.DisplayDialog("FusionSkill", "必须选择两个技能才能进行融合！", "Error");
            return;
        }
        SkillDataConfig skillDataConfig=SkillDataConfig.SaveSkillData(basicConfig, effectList,
            audioList, buffList);
    }

    /// <summary>
    /// 加载技能数据
    /// </summary>
    /// <param name="skillData"></param>
    public void LoadSkillData(SkillDataConfig skillData)
    {
        this.basicConfig = skillData.basicCfg;
        // this.logicConfig = skillData.skillLogicCfg;
        this.effectList = skillData.effectCfgList;
        this.audioList = skillData.audioCfgList;
        this.buffList = skillData.buffCfgList;
        // this.physList= skillData.physicsCfgList;
    }
}
#endif