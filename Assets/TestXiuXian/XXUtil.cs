using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class XXUtil
{
    public class Timer
    {
        public float mTime;
        public float mTimer;
        public Action mCallback;

        public Timer(float time, Action callback)
        {
            mTime = time;
            mTimer = 0;
            mCallback = callback;
        }

        public void Tick()
        {
            mTimer += Time.deltaTime;
            if (mTimer >= mTime)
            {
                mTimer -= mTime;
                mCallback();
            }
        }

        public void Reset(float time=0, Action callback=null)
        {
            mTimer = 0;
            mTime = time;
            mCallback = callback;
        }
    }

    public static XXEntry GetActionEntryByMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int mask = LayerMask.GetMask(new string[] { GameXiuXian.Layer_Selectable });
        if (Physics.Raycast(ray, out hitInfo, mask))
        {
            var tar = hitInfo.collider.gameObject;
            var acEntry = tar.GetComponentInParent<XXEntry>();
            if (acEntry != null)
                return acEntry;
        }
        return null;
    }

    public static XXEntry GetNearestEntry(IEnumerable<XXEntry> enumerable, Vector3 pos, Func<XXEntry,bool> judge = null)
    {
        XXEntry tarEntry = null;
        float minDis = float.MaxValue;
        foreach (var entry in enumerable)
        {
            if(judge != null && !judge(entry))
            {
                continue;
            }
            float dis = Vector3.Distance(entry.transform.position, pos);
            if (tarEntry == null || dis < minDis)
            {
                minDis = dis;
                tarEntry = entry;
            }
        }
        return tarEntry;
    }

    //public class PatrolLogic
    //{
    //    public XXMovableEntry m_kTarget;
    //    public float m_kMoveTime = 1f;
    //    public float m_kStandTime = 1f;
    //    XXUtil.Timer m_kTimer;

    //    public PatrolLogic(XXMovableEntry tar, float moveTime, float standTime)
    //    {
    //        m_kTarget = tar;
    //        m_kMoveTime = moveTime;
    //        m_kStandTime = standTime;
    //        //m_kTimer = new Timer(m_kStandTime, switchMoveState);
    //    }

    //    //void switchMoveState()
    //    //{
    //    //    if (m_kTarget.m_kMoveState == XXMovableEntry.Enum_MoveState.MS_Move)
    //    //    {
    //    //        m_kTarget.StopMove();
    //    //        m_kTimer.Reset(m_kStandTime, switchMoveState);
    //    //    }
    //    //    else if (m_kTarget.m_kMoveState == XXMovableEntry.Enum_MoveState.MS_Stand)
    //    //    {
    //    //        m_kTarget.SetMoveTarget(m_kTarget.SelectPatrolTarget());
    //    //        m_kTimer.Reset(m_kMoveTime, switchMoveState);
    //    //    }
    //    //}

    //    public void Tick()
    //    {
    //        m_kTimer.Tick();
    //    }
    //}
}