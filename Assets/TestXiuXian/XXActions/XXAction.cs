using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XXAction
{
    public enum XXActionState
    {
        unstart,
        acting,
        cancel,
        finish
    }
    public string mActionName;
    public XXActionState mState;
    /// <summary>动作发动范围(小于等于0时不限制)</summary>
    public float mStartRange;
    /// <summary>动作执行范围(小于等于0时不限制)</summary>
    public float mActRange;
    //-----------
    public XXEntry mSrc;
    public XXEntry mTar;

    public bool m_kHasCancel = false;
    //-----------
    public Action mOnActionStart;
    public Action mOnActionEnd;
    public Action mOnActionCancel;
    public Action<float> mOnActionActing;

    //-----------
    public ActionPeriod m_kCurPeriod;
    public float m_kCurPeriodTime = 0f;

    protected virtual void reset()
    {
        m_kCurPeriodTime = 0;
        m_kCurPeriod = null;
        m_kHasCancel = false;
        mOnActionStart = null;
        mOnActionEnd = null;
        mState = XXActionState.unstart;
    }

    public bool CanAct()
    {
        if (mStartRange <= 0)
            return true;
        else
            return Vector3.Distance(mSrc.transform.position, mTar.transform.position) <= mStartRange;
    }

    public virtual bool CanCancel()
    {
        if (m_kCurPeriod == null)
            return true;
        else
            return m_kCurPeriod.m_kCanMove && m_kCurPeriod.m_kCancelByMove;
    }

    public bool IsActing()
    {
        return mState == XXActionState.acting;
    }

    public virtual void StartAction()
    {
        if (!CanAct())
        {
            Debug.Log("距离外, 无法开始");
            return;
        }
        m_kCurPeriodTime = 0;
        if (mOnActionStart != null)
            mOnActionStart();
        mState = XXActionState.acting;
    }
    public void CancelAction()
    {
        //Debug.Log("CancelAction"+mActionName);
        mState = XXActionState.cancel;
        m_kCurPeriod = null;
        if (mOnActionCancel != null)
            mOnActionCancel();
    }


    public void EndAction()
    {
        //Debug.Log("EndAction" + mActionName);
        mState = XXActionState.finish;
        m_kCurPeriod = null;
        if (mOnActionEnd != null)
            mOnActionEnd();
    }

    protected void OnActionActing(float time)
    {
        //Debug.Log("OnActionActing"+ time);
        if (mSrc != null && mTar != null)
            Debug.DrawLine(mSrc.transform.position, mTar.transform.position, Color.red);
        if (mOnActionActing != null)
            mOnActionActing(time);
        ActionEffect();
    }

    protected void OnActionEnd()
    {
        mState = XXActionState.finish;
        //Debug.Log("OnActionEnd" + mActionName);
        if (mOnActionEnd != null)
            mOnActionEnd();
    }

    public bool DistanceCheck()
    {
        if (mSrc == null || mTar == null || mActRange <= 0)
            return true;
        else
            return Vector3.Distance(mSrc.transform.position, mTar.transform.position) < mActRange;
    }

    public virtual void UpdateLogic()
    {
        if (m_kCurPeriod == null)
            return;
        if (m_kCurPeriod.m_kTime >= 0 && m_kCurPeriodTime >= m_kCurPeriod.m_kTime)
        {
            OnActionEnd();
            return;
        }
        else
            OnActionActing(m_kCurPeriodTime);
    }

    public bool CanMove()
    {
        if (m_kCurPeriod == null)
            return true;
        else
            return m_kCurPeriod.m_kCanMove;
    }

    public bool TryCancelByMove()
    {
        if (m_kCurPeriod == null)
            return true;
        else
        {
            if (CanMove())
            {
                //Debug.Log("可以移动");
                if (m_kCurPeriod.m_kCancelByMove)
                {
                    Debug.Log("移动取消");
                    CancelAction();
                }
                return true;
            }
            else
            {
                //Debug.Log("不可移动");
                return false;
            }
        }
    }

    public virtual void ActionEffect()
    {
    }
}


public class ActionPeriod
{
    /// <summary>阶段持续时间(小于0时无限时间)</summary>
    public float m_kTime;
    /// <summary>是否可移动</summary>
    public bool m_kCanMove;
    /// <summary>移动是否打断</summary>
    public bool m_kCancelByMove;
    public ActionPeriod(float time, bool canMove, bool cancelByMove)
    {
        m_kTime = time;
        m_kCanMove = canMove;
        m_kCancelByMove = cancelByMove;
    }
}