using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RandomAnima : StateMachineBehaviour
{
    public string RandomIntName = "RandomIntName";
    public List<int> RandomValues = new List<int>();
    public bool MachEnter = true;
    public bool MachExit = true;
    public bool StateEnter = true;
    public bool StateExit = true;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if(MachEnter)
            doRandom(animator);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (MachExit)
            doRandom(animator);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        if (MachExit)
            doRandom(animator);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (StateEnter)
            doRandom(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (StateExit)
            doRandom(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        if (StateExit)
            doRandom(animator);
    }

    void doRandom(Animator animator)
    {
        try
        {
            int rValue = RandomValues[Random.Range(0, RandomValues.Count)];
            //Debug.Log("doRandom "+ rValue);
            animator.SetInteger(RandomIntName, rValue);
        }
        catch (System.Exception exp)
        {
            Debug.Log(exp); 
        }
    }
    
}
