using UnityEngine;
using System.Collections;
using System;

public class XXTestAction
{
    public string BehaviorName = "";
    public XXEntry Target;
    public float ActionNeedTime;
    public float ActionLastTime;
    public Action OnActionEnd;
    public bool IsActive;

    public void UpdateLogic()
    {
        if (!IsActive)
            return;
        ActionLastTime += Time.deltaTime;
        if(ActionLastTime >= ActionNeedTime)
        {
            ActionEnd();
        }
    }

    public void ActionStart()
    {
        ActionLastTime = 0;
        IsActive = true;
    }

    public void ActionCancel()
    {
        IsActive = false;
    }

    void ActionEnd()
    {
        IsActive = false;
        if(OnActionEnd != null)
            OnActionEnd();
    }

}
