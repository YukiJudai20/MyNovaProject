

using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Skill
{
    public SkillDataConfig skillDataConfig;
    private int frameCount;//技能当前进行的帧数
    private List<Actor> skillTargetList; //技能目标列表
    private Actor _actor; //技能释放者
    public bool isRealeasing; //是否正在释放
    private int skillDamageCount; //伤害段数
    private int damageInterval; //伤害触发间隔
    private bool startCauseDamage; //是否已经造成伤害

    public Skill(SkillDataConfig skillDataConfig)
    {
        this.skillDataConfig = skillDataConfig;
    }
    
    public void ReleaseSkill(GameObject skillOwner, List<Actor> skillTargetList)
    {
        frameCount = 0;
        skillDamageCount = 0;
        damageInterval = 0;
        startCauseDamage = false;
        isRealeasing = true;
        _actor = skillOwner.GetComponent<Actor>();
        this.skillTargetList = skillTargetList;
        Animator animator = skillOwner.GetComponent<Animator>();
        animator.Play(skillDataConfig.basicCfg.skillAnimation.name);
    }

    public void LogicFrameUpdate()
    {
        frameCount++;
        AudioLogicFrameUpdate();
        DamageLogicFrameUpdate();
        EffectLogicFrameUpdate();
        MoveLogicFrameUpdate();
        if (frameCount == skillDataConfig.basicCfg.animFrameCount)
        {
            isRealeasing = false;
        }
    }
    
    //逻辑音效更新
    private void AudioLogicFrameUpdate()
    {
        //播放音效
        foreach (var skillAudioConfig in skillDataConfig.audioCfgList)
        {
            if (frameCount == skillAudioConfig.triggerFrame)
            {
                AudioManager.Instance.PlayAudioClip(skillAudioConfig.soundEffect);
            }
        }
    }
    //逻辑伤害更新
    private void DamageLogicFrameUpdate()
    {
        //如果是多段伤害，则第一段伤害后，每隔设定好的间隔就触发一次伤害
        if (startCauseDamage && skillDamageCount < skillDataConfig.basicCfg.skillDamageCount)
        {
            
            damageInterval++;
            //达到间隔，造成伤害
            if (damageInterval == skillDataConfig.basicCfg.skillDamageTriggerInterval)
            {
                skillDamageCount++;
                foreach (var targetActor in skillTargetList)
                {
                    targetActor.TakeDamage(skillDataConfig.basicCfg.skillDamageParam,_actor.atk);
                }
                //间隔重置
                damageInterval = 0;
            }
        }
        
        //从伤害触发帧开始对技能目标造成伤害
        if (frameCount == skillDataConfig.basicCfg.skillDamageTriggerFrame)
        {
            skillDamageCount++;
            startCauseDamage = true;
            foreach (var targetActor in skillTargetList)
            {
                targetActor.TakeDamage(skillDataConfig.basicCfg.skillDamageParam,_actor.atk);
            }   
        }
    }
    //逻辑特效更新
    private void EffectLogicFrameUpdate()
    {
        //生成特效 
        foreach (var skillEffectConfig in skillDataConfig.effectCfgList)
        {
            //如果当前是某个特效的触发帧 则生成特效
            if (frameCount == skillEffectConfig.triggerFrame)
            {
                //从自身飞向目标的特效
                if (skillEffectConfig.effectGenerateType == EffectGenerateType.SelfToTarget)
                {
                    
                }
                //在技能目标身上生成特效
                else if (skillEffectConfig.effectGenerateType == EffectGenerateType.Target)
                {
                    foreach (var skillTarget in skillTargetList)
                    {
                        ResManager.Instance.LoadGameObjectAsync(PathUtils.RemoveResourcesPrefix(skillEffectConfig.effectPath),null,
                            (effectGo) =>
                            {
                                effectGo.transform.position = new Vector3(
                                    skillTarget.transform.position.x + skillEffectConfig.effectOffset.x,
                                    skillTarget.transform.position.y + skillEffectConfig.effectOffset.y,
                                    skillTarget.transform.position.z + skillEffectConfig.effectOffset.z);
                                float duration = effectGo.GetComponent<ParticleSystem>().main.duration;
                                GameObject.Destroy(effectGo, duration);
                            });
                    }
                }
            }
        }
    }
    //逻辑位移更新
    private void MoveLogicFrameUpdate()
    {
        if (frameCount == skillDataConfig.basicCfg.actionTriggerFrame && skillDataConfig.basicCfg.skillAction)
        {
            Vector3 originalPos = _actor.transform.position;
            Vector3 targetPos = Vector3.zero;
            //若是单体技能 则移动到目标位置
            if (skillDataConfig.basicCfg.skillTarget == SkillTarget.Single)
            {
                targetPos = skillTargetList[0].gameObject.transform.position;
            }
            //若是群体技能，则移动到所有目标中间
            else if (skillDataConfig.basicCfg.skillTarget == SkillTarget.All)
            {
                foreach (var actor in skillTargetList)
                {
                    targetPos += actor.transform.position;
                }
                targetPos = new Vector3(targetPos.x / skillTargetList.Count, targetPos.y / skillTargetList.Count,
                    targetPos.z / skillTargetList.Count);
            }
            //开始移动
            MoveToAction moveToAction = new MoveToAction(_actor,_actor.transform.position,targetPos,
                skillDataConfig.basicCfg.actionTotalFrame*0.33f, () =>
                {
                    //移动完成后回到原位置
                    MoveToAction moveBack = new MoveToAction(_actor, targetPos,
                        originalPos,
                        skillDataConfig.basicCfg.actionTotalFrame * 0.33f, null, null);
                    ActionSystem.Instance.RunAction(moveBack);  
                },null);
            ActionSystem.Instance.RunAction(moveToAction);   
        }
    }
}
