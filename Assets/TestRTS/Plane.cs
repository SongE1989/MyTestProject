using UnityEngine;
using System.Collections;
namespace rtsNS
{
    public class Plane : MonoBehaviour
    {
        public Vector3 CurSpeedVec = new Vector3();
        public float CurSpeed = 0;
        public float Acc = 0.001f;
        public float StopAcc = 0.004f;
        public float MaxSpeed = 0.2f;
        public float MinSpeed = 0.03f;
        public float TurnSpeed = 10f;
        public float FireTime = 2f;

        Coroutine curCoro;

        public void ToPosition(Transform firePos, Transform fireTarget, Transform awayPos, bool keepHeight = true)
        {
            if (curCoro != null)
                StopCoroutine(curCoro);
            //curCoro = StartCoroutine(coroToTran(tar, keepHeight));
            //curCoro = StartCoroutine(CoroToPosition(tar));
            curCoro = StartCoroutine(GoFireGo(firePos, fireTarget, awayPos));
        }

        IEnumerator GoFireGo(Transform firePos, Transform fireTarget, Transform awayPos)
        {
            yield return StartCoroutine(CoroToPosition(firePos));
            yield return StartCoroutine(CoroFireAt(fireTarget));
            yield return StartCoroutine(CoroToPosition(awayPos));
        }

        IEnumerator CoroFireAt(Transform fireTarget)
        {
            if (FireTime <= 0)
                yield break;
            float startTime = Time.realtimeSinceStartup;
            while(Time.realtimeSinceStartup - startTime < FireTime)
            {
                MoveUtils.PlaneBrake(transform, ref CurSpeedVec, MinSpeed, StopAcc);
                MoveUtils.Rotate(transform, fireTarget.position - transform.position, TurnSpeed);
                yield return null;
            }
        }

        IEnumerator CoroToPosition(Transform tar)
        {
            bool hasReached = false;
            bool hasTurnTo = false;
            
            while (true)
            {
                if (tar == null)
                    yield return null;

                if(!hasTurnTo)//转向
                {
                    bool isTurn = MoveUtils.Rotate(transform, tar.position - transform.position, TurnSpeed);
                    if (isTurn)
                        hasTurnTo = true;
                }
                else
                {
                    if (hasReached)
                    {
                        MoveUtils.PlaneBrake(transform, ref CurSpeedVec, MinSpeed, StopAcc);
                        break;
                    }
                    else
                    {
                        Vector3 dir = tar.position - transform.position;
                        bool isReach = MoveUtils.PlaneMove(transform, tar.position, ref CurSpeed, MaxSpeed, MinSpeed, Acc, StopAcc);//开始向目标前进
                                                                                                                                    //else
                        if (isReach)
                        {
                            CurSpeedVec = dir.normalized.GetScaledVector(CurSpeed);

                            hasReached = true;
                        }
                    }
                }




                yield return null;
            }
        }
    }
}