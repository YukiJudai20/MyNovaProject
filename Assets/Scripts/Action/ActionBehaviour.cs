using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBehaviour
{
    public bool actionFinish = false;//是否移动完成
    protected Action actionFinishCallBack;//移动完成回调
    protected Action updateActionCallBack;//移动更新回调
    public abstract void LogicFrameUpdate();
    public abstract void ActionFinish();
}
