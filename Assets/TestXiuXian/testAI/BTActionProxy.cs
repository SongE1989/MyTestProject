using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTActionProxy :  Action
{
    public string ActionName;
    
    public XXAction mAction;
    XXEntry _entry;
    XXEntry GetActionEntry()
    {
        if (_entry == null)
            _entry = Owner.GetComponent<XXEntry>();
        return _entry;
    }


    public override void OnReset()
    {
        base.OnReset();
        mAction = null;
        _entry = null;
        ActionName = null;
        //Debug.Log("OnReset");
    }

    public override void OnStart()
    {
        //XXActionEntry target = (Owner.GetVariable(XXActionEntry.Shared_TargetTran).GetValue() as Transform).GetComponent<XXActionEntry>();
        //Vector3 actionPos = (Vector3)Owner.GetVariable(XXActionEntry.Shared_TargetPos).GetValue();
        mAction = GetActionEntry().DoXXAction(ActionName, null, Vector3.zero);
    }

    public override TaskStatus OnUpdate()
    {
        if (mAction == null)
            return TaskStatus.Success;
        else if (mAction.mState == XXPeriodAction.XXActionState.acting)
            return TaskStatus.Running;
        else if (mAction.mState == XXPeriodAction.XXActionState.cancel)
            return TaskStatus.Failure;
        else if (mAction.mState == XXPeriodAction.XXActionState.unstart)
            return TaskStatus.Inactive;
        else if (mAction.mState == XXPeriodAction.XXActionState.finish)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        //Debug.Log("OnEnd");
    }

    public override void OnBehaviorComplete()
    {
        base.OnBehaviorComplete();
        //Debug.Log("OnBehaviorComplete");
    }

    public override void OnBehaviorRestart()
    {
        base.OnBehaviorRestart();
        //Debug.Log("OnBehaviorRestart");
    }


}
