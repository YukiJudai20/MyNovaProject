using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : Singleton<BattleSystem>
{
    //回合数
    private int roundCount;
    private List<int> actorIds;
    

    public void LogicFrameUpdate()
    {
        
    }

    public void NextActorAction(int actorNumber)
    {
        if (actorNumber < actorIds.Count)
        {
            actorNumber++;
        }
    }

    public void Destroy()
    {
        actorIds.Clear();
    }
}
