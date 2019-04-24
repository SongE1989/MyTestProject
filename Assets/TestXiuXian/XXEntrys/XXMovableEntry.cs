using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;

public class XXMovableEntry : XXEntry
{
    public const string Share_CurWeight = "CurWeight";
    public const string Share_TotalWeight = "TotalWeight";
    public const string Action_Pick = "Pick";
    public const string Action_FollowTarget = "FollowTarget";
    public const string Action_MoveToTarget = "MoveToTarget";
    public const string Action_MoveToPos = "MoveToPos";


    /// <summary>移动速度</summary>
    public float mMoveSpeed = 3f;
    /// <summary>跟随时的最小距离</summary>
    public float mCloseDistance = 3f;
    public Vector3 mMoveTargetPos;
    //public Transform mMoveTargetTran;
    public XXEntry mTargetEntry;

    public float TotalWeight = 10f;

    private float curWeight = 0f;
    public float CurWeight
    {
        get
        {
            return curWeight;
        }

        set
        {
            curWeight = Mathf.Min(value, TotalWeight);
            if (GetBehaviorTree() != null)
                GetBehaviorTree().SetVariableValue(Share_CurWeight, curWeight as object);
        }
    }

    protected override void Start()
    {
        CurWeight = 0f;
        TotalWeight = 10f;
        if(GetBehaviorTree() != null)
            GetBehaviorTree().SetVariableValue(Share_CurWeight, CurWeight as object);
        if (GetBehaviorTree() != null)
            GetBehaviorTree().SetVariableValue(Share_TotalWeight, TotalWeight as object);
    }

    public override XXAction DoXXAction(string actionName, XXEntry tar = null, Vector3 deltaPos = default(Vector3))
    {
        if (CurAction != null)
            CurAction.CancelAction();
        switch (actionName)
        {
            case "SetCachedEntryAsTarget":
                mTargetEntry = GetBehaviorTree().GetVariable("CachedEntry").GetValue() as XXEntry;
                return null;
            case Action_Pick:
                //Transform tarTran =  GetBehaviorTree().GetVariable(Shared_TargetTran).GetValue() as Transform;
                //XXHerb herb = tarTran.GetComponent<XXHerb>();

                XXHerb herb = mTargetEntry as XXHerb;


                if (herb.IsPicking)
                {
                    return null;
                }
                else
                {
                    BaseEffectAction pickAct = new BaseEffectAction
                    {
                        mActionName = Action_Pick,
                        mStartPeriod = new ActionPeriod(0f, true, true),
                        mActPeriod = new ActionPeriod(1f, true, true),
                        mEndPeriod = new ActionPeriod(0f, true, true),
                        mStartRange = 2f,
                        mActRange = 2f,
                        mSrc = this,
                        mTar = herb,
                    };
                    StartAction(pickAct);
                    return pickAct;
                }

            case "FindRandomTargetPos":
                var range = Random.Range(18f, 22f);
                var point = Random.insideUnitCircle;
                mMoveTargetPos = transform.position + new Vector3(point.x * range, 0, point.y * range);
                return null;

            case Action_FollowTarget:
                XXPeriodAction act = new XXPeriodAction {
                    mActionName = Action_FollowTarget,
                    mStartPeriod = new ActionPeriod(0, true, true),
                    mActPeriod = new ActionPeriod(-1, true, true),
                    mEndPeriod = new ActionPeriod(0, true, false),
                    mStartRange = -1f,
                    mActRange = -1f,
                };
                //mMoveTargetTran = GetBehaviorTree().GetVariable(Shared_TargetTran).GetValue() as Transform;
                //mTargetEntry = GetBehaviorTree().GetVariable(Shared_TargetEntry).GetValue() as XXEntry;
                StartAction(act);
                return act;

            case Action_MoveToTarget:
                XXPeriodAction mttAct = new XXPeriodAction
                {
                    mActionName = Action_MoveToTarget,
                    mStartPeriod = new ActionPeriod(0, true, true),
                    mActPeriod = new ActionPeriod(-1, true, true),
                    mEndPeriod = new ActionPeriod(0, true, false),
                    mStartRange = -1f,
                    mActRange = -1f,
                };
                //mMoveTargetTran = GetBehaviorTree().GetVariable(Shared_TargetTran).GetValue() as Transform;
                //mTargetEntry = GetBehaviorTree().GetVariable(Shared_TargetEntry).GetValue() as XXEntry;
                StartAction(mttAct);
                return mttAct;

            case Action_MoveToPos:
                XXPeriodAction moveToAct = new XXPeriodAction
                {
                    mActionName = Action_MoveToPos,
                    mStartPeriod = new ActionPeriod(0, true, true),
                    mActPeriod = new ActionPeriod(-1, true, true),
                    mEndPeriod = new ActionPeriod(0, true, false),
                    mStartRange = -1f,
                    mActRange = -1f,
                };
                StartAction(moveToAct);
                return moveToAct;
                //case Action_Pick
            default:
                return base.DoXXAction(actionName, tar, deltaPos);
        }
    }

