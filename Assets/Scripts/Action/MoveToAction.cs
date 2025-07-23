using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移动类型

public class MoveToAction : ActionBehaviour
{
    private Actor _actor;
    private Vector3 startPos;
    private float moveTime;
    private Vector3 moveDistance;//移动向量
    private float accRunTime;
    private float timeScale;//移动时间缩放

    public MoveToAction(Actor actionActor, Vector3 startPos, Vector3 targetPos, float time, Action moveFinishCallBack,
        Action updateCallBack)
    {
        _actor = actionActor;
        this.startPos = startPos;
        moveTime = time;
        this.actionFinishCallBack = moveFinishCallBack;
        this.updateActionCallBack = updateCallBack;
        moveDistance = targetPos - startPos;
    }

    public override void ActionFinish()
    {
        if (actionFinish)
        {
            actionFinishCallBack?.Invoke();
        }
    }

    public override void LogicFrameUpdate()
    {
        accRunTime += 0.33f;
        timeScale = accRunTime / moveTime;
        if (timeScale >= 1)
        {
            timeScale = 1;
            actionFinish = true;
        }
        updateActionCallBack?.Invoke();
        Vector3 addDistance = Vector3.zero;
        addDistance = moveDistance * timeScale;
        _actor.gameObject.transform.position = startPos + addDistance;
    }
}
