using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("BTAction")]
//[TaskDescription("Returns the distance between two Vector3s.")]
public class BTActionGather : Action
{
    public SharedFloat mMaxWeight;
    public SharedFloat mCurWeight;

    public override TaskStatus OnUpdate()
    {
        if ((float)mCurWeight.GetValue() >= (float)mMaxWeight.GetValue())
            return TaskStatus.Success;
        else
        {
            Debug.Log("gathering("+ mMaxWeight+"/"+ mCurWeight+")");
            return TaskStatus.Running;
        }
    }
}
