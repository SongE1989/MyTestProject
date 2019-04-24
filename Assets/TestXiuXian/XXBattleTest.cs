using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XXBattleTest : MonoBehaviour {
    public XXEntry Src;
    public XXEntry Tar;
    XXPeriodAction att;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (att == null || !att.IsActing())
            {
                att = CreateQingYuanJianAttackAction(Src, Tar);
                att.StartAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (att == null || !att.IsActing())
            {
                att = CreateFuLuAttackAction(Src, Tar);
                att.StartAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (att == null || !att.IsActing())
            {
                att = CreateMonsterAttackAction(Src, Tar);
                att.StartAction();
            }
        }

        if (Input.GetKey(KeyCode.W))//try cancel by move
        {
            if (att != null && att.IsActing())
            {
                bool canMove = att.TryCancelByMove();
                Debug.Log("尝试移动 canMove="+ canMove);
            }
        }


        if (att != null && att.IsActing())
        {
            att.UpdateLogic();
        }
    }
    //类似万剑归宗或王之财宝类的多道剑气依次飞出的攻击
    public static XXPeriodAction CreateQingYuanJianAttackAction(XXEntry src, XXEntry tar)
    {
        XXPeriodAction act = new XXPeriodAction
        {
            mStartPeriod = new ActionPeriod(1, true, false),
            mActPeriod = new ActionPeriod(100f, true, true),
            mEndPeriod = new ActionPeriod(0.5f, true, false),
            mStartRange = 5f,
            mActRange = 15f,
            mSrc = src,
            mTar = tar,
        };
        return act;
    }

    //符箓攻击
    public static XXPeriodAction CreateFuLuAttackAction(XXEntry src, XXEntry tar)
    {
        XXPeriodAction act = new XXPeriodAction
        {
            mStartPeriod = new ActionPeriod(0, true, false),
            mActPeriod = new ActionPeriod(0, true, true),
            mEndPeriod = new ActionPeriod(0.5f, true, false),
            mStartRange = 10f,
            mActRange = 10f,
            mSrc = src,
            mTar = tar,
        };
        return act;
    }

    //怪物攻击
    public static XXPeriodAction CreateMonsterAttackAction(XXEntry src, XXEntry tar)
    {
        XXPeriodAction act = new XXPeriodAction
        {
            mStartPeriod = new ActionPeriod(0.6f, false, false),
            mActPeriod = new ActionPeriod(0, false, false),
            mEndPeriod = new ActionPeriod(1f, false, false),
            mStartRange = 3f,
            mActRange = 3.5f,
            mSrc = src,
            mTar = tar,
        };
        return act;
    }
}
