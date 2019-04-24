using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XXStudent : XXAIEntry
{
    public const string Msg_OnCollect = "OnColect";

    public XXSchool m_kSchool;
    public XXMaster m_kMaster;
    public bool IsFree = true;
    public const string Action_FindHerb = "FindHerb";
    public const string Action_TargetMaster = "TargetMaster";
    public const string Action_GiveRes = "GiveRes";

    protected override void Start()
    {
        CurWeight = 0f;
        TotalWeight = 2f;
        if (GetBehaviorTree() != null)
            GetBehaviorTree().SetVariableValue(Share_CurWeight, CurWeight as object);
        if (GetBehaviorTree() != null)
            GetBehaviorTree().SetVariableValue(Share_TotalWeight, TotalWeight as object);
    }

    public override void ReUse()
    {
        base.ReUse();
        IsFree = true;
    }

    public override XXAction DoXXAction(string actionName, XXEntry tar = null, Vector3 deltaPos = default(Vector3))
    {
        if (CurAction != null)
            CurAction.CancelAction();
        switch (actionName)
        {
            case Action_FindHerb:
                XXPeriodAction ac = new XXPeriodAction
                {
                    mActionName = Action_FindHerb
                };
                var herbSet = EntryManager.Instance.GetEntrySetByName("Grass");
                XXEntry tarHerb = XXUtil.GetNearestEntry(herbSet, transform.position, entry => 
                {
                    if(mMessageSystem.CheckRecordEntry(Msg_OnCollect, entry))
                        return false;
                    else
                        return (entry as XXHerb).CanPick();
                });
                
                if (tarHerb == null)
                {
                    ac.CancelAction();
                    return ac;
                }
                else
                {
                    //GetBehaviorTree().SetVariableValue(Shared_TargetEntry, tarHerb);
                    //GetBehaviorTree().SetVariableValue(Shared_TargetPos, tarHerb.transform.position);
                    mTargetEntry = tarHerb;
                    mMessageSystem.AddRecordEntry(Msg_OnCollect, tarHerb);
                    StartAction(ac);
                    ac.EndAction();
                    return ac;

                }
            case Action_TargetMaster:
                XXPeriodAction tmAC = new XXPeriodAction()
                {
                    mActionName = Action_TargetMaster
                };
                mTargetEntry = m_kMaster;
                StartAction(tmAC);
                tmAC.EndAction();
                return tmAC;

            case Action_GiveRes:
                var grAC = new XXPeriodAction()
                {
                    mActionName = Action_GiveRes
                };
                StartAction(grAC);
                float masterWeightSpace = m_kMaster.TotalWeight - m_kMaster.CurWeight;
                if (masterWeightSpace > 0)
                {
                    float giveNum = Mathf.Min(CurWeight, masterWeightSpace);
                    if (giveNum > 0)
                    {
                        m_kMaster.CurWeight += giveNum;
                        CurWeight -= giveNum;
                    }
                    grAC.EndAction();
                }
                else
                {
                    grAC.CancelAction();
                }
                if (GetBehaviorTree() != null)
                    GetBehaviorTree().SetVariableValue(Share_CurWeight, CurWeight as object);
                return grAC;
            default:
                return base.DoXXAction(actionName, tar, deltaPos);
        }

    }

    protected override void OnActionCancel()
    {
        switch (CurAction.mActionName)
        {
            case Action_FindHerb:
                if (mMessageSystem != null)
                    mMessageSystem.RemoveRecordEntry(Msg_OnCollect,CurAction.mTar);
                base.OnActionCancel();
                break;
            default:
                base.OnActionCancel();
                break;
        }
    }

    protected override void OnActionEnd()
    {
        switch (CurAction.mActionName)
        {
            case Action_FindHerb:
                if (mMessageSystem != null)
                    mMessageSystem.RemoveRecordEntry(Msg_OnCollect, CurAction.mTar);
                base.OnActionEnd();
                break;
            default:
                base.OnActionEnd();
                break;
        }
    }

}
