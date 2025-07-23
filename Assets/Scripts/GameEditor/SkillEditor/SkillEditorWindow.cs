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

    [TabGroup("�����趨")]
    [LabelText("���ܻ����趨")]
    public SkillBasicConfig basicConfig = new SkillBasicConfig();

    // [TabGroup("�߼�֡�趨")]
    // [LabelText("�����߼�֡�趨")]
    // public SkillLogicConfig logicConfig = new SkillLogicConfig();

    [TabGroup("��Ч")]
    [LabelText("������Ч�б�")]
    public List<SkillEffectConfig> effectList = new List<SkillEffectConfig>();

    [TabGroup("��Ч")]
    [LabelText("������Ч�б�")]
    public List<SkillAudioConfig> audioList = new List<SkillAudioConfig>();

    // [TabGroup("����")]
    // [LabelText("����Ч���б�")]
    // public List<SkillPhysicsConfig> physList = new List<SkillPhysicsConfig>();
    //
    // [TabGroup("tab2","λ��")]
    // [LabelText("λ��Ч���б�")]
    // public List<SkillActionConfig> actionList = new List<SkillActionConfig>();

    [TabGroup("Buff")]
    [LabelText("Buff�б�")]
    public List<SkillBuffConfig> buffList = new List<SkillBuffConfig>();

    [ButtonGroup]
    [GUIColor(0, 1, 0)]
    [Button("���漼��")]
    public void SaveSkill()
    {
        // ������ӱ��漼�ܵ��߼�
        // SkillDataConfig.SaveSkillData(basicConfig, logicConfig, effectList,
        //     audioList, physList,actionList,buffList);
        SkillDataConfig.SaveSkillData(basicConfig, effectList,
            audioList, buffList);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Skill Save",
            $"����{basicConfig.skillId}���ݱ���ɹ�!", "OK");
        //Close();
    }

    [ButtonGroup]
    [GUIColor(1, 1, 0)]
    [Button("���Լ���")]
    public void TestSkill()
    {
        // ���Լ���
        // �������Ԥ����
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
        // �ҵ�director ��timeline��ֵ��director
        GameObject directorGO = GameObject.Find("Director");
        if(directorGO == null)
        {
            EditorUtility.DisplayDialog("Test Error",
$"δ�ҵ�Director!", "ERROR");
        }
        string timelinePath = "Assets/GameData/Timeline/Skill_" + skillDataConfig.basicCfg.skillId + "_Timeline.playable";
        var timelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(timelinePath);
        //var timelineAsset = Resources.Load<TimelineAsset>("Skill_" + skillDataConfig.basicCfg.skillId + "_Timeline");
        PlayableDirector director = directorGO.GetComponent<PlayableDirector>();
        director.playableAsset = timelineAsset;

        // 6. ��Animator
        var animator = playerInstance.GetComponent<Animator>();

        if (animator != null)
        {
            TimelineGenerator.BindAnimatorToTrack(directorGO.GetComponent<PlayableDirector>(), animator);
        }
        
        //��AudioSource
        AudioSource audioSource = directorGO.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            TimelineGenerator.BindAudioSourceToTrack(directorGO.GetComponent<PlayableDirector>(), audioSource);
        }
        director.Play();
    }

    // [ButtonGroup]
    // [GUIColor(0, 1, 1)]
    // [Button("����TimeLine")]
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
    // $"����{basicConfig.skillId}��������TimeLine�ɹ�!", "OK");
    // }
    
    [ButtonGroup]
    [GUIColor(0, 1, 1)]
    [Button("�ںϼ���")]
    public void FusionSkill()
    {
        //     SkillDataConfig skillDataConfig= SkillDataConfig.SaveSkillData(basicConfig, logicConfig, effectList,
        // audioList, physList,actionList,buffList);
        if (basicConfig.skill1 != null && basicConfig.skill2 != null)
        {
            
        }
        else
        {
            EditorUtility.DisplayDialog("FusionSkill", "����ѡ���������ܲ��ܽ����ںϣ�", "Error");
            return;
        }
        SkillDataConfig skillDataConfig=SkillDataConfig.SaveSkillData(basicConfig, effectList,
            audioList, buffList);
    }

    /// <summary>
    /// ���ؼ�������
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