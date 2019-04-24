using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCEntryCondition : Conditional
{
    public string ConditionName;
    XXEntry _entry;
    XXEntry GetActionEntry()
    {
        if (_entry == null)
            _entry = Owner.GetComponent<XXEntry>();
        return _entry;
    }

    public override TaskStatus OnUpdate()
    {
        return GetActionEntry().BehaviorCheck(ConditionName) ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnReset()
    {
        base.OnReset();
        ConditionName = "";
        _entry = null;
    }

}
