using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFusion
{
    public static void FusionTwoSkills(SkillDataConfig skill1,SkillDataConfig skill2)
    {
        SkillDataConfig newSkill = new SkillDataConfig();
        
        JsonTools.SaveSkillToJson(newSkill);
    }
}
