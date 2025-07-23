#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.IO;
using UnityEditor;


public static class TimelineGenerator
{
    public static void GenerateTimelineFromSkill(SkillDataConfig skillConfig, string outputPath = "Assets/GameData/Timeline")
    {
        // ����Timeline
        var timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
        timelineAsset.name = "Skill_" + skillConfig.basicCfg.skillId + "_Timeline";

        GameObject directorGO = GameObject.Find("Director");
        directorGO.GetComponent<PlayableDirector>().playableAsset = timelineAsset;

        // ����Animation Track
        var animationTrack = timelineAsset.CreateTrack<AnimationTrack>(null, "Skill Animation Track");
        // �����ܶ�����ӵ����
        if (skillConfig.basicCfg.skillAnimation != null)
        {
            // ����TimelineClip�����ö���
            var clip = animationTrack.CreateClip(skillConfig.basicCfg.skillAnimation);
            clip.displayName = skillConfig.basicCfg.skillId + " Animation";

            // ����clip�ĳ���ʱ���붯������һ��
            clip.duration = skillConfig.basicCfg.skillAnimation.length;
        }

        //����Audio Track
        var audioTrack = timelineAsset.CreateTrack<AudioTrack>(null, "Skill Audio Track");
        // �����Ƶclip
        foreach (var audioConfig in skillConfig.audioCfgList)
        {
            if (audioConfig.soundEffect != null)
            {
                // ��֡��ת��Ϊʱ��
                double startTime = audioConfig.triggerFrame;
                // ������Ƶ����
                var audioClipAsset = audioTrack.CreateClip(audioConfig.soundEffect);
                audioClipAsset.displayName = audioConfig.soundEffect.name;
                // ���ü���λ�úͳ���ʱ��
                audioClipAsset.start = startTime;
                audioClipAsset.duration = audioConfig.soundEffect.length;
                // �����Ч��Ҫ�ضϣ�����ֻ����һ���֣�
                // ��������������clipIn��timeScale����
            }
        }



        // ����Timeline
        string assetPath = Path.Combine(outputPath, timelineAsset.name + ".playable");
        JsonTools.SaveTimeLineToJson(timelineAsset);
        AssetDatabase.CreateAsset(timelineAsset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// �� Animator �� Timeline �� Animation Track
    /// </summary>
    /// <param name="director">Timeline �� PlayableDirector</param>
    /// <param name="animator">Ҫ�󶨵� Animator</param>
    public static void BindAnimatorToTrack(PlayableDirector director, Animator animator)
    {
        if (director == null || animator == null)
        {
            Debug.LogError("Director or Animator is null!");
            return;
        }
        // ��ȡ TimelineAsset
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("Director's PlayableAsset is not a Timeline!");
            return;
        }
        // �������й�����ҵ� Animation Track
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is AnimationTrack animationTrack)
            {
                // �� Animator �����
                director.SetGenericBinding(animationTrack, animator);
            }
        }
    }
    
    /// <summary>
    /// �� AudioSource �� Timeline �� Audio Track
    /// </summary>
    /// <param name="director">Timeline �� PlayableDirector</param>
    /// <param name="animator">Ҫ�󶨵� AudioSource</param>
    public static void BindAudioSourceToTrack(PlayableDirector director, AudioSource audioSource)
    {
        if (director == null || audioSource == null)
        {
            Debug.LogError("Director or AudioSource is null!");
            return;
        }
        // ��ȡ TimelineAsset
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("Director's PlayableAsset is not a Timeline!");
            return;
        }
        // �������й�����ҵ� Animation Track
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is AudioTrack audioTrack)
            {
                // �� Animator �����
                director.SetGenericBinding(audioTrack, audioSource);
            }
        }
    }
    
}

// �����࣬���ڴ�����������
public static class AnimationClipUtil
{
    public static AnimationClip CreateAnimationClip(SkillDataConfig skillData)
    {
        var clip = new AnimationClip();
        clip.name = skillData.basicCfg.skillAnimation + "_Animation";

        // ������Ӷ������ߣ����ݼ����������ö���
        // ���磺
        // AnimationCurve curve = new AnimationCurve();
        // curve.AddKey(0.0f, 0.0f);
        // curve.AddKey(skillData.animationDuration, 1.0f);
        // clip.SetCurve("", typeof(Transform), "localPosition.x", curve);

        clip.legacy = false;
        clip.wrapMode = WrapMode.Once;
        clip.frameRate = 60;

        return clip;
    }
}
#endif

