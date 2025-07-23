using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : Singleton<SkillSystem>
{
    public Dictionary<int, SkillDataConfig> skillDic = new Dictionary<int, SkillDataConfig>();
    private List<Skill> realeasingSkills = new List<Skill>();

    public void Initialize()
    {
        skillDic.Clear();
        realeasingSkills.Clear();
    }
    
    public void RegistSkills(List<int> skillIds)
    {
        foreach (var skillId in skillIds)
        {
            //如果已经注册了该技能，则跳过
            if (skillDic.TryGetValue(skillId, out var skill))
            {
                continue;
            }

            SkillDataConfig skillDataConfig = JsonTools.LoadSkillFromJson(skillId.ToString());
            skillDic[skillId] = skillDataConfig;
        }
    }

    public void LogicFrameUpdate()
    {
        foreach (var realeasingSkill in realeasingSkills)
        {
            realeasingSkill.LogicFrameUpdate();
        }

        for (var i = realeasingSkills.Count - 1; i >= 0; i--)
        {
            if (!realeasingSkills[i].isRealeasing)
            {
                Skill curSkill = realeasingSkills[i];
                realeasingSkills.Remove(curSkill);
            }
        }
    }
    
    public void ReleaseSkill(GameObject skillOwner,int skillId,List<Actor> skillTargetList)
    {
        Skill skill = new Skill(skillDic[skillId]);
        skill.ReleaseSkill(skillOwner,skillTargetList);
        realeasingSkills.Add(skill);
    }

    public void Destroy()
    {
        skillDic.Clear();
        realeasingSkills.Clear();
    }
    
}
