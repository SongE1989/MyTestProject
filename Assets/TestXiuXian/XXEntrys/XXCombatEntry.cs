using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XXCombatEntry : XXMovableEntry, ICombatSystem
{
    public HashSet<XXEntry> EnemyList { get; set; }

    private bool _isInCombat;
    public bool IsInCombat
    {
        get
        {
            return _isInCombat;
        }

        set
        {
            _isInCombat = value;
            GetBehaviorTree().SetVariableValue("inCombat", _isInCombat);
        }
    }

    public override bool BehaviorCheck(string checkName)
    {
        switch (checkName)
        {
            case "HPLow":
                return HealthPoint < MaxHealthPoint*0.3f;
            case "TargetDead":
                return mTargetEntry == null ? true : mTargetEntry.IsDead;
            case "TargetIsEnemy":
                return EnemyList != null && EnemyList.Contains(mTargetEntry);
            case "HasEnemy":
                return EnemyList != null && EnemyList.Count > 0;
            case "TargetNotInRange":
                return mTargetEntry != null && Vector3.Distance(transform.position, mTargetEntry.transform.position) > 1f;
            default:
                return base.BehaviorCheck(checkName);
        }
    }

    public override XXAction DoXXAction(string actionName, XXEntry tar = null, Vector3 deltaPos = default(Vector3))
    {
        if (CurAction != null)
            CurAction.CancelAction();
        switch (actionName)
        {
            case "RemoveEnemy":
                if (EnemyList.Contains(mTargetEntry))
                    RemoveEnemy(mTargetEntry);

                return null;

            case "AttackEnemy":
                BaseEffectAction act = new BaseEffectAction
                {
                    mActionName = XXPlayer.Action_Attack1,
                    mStartPeriod = new ActionPeriod(0, true, true),
                    mActPeriod = new ActionPeriod(0.4f, true, true),
                    mEndPeriod = new ActionPeriod(0.3f, true, true),
                    mStartRange = 1f,
                    mActRange = 2f,
                    mSrc = this,
                    mTar = mTargetEntry,
                };
                StartAction(act);
                return act;
            case "SelectEnemyTarget":
                if (EnemyList.Count > 0)
                {
                    var enumerator = EnemyList.GetEnumerator();
                    enumerator.MoveNext();
                    mTargetEntry = enumerator.Current;
                }
                return null;
            default:
                return base.DoXXAction(actionName, tar, deltaPos);
        }
    }

    public override void ReUse()
    {
        base.ReUse();
        if (EnemyList != null)
            EnemyList.Clear();
        else
            EnemyList = new HashSet<XXEntry>();
    }

    public override void GetHit(XXEntry tar, int damage)
    {
        base.GetHit(tar, damage);
        AddEnemy(tar);
        if (mMessageSystem != null)
            mMessageSystem.AddRecordEntry("Enemy", tar);
    }

    public override void OnRecordEntryListChange(string recordType)
    {
        base.OnRecordEntryListChange(recordType);
        if(recordType == "Enemy")
        {
            EnemyList.UnionWith(mMessageSystem.RecordEntryDic[recordType]);
            onEnemyChange();
        }
    }

    public virtual void AddEnemy(XXEntry tar)
    {
        if (!EnemyList.Contains(tar))
            EnemyList.Add(tar);
        onEnemyChange();
    }

    public virtual void RemoveEnemy(XXEntry tar)
    {
        if (EnemyList.Contains(tar))
            EnemyList.Remove(tar);
        onEnemyChange();
    }

    protected virtual void onEnemyChange()
    {
        IsInCombat = EnemyList.Count > 0;
    }

    protected override void OnActionStart()
    {
        switch (CurAction.mActionName)
        {
            case XXPlayer.Action_Attack1:
                mAnimator.SetTrigger("attack");
                break;
            default:
                base.OnActionStart();
                break;
        }
    }
}
