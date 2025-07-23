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
        // 创建Timeline
        var timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
        timelineAsset.name = "Skill_" + skillConfig.basicCfg.skillId + "_Timeline";

        GameObject directorGO = GameObject.Find("Director");
        directorGO.GetComponent<PlayableDirector>().playableAsset = timelineAsset;

        // 创建Animation Track
        var animationTrack = timelineAsset.CreateTrack<AnimationTrack>(null, "Skill Animation Track");
        // 将技能动画添加到轨道
        if (skillConfig.basicCfg.skillAnimation != null)
        {
            // 创建TimelineClip并设置动画
            var clip = animationTrack.CreateClip(skillConfig.basicCfg.skillAnimation);
            clip.displayName = skillConfig.basicCfg.skillId + " Animation";

            // 设置clip的持续时间与动画长度一致
            clip.duration = skillConfig.basicCfg.skillAnimation.length;
        }

        //创建Audio Track
        var audioTrack = timelineAsset.CreateTrack<AudioTrack>(null, "Skill Audio Track");
        // 添加音频clip
        foreach (var audioConfig in skillConfig.audioCfgList)
        {
            if (audioConfig.soundEffect != null)
            {
                // 将帧数转换为时间
                double startTime = audioConfig.triggerFrame;
                // 创建音频剪辑
                var audioClipAsset = audioTrack.CreateClip(audioConfig.soundEffect);
                audioClipAsset.displayName = audioConfig.soundEffect.name;
                // 设置剪辑位置和持续时间
                audioClipAsset.start = startTime;
                audioClipAsset.duration = audioConfig.soundEffect.length;
                // 如果音效需要截断（例如只播放一部分）
                // 可以在这里设置clipIn和timeScale属性
            }
        }



        // 保存Timeline
        string assetPath = Path.Combine(outputPath, timelineAsset.name + ".playable");
        JsonTools.SaveTimeLineToJson(timelineAsset);
        AssetDatabase.CreateAsset(timelineAsset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 绑定 Animator 到 Timeline 的 Animation Track
    /// </summary>
    /// <param name="director">Timeline 的 PlayableDirector</param>
    /// <param name="animator">要绑定的 Animator</param>
    public static void BindAnimatorToTrack(PlayableDirector director, Animator animator)
    {
        if (director == null || animator == null)
        {
            Debug.LogError("Director or Animator is null!");
            return;
        }
        // 获取 TimelineAsset
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("Director's PlayableAsset is not a Timeline!");
            return;
        }
        // 遍历所有轨道，找到 Animation Track
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is AnimationTrack animationTrack)
            {
                // 绑定 Animator 到轨道
                director.SetGenericBinding(animationTrack, animator);
            }
        }
    }
    
    /// <summary>
    /// 绑定 AudioSource 到 Timeline 的 Audio Track
    /// </summary>
    /// <param name="director">Timeline 的 PlayableDirector</param>
    /// <param name="animator">要绑定的 AudioSource</param>
    public static void BindAudioSourceToTrack(PlayableDirector director, AudioSource audioSource)
    {
        if (director == null || audioSource == null)
        {
            Debug.LogError("Director or AudioSource is null!");
            return;
        }
        // 获取 TimelineAsset
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("Director's PlayableAsset is not a Timeline!");
            return;
        }
        // 遍历所有轨道，找到 Animation Track
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is AudioTrack audioTrack)
            {
                // 绑定 Animator 到轨道
                director.SetGenericBinding(audioTrack, audioSource);
            }
        }
    }
    
}

// 辅助类，用于创建动画剪辑
public static class AnimationClipUtil
{
    public static AnimationClip CreateAnimationClip(SkillDataConfig skillData)
    {
        var clip = new AnimationClip();
        clip.name = skillData.basicCfg.skillAnimation + "_Animation";

        // 这里添加动画曲线，根据技能数据设置动画
        // 例如：
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