    protected override void OnActionStart()
    {
        switch (CurAction.mActionName)
        {
            case Action_Pick:
                var herb = (CurAction.mTar as XXHerb);
                if (mAnimator)
                    mAnimator.SetBool("isCollect", true);
                herb.IsPicking = true;
                break;
            //case Action_FollowTarget:
            //case Action_MoveToTarget:
            //case Action_MoveToPos:
            //    if (mAnimator)
            //        mAnimator.SetFloat("moveSpeed", 1f);
            //    break;
            default:
                base.OnActionStart();
                break;
        }
    }

    protected override void OnActionActing(float time)
    {
        switch (CurAction.mActionName)
        {
            case Action_FollowTarget:
                moveToTarget();
                break;
            case Action_MoveToTarget:
                bool isReach1 = moveToTarget();
                if (isReach1)
                    CurAction.EndAction();
                break;
            case Action_MoveToPos:
                bool isReach2 = moveToPos();
                if (isReach2)
                    CurAction.EndAction();
                break;
            case Action_Pick:

                Debug.DrawLine(transform.position, CurAction.mTar.transform.position, Color.red);
                break;
            default:
                base.OnActionActing(time);
                break;
        }
    }

    protected override void OnActionEnd()
    {
        switch (CurAction.mActionName)
        {
            case Action_Pick:
                if (mAnimator)
                    mAnimator.SetBool("isCollect", false);
                //var herb = CurAction.mTar as XXHerb;
                //if(herb.m_kState == XXHerb.EnumHerbState.Fruited)
                //{
                //    herb.GetPicked();
                //    CurWeight++;
                    
                //}
                break;
            case Action_FollowTarget:
            case Action_MoveToTarget:
            case Action_MoveToPos:
                if (mAnimator)
                    mAnimator.SetFloat("moveSpeed", 0f);
                break;
            default:
                base.OnActionEnd();
                break;
        }
        CurAction = null;
    }

    protected override void OnActionCancel()
    {
        switch (CurAction.mActionName)
        {
            case Action_Pick:
                if(mAnimator)
                    mAnimator.SetBool("isCollect", false);
                (CurAction.mTar as XXHerb).IsPicking = false;
                break;
            case Action_FollowTarget:
            case Action_MoveToTarget:
            case Action_MoveToPos:
                if (mAnimator)
                    mAnimator.SetFloat("moveSpeed", 0f);
                break;
            default:
                base.OnActionCancel();
                break;
        }
        CurAction = null;
    }

    protected bool moveToPos()
    {
        return moveTo(mMoveTargetPos, mCloseDistance);
    }

    protected bool moveToTarget()
    {
        return moveTo(mTargetEntry.transform.position, mCloseDistance);
    }

    protected bool moveTo(Vector3 pos, float closeDistance)
    {
        if (Vector3.Distance(pos, transform.position) < closeDistance)
        {
            if (mAnimator)
                mAnimator.SetFloat("moveSpeed", 0f);
            return true;
        }
        else
        {
            Vector3 moveDir = (pos - transform.position).normalized * mMoveSpeed * Time.deltaTime;
            transform.Translate(moveDir, Space.World);
            if (mAnimator)
            {
                mAnimator.SetFloat("moveSpeed", 1f);
                if (moveDir.x > 0 && mAnimator.transform.eulerAngles != Vector3.zero)
                    mAnimator.transform.eulerAngles = Vector3.zero;
                else if (moveDir.x < 0 && mAnimator.transform.eulerAngles != new Vector3(0, 180, 0))
                    mAnimator.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            Debug.DrawLine(transform.position, pos);
            return false;
        }
    }

    #region MessageSystem

    #endregion
}
