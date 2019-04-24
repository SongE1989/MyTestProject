using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>基础效果动作(完成时生效)</summary>
public class BaseEffectAction : XXPeriodAction
{
    public override void ActionEffect()
    {
        base.ActionEffect();

        switch (mActionName)
        {
            case XXMovableEntry.Action_Pick:
                if (m_kCurPeriodTime >= m_kCurPeriod.m_kTime)
                {
                    (mTar as XXHerb).GetPicked();
                    var srcEntry = mSrc as XXMovableEntry;
                    srcEntry.CurWeight++;
                }
                break;
            case XXPlayer.Action_Attack1:
                {
                    if (m_kCurPeriodTime >= m_kCurPeriod.m_kTime)
                        mTar.GetHit(mSrc, 10);
                }
                break;
        }
    }
}
