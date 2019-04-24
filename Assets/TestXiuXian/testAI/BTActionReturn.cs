using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("BTAction")]
public class BTActionReturn : BehaviorDesigner.Runtime.Tasks.Action
{
    public SharedTransform mTargetTransform;
    public SharedTransform mMyTransform;
    float mCatchDistanc = 2f;
    public override TaskStatus OnUpdate()
    {
        float dis = Vector3.Distance(((Transform)mMyTransform.GetValue()).position, ((Transform)mTargetTransform.GetValue()).position);
        if (dis > mCatchDistanc)
        {
            Debug.Log("MoveTo");
            return TaskStatus.Running;
        }
        else
            return TaskStatus.Success;
    }
}
