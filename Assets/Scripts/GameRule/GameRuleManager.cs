using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;


public class GameRuleManager : MonoSingleton<GameRuleManager>
{
    private bool isBattle;
    private Actor _player;
    //战斗时玩家和敌人位置的父节点
    private List<Transform> battleWorldNodes;

    public void StartBattle(Actor player, List<int> enemyIds,List<int> skillIds)
    {
        //弹出加载界面 加载战斗UI窗口
        UIManager.Instance.LoadingBattleWorld();
        EventCenter.Instance.EventTrigger("加载进度条更新", 0);
        int nodePosNum = 1;
        //注册玩家技能
        SkillSystem.Instance.RegistSkills(skillIds);
        //设置玩家位置
        _player = player;
        _player.transform.SetParent(battleWorldNodes[0],false);
        _player.transform.localPosition = Vector3.zero;
        _player.transform.localRotation = Quaternion.identity;
        _player.transform.localScale = Vector3.one;
        EventCenter.Instance.EventTrigger("加载进度条更新", 50);
        //注册所有敌人的技能 生成所有敌人
        foreach (var enemyId in enemyIds)
        {
            ResManager.Instance.LoadGameObjectAsync("prefab/enemy_"+enemyId.ToString(),battleWorldNodes[nodePosNum].gameObject,
                (enemyGo) =>
            {
                Actor enemyActor = enemyGo.GetComponent<Actor>();
                SkillSystem.Instance.RegistSkills(enemyActor.skillIds);
                //设置敌人的位置 TODO
                nodePosNum++;
            });
        }
        EventCenter.Instance.EventTrigger("加载进度条更新", 100);
        //切换战斗镜头
        CameraManager.Instance.ChangeToBattleCamera();
        //关闭加载窗口
        UIManager.Instance.LoadingBattleWorldDone();
        isBattle = true;
        EventCenter.Instance.EventTrigger("LoadingDone");
    }

    private void FixedUpdate()
    {
        if (!isBattle)
        {
            return;
        }
        BattleSystem.Instance.LogicFrameUpdate();
        SkillSystem.Instance.LogicFrameUpdate();
        ActionSystem.Instance.LogicFrameUpdate();
    }

    private void BattleEnd()
    {
        isBattle = false;
        BattleSystem.Instance.Destroy();
        SkillSystem.Instance.Destroy();
        ActionSystem.Instance.Destroy();
        _player.gameObject.GetComponent<ThirdPersonController>()._playerInput.enabled = true;
        UIManager.Instance.DestroyWindow("BattleWindow");
        CameraManager.Instance.ChangeToNormalCamera();
    }
}
