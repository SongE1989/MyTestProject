using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("BTAction")]
public class BTActionFollow : BehaviorDesigner.Runtime.Tasks.Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Running;
    }
    public override void OnReset()
    {
        Debug.Log("OnReset");
    }
    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log("OnEnd");
    }

    public override void OnBehaviorComplete()
    {
        base.OnBehaviorComplete();
        Debug.Log("OnBehaviorComplete");
    }

    public override void OnBehaviorRestart()
    {
        base.OnBehaviorRestart();
        Debug.Log("OnBehaviorRestart");
    }
}
