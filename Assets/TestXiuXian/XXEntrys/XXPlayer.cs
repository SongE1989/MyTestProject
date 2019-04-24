using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class XXPlayer : XXMovableEntry
{
    public const string Action_ConstAttack = "ConstAttack";
    public const string Action_Attack1 = "Attack1";
    public const string Action_Attack3 = "Attack3";
    public const string Action_Player_GoToPick = "Player_GoToPick";
    public const string Action_Player_GoToAttack = "Player_GoToAttack";

    public float PlayerMoveSpeed = 5f;
    public Rigidbody m_Rigidbody;
    public float actionMaxDis = 1f;
    //public Transform HandModel;
    public Transform m_ModelNode;

    public bool IsActionInput = false;

    protected override void Start()
    {
        base.Start();
        MaxHealthPoint = HealthPoint = 1000;
    }

    void actionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("mouse down");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            int mask = LayerMask.GetMask(new string[] { GameXiuXian.Layer_Selectable });
            if (Physics.Raycast(ray, out hitInfo, mask))
            {
                var tar = hitInfo.collider.gameObject;
                var acEntry = tar.GetComponentInParent<XXEntry>();
                if (acEntry != null)
                    Debug.Log(acEntry.name);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            //Debug.Log("mouse hold");
        }


        if (Input.GetKey(KeyCode.Space))
        {
            if (CurAction == null)
            {
                var herbSet = EntryManager.Instance.GetEntrySetByName("Grass");
                mPlayerTarget = XXUtil.GetNearestEntry(herbSet, transform.position, entry => { return (entry as XXHerb).CanPick(); });
                if(mPlayerTarget != null)
                {
                    DoXXAction(Action_Player_GoToPick);
                }
            }
        }
        if (Input.GetKey(KeyCode.F))
        {
            if (CurAction == null)
            {
                var enemySet1 = EntryManager.Instance.GetEntrySetByName("Student");
                var enemySet2 = EntryManager.Instance.GetEntrySetByName("Master");
                if(enemySet1 != null || enemySet2 != null)
                {
                    HashSet<XXEntry> tarSet = new HashSet<XXEntry>();
                    if(enemySet1 != null)
                        tarSet.UnionWith(enemySet1);
                    if(enemySet2 != null)
                        tarSet.UnionWith(enemySet2);
                    mPlayerTarget = XXUtil.GetNearestEntry(tarSet, transform.position, tar=> { return !IsDead; });
                    if (mPlayerTarget != null)
                    {
                        DoXXAction(Action_Player_GoToAttack, mPlayerTarget);
                    }
                }
            }
        }
        IsActionInput = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.F);
        //KeepApproachingTarget();


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    tryPickHerb();
        //}
        if (!Input.GetKey(KeyCode.Alpha1))
        {
            if (CurAction != null && CurAction.mActionName == Action_ConstAttack && CurAction.IsActing())
                CurAction.CancelAction();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DoXXAction(Action_ConstAttack, this);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DoXXAction(Action_Attack3, this);
        }
    }


    void moveInput()
    {
        if (IsActionInput)
            return;
        Vector3 moveDir = new Vector3();
        if (moveDir != Vector3.zero)//动作移动优先级高于WASD
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }

        if (moveDir != Vector3.zero)
        {
            bool canMove = true;
            if (CurAction != null && CurAction.IsActing())
            {
                canMove = CurAction.TryCancelByMove();
            }

            if (canMove)
            {
                moveTo(transform.position + moveDir * PlayerMoveSpeed * Time.deltaTime, 0);
            }
            else
                mAnimator.SetFloat("moveSpeed", 0f);
        }
        else
        {
            mAnimator.SetFloat("moveSpeed", 0f);
        }
    }

    public void Control()
    {
        moveInput();
        actionInput();
    }

    public override XXAction DoXXAction(string actionName, XXEntry tar = null, Vector3 deltaPos = default(Vector3))
    {
        if (CurAction != null)
            CurAction.CancelAction();
        switch (actionName)
        {
            case Action_Pick:
                if ((tar as XXHerb).CanPick())
                    return PickHerb(tar as XXHerb);
                else
                    return null;

            case Action_ConstAttack:
                return DoConstAttack(mPlayerTarget);

            case Action_Attack3:
                return DoAttack3(mPlayerTarget);

            case Action_Attack1:
                return DoAttack1(mPlayerTarget);

            case Action_Player_GoToPick:
                if (CurAction != null && CurAction.IsActing())
                    return null;
                XXAction act = new XXAction
                {
                    mActionName = Action_Player_GoToPick,
                    m_kCurPeriod = new ActionPeriod(-1, true, true),
                    mStartRange = -1f,
                    mActRange = -1f,
                    mSrc = this,
                    mTar = tar,
                };
                StartAction(act);
                return act;

            case Action_Player_GoToAttack:
                if (CurAction != null && CurAction.IsActing())
                    return null;
                XXAction apgAct = new XXAction
                {
                    mActionName = Action_Player_GoToAttack,
                    m_kCurPeriod = new ActionPeriod(-1, true, true),
                    mStartRange = -1f,
                    mActRange = -1f,
                    mSrc = this,
                    mTar = tar,
                };
                StartAction(apgAct);
                return apgAct;
            default:
                return base.DoXXAction(actionName, tar, deltaPos);
        }
    }

    /// <summary>玩家操作目标</summary>
    public XXEntry mPlayerTarget;
    public string mPlayerNextActionName;
    
    #region XXAction

    public BaseEffectAction PickHerb(XXHerb tar)
    {
        if (CurAction != null && CurAction.mActionName == Action_Pick && CurAction.IsActing())
            return null;

        BaseEffectAction act = new BaseEffectAction
        {
            mActionName = Action_Pick,
            mStartPeriod = new ActionPeriod(0f, true, true),
            mActPeriod = new ActionPeriod(1f, true, true),
            mEndPeriod = new ActionPeriod(0f, true, true),
            mStartRange = 1f,
            mActRange = 1f,
            mSrc = this,
            mTar = tar,
        };
        StartAction(act);
        return act;
    }

    public XXPeriodAction DoConstAttack(XXEntry tar)
    {
        if (CurAction != null && CurAction.IsActing())
            return null;
        //NOTE 需要摁住技能键
        XXPeriodAction act = new XXPeriodAction
        {
            mActionName = Action_ConstAttack,
            mStartPeriod = new ActionPeriod(1, true, true),
            mActPeriod = new ActionPeriod(100f, true, true),
            mEndPeriod = new ActionPeriod(0.5f, true, false),
            mStartRange = 5f,
            mActRange = 15f,
            mSrc = this,
            mTar = tar,
        };
        StartAction(act);
        return act;
    }

    public BaseEffectAction DoAttack1(XXEntry tar)
    {
        if (CurAction != null && CurAction.IsActing())
            return null;
        BaseEffectAction act = new BaseEffectAction
        {
            mActionName = Action_Attack1,
            mStartPeriod = new ActionPeriod(0, true, true),
            mActPeriod = new ActionPeriod(0.4f, true, true),
            mEndPeriod = new ActionPeriod(0.3f, true, true),
            mStartRange = 10f,
            mActRange = 10f,
            mSrc = this,
            mTar = tar,
        };
        StartAction(act);
        return act;
    }

    public XXPeriodAction DoAttack3(XXEntry tar)
    {
        if (CurAction != null && CurAction.IsActing())
            return null;
        XXPeriodAction act = new XXPeriodAction
        {
            mActionName = Action_Attack3,
            mStartPeriod = new ActionPeriod(0, false, false),
            mActPeriod = new ActionPeriod(1.217f, false, false),
            mEndPeriod = new ActionPeriod(0, false, false),
            mStartRange = 10f,
            mActRange = 10f,
            mSrc = this,
            mTar = tar,
        };
        StartAction(act);
        return act;
    }


    protected override void OnActionStart()
    {
        //Debug.Log("OnActionStart:"+CurAction.mActionName);
        switch (CurAction.mActionName)
        {
            case Action_ConstAttack:
                Debug.Log("Start Att");
                mAnimator.SetBool("constAttack", true);
                break;
            case Action_Attack3:
                mAnimator.SetTrigger("attack3");
                break;
            case Action_Attack1:
                mAnimator.SetTrigger("attack");
                break;
            default:
                base.OnActionStart();
                break;
        }
    }

    protected override void OnActionActing(float time)
    {
        //Debug.Log("OnActionActing:" + CurAction.mActionName);
        switch (CurAction.mActionName)
        {
            case Action_Player_GoToPick:
                {
                    bool isArrive = moveTo(mPlayerTarget.transform.position, 1f);//TODO Action的最小启动距离
                    if (isArrive)
                        CurAction.EndAction();
                    break;

                }
            case Action_Player_GoToAttack:
                {
                    bool isArrive = moveTo(mPlayerTarget.transform.position, 1f);//TODO Action的最小启动距离
                    if (isArrive)
                        CurAction.EndAction();
                    break;
                }
            case Action_Attack1:
                {
                }
                break;
            default:
                base.OnActionActing(time);
                break;
        }
        //TODO 此处需要处理持续攻击的伤害, 但不方便获取Action本身的数据
    }

    protected override void OnActionEnd()
    {
        //Debug.Log("OnActionEnd:" + CurAction.mActionName);
        bool startNewAction = false;//若在当前行为结束时开启新行为, 则提前清空CurAction, 之后不再清空(防止把新行为清空)
        switch (CurAction.mActionName)
        {
            case Action_ConstAttack:
                mAnimator.SetBool("constAttack", false);
                break;
            case Action_Player_GoToPick:
                startNewAction = true;
                CurAction = null;
                DoXXAction(Action_Pick, mPlayerTarget);
                break;
            case Action_Player_GoToAttack:
                startNewAction = true;
                CurAction = null;
                DoXXAction(Action_Attack1, mPlayerTarget);
                break;
            case Action_Attack1:
                Debug.Log("att1 end");
                break;
            default:
                base.OnActionEnd();
                break;
        }
        if(!startNewAction)
            CurAction = null;
    }

    protected override void OnActionCancel()
    {
        //Debug.Log("OnActionCancel:" + CurAction.mActionName);
        switch (CurAction.mActionName)
        {
            case Action_ConstAttack:
                mAnimator.SetBool("constAttack", false);
                break;

            case Action_Attack1:
                Debug.Log("att1 cancel");
                break;
            default:
                base.OnActionCancel();
                break;
        }
        CurAction = null;
    }
    #endregion
}
