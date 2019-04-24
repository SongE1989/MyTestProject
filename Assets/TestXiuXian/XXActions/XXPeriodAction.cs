using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻击行为
//--Update逻辑
//--效果逻辑
//--移动逻辑
//--取消/中断逻辑


public class XXPeriodAction : XXAction
{

    //-----------
    public ActionPeriod mStartPeriod;
    public ActionPeriod mActPeriod;
    public ActionPeriod mEndPeriod;
    //-----------

    protected override void reset()
    {
        m_kCurPeriodTime = 0;
        m_kCurPeriod = null;
        base.reset();
    }

    public override void StartAction()
    {
        if (!CanAct())
        {
            Debug.Log("距离外, 无法开始");
            return;
        }
        m_kCurPeriod = mStartPeriod;
        m_kCurPeriodTime = 0;
        //Debug.Log("StartAction"+mActionName);
        if (mOnActionStart != null)
            mOnActionStart();
        mState = XXActionState.acting;
    }

    public override void UpdateLogic()
    {
        if (m_kCurPeriod == null)
            return;
        else if (m_kCurPeriod == mStartPeriod)
        {
            m_kCurPeriodTime += Time.deltaTime;
            if (m_kCurPeriod.m_kTime >= 0 && m_kCurPeriodTime >= m_kCurPeriod.m_kTime)
            {
                //准备结束, 执行动作
                m_kCurPeriod = mActPeriod;
                m_kCurPeriodTime = 0;
            }
            return;
        }
        else if (m_kCurPeriod == mActPeriod)
        {
            if(m_kHasCancel)
            {
                return;
            }
            else
            {
                if(!DistanceCheck())
                {
                    Debug.Log("超出范围");
                    m_kCurPeriod = mEndPeriod;
                    return;
                }
                else
                {
                    m_kCurPeriodTime += Time.deltaTime;
                    if (m_kCurPeriod.m_kTime >= 0 && m_kCurPeriodTime >= m_kCurPeriod.m_kTime)
                    {
                        OnActionActing(m_kCurPeriodTime);
                        m_kCurPeriod = mEndPeriod;
                        m_kCurPeriodTime = 0;
                    }
                    else
                    {
                        OnActionActing(m_kCurPeriodTime);
                    }
                    return;
                }
            }
        }
        else if (m_kCurPeriod == mEndPeriod)
        {
            m_kCurPeriodTime += Time.deltaTime;
            if (m_kCurPeriod.m_kTime >= 0 && m_kCurPeriodTime >= m_kCurPeriod.m_kTime)
            {
                OnActionEnd();
            }
            else
            {
            }
            return;
        }
    }
}