using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<ActionBehaviour> actionList = new List<ActionBehaviour>();

    public void RunAction(ActionBehaviour action)
    {
        action.actionFinish = false;
        actionList.Add(action);
    }

    public void LogicFrameUpdate()
    {
        for (var i = actionList.Count - 1; i >= 0; i--)
        {
            ActionBehaviour action = actionList[i];
            if (action.actionFinish)
            {
                actionList.Remove(action);
            }
        }

        foreach (var action in actionList)
        {
            action.LogicFrameUpdate();
        }
    }

    public void Destroy()
    {
        actionList.Clear();
    }
}
