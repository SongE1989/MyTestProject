using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCDistanceCheck : Conditional
{
    public SharedTransformList TargetList;
    public float Distance;

    public override TaskStatus OnUpdate()
    {
        if(TargetList.Value == null)
            return TaskStatus.Failure;
        if(TargetList.Value.Count == 0)
            return TaskStatus.Failure;
        for (int i = 0,length = TargetList.Value.Count; i < length; i++)
        {
            var target = TargetList.Value[i];
            if (Vector3.Distance(target.position, transform.position) > Distance)
                return TaskStatus.Failure;
            else
                return TaskStatus.Success;
        }
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
    }
}
