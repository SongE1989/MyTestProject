using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;

public class XXEntry : MonoBehaviour, ILifeSystem, IMessageMemeber
{
    #region BehaviorTree

    BehaviorTree mBehaviorTree;
    public BehaviorTree GetBehaviorTree()
    {
        if (mBehaviorTree == null)
            mBehaviorTree = GetComponent<BehaviorTree>();
        return mBehaviorTree;
    }

    public virtual bool BehaviorCheck(string checkName)
    {
        return false;
    }

    #endregion

    public Animator mAnimator;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if(IsDead)
        {

        }
        else
        {
            if (CurAction != null && CurAction.IsActing())
            {
                CurAction.UpdateLogic();
            }
        }
    }

    public virtual void ReUse()
    {
        //mBehaviorTree = null;
    }

    #region XXAction
    public XXAction CurAction = null;

    public virtual XXAction DoXXAction(string actionName, XXEntry tar = null, Vector3 deltaPos = default(Vector3))
    {
        return null;
    }

    protected virtual void StartAction(XXAction tar)
    {
        CurAction = tar;
        CurAction.mOnActionStart = OnActionStart;
        CurAction.mOnActionEnd = OnActionEnd;
        CurAction.mOnActionCancel = OnActionCancel;
        CurAction.mOnActionActing = OnActionActing;
        CurAction.StartAction();
    }

    protected virtual void OnActionStart()
    {

    }

    protected virtual void OnActionActing(float time)
    {

    }

    protected virtual void OnActionEnd()
    {

    }

    protected virtual void OnActionCancel()
    {

    }
    #endregion

    #region ILifeSystem
    public int MaxHealthPoint { get; set; }

    int healthPoint = 100;
    public int HealthPoint
    {
        get
        {
            return healthPoint;
        }

        set
        {
            healthPoint = value;
            if(healthPoint <= 0)
            {
                healthPoint = 0;
                IsDead = true;
            }
        }
    }

    private bool isDead = false;
    private bool isDeadOldValue = false;
    public bool IsDead
    {
        get
        {
            return isDead;
        }

        set
        {
            isDeadOldValue = isDead;
            isDead = value;
            if (!isDeadOldValue && isDead)
            {
                //TODO 重写玩家死亡逻辑
                if(GetBehaviorTree() != null)
                    GetBehaviorTree().DisableBehavior();
                mAnimator.SetTrigger("isDead");
                if (CurAction != null)
                    CurAction.CancelAction();
            }
        }
    }

    public virtual void GetHit(XXEntry tar, int damage)
    {
        HealthPoint -= damage;
    }

    [ContextMenu("ShowInfo")]
    public void ShowInfo()
    {
        Debug.Log(IsDead+","+HealthPoint);
    }
    #endregion

    #region MessageSystem
    private MessageSystem _messageSystem;
    public MessageSystem mMessageSystem
    {
        get
        {
            return _messageSystem;
        }

        set
        {
            _messageSystem = value;
        }
    }

    public virtual void OnRecordEntryListChange(string recordType)
    {
        
    }
    #endregion

}