using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Timeline;

public class JsonTools
{
    public static void SaveSkillToJson(SkillDataConfig skillDataConfig)
    {
        string jsonData = JsonUtility.ToJson(skillDataConfig, true);
        string savePath = Path.Combine(Application.persistentDataPath, "SkillData");
        Directory.CreateDirectory(savePath);
        string fileName = $"{skillDataConfig.basicCfg.skillId}.json";
        string fullPath = Path.Combine(savePath, fileName);
        
        File.WriteAllText(fullPath, jsonData);
        
        Debug.Log($"Skill saved to: {fullPath}");
    }

    public static SkillDataConfig LoadSkillFromJson(string skillId)
    {
        string path = Path.Combine(
            Application.persistentDataPath, 
            "SkillData", 
            $"{skillId}.json"
        );

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SkillDataConfig loadedSkill = ScriptableObject.CreateInstance<SkillDataConfig>();
            JsonUtility.FromJsonOverwrite(json,loadedSkill);
            return loadedSkill;
        }
        Debug.LogError($"File not found: {path}");
        return null;
    }
    public static void SavePlayerStatusToJson(PlayerStatus skillDataConfig)
    {
        string jsonData = JsonUtility.ToJson(skillDataConfig, true);
        string savePath = Path.Combine(Application.persistentDataPath, "PlayerStatus");
        Directory.CreateDirectory(savePath);
        string fileName = "PlayerStatus.json";
        string fullPath = Path.Combine(savePath, fileName);
        
        File.WriteAllText(fullPath, jsonData);
        
        Debug.Log($"PlayerStatus saved to: {fullPath}");
    }
    public static PlayerStatus LoadSkillFromJson()
    {
        string path = Path.Combine(
            Application.persistentDataPath, 
            "PlayerStatus", 
            "PlayerStatus.json"
        );

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerStatus playerStatus = ScriptableObject.CreateInstance<PlayerStatus>();
            JsonUtility.FromJsonOverwrite(json,playerStatus);
            return playerStatus;
        }
        Debug.LogError($"File not found: {path}");
        return null;
    }
    
    public static void SaveTimeLineToJson(TimelineAsset timelineAsset)
    {
        string jsonData = JsonUtility.ToJson(timelineAsset, true);
        string savePath = Path.Combine(Application.persistentDataPath, "Timeline");
        Directory.CreateDirectory(savePath);
        string fileName = $"{timelineAsset.name}.json";
        string fullPath = Path.Combine(savePath, fileName);
        
        File.WriteAllText(fullPath, jsonData);
        
        Debug.Log($"Timeline saved to: {fullPath}");
    }
    
    public static TimelineAsset LoadTimelineFromJson(string timeLineName)
    {
        string path = Path.Combine(
            Application.persistentDataPath, 
            "SkillData", 
            $"{timeLineName}.json"
        );

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            TimelineAsset loadedTimeline = ScriptableObject.CreateInstance<TimelineAsset>();
            JsonUtility.FromJsonOverwrite(json,loadedTimeline);
            return loadedTimeline;
        }
        Debug.LogError($"File not found: {path}");
        return null;
    }
}
