using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace rtsNS
{
    public class Infantry : MonoBehaviour
    {
        public float MoveSpeed;

        void Update()
        {
        }

        Coroutine curCoro;

        public void Die()
        {
            if (curCoro != null)
                StopCoroutine(curCoro);
            transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);
            ColorManager.GetIns().ChangeColor(this, Color.red);
        }

        public void MoveTo(List<float> speedList, List<Transform> tranList, List<Vector3> deltaPosList=null)
        {
            float speed = speedList[0];
            Transform tarTran = tranList[0];
            Vector3 deltaPost = deltaPosList != null ? deltaPosList[0] : Vector3.zero;
            speedList.RemoveAt(0);
            tranList.RemoveAt(0);
            if (deltaPosList != null)
                deltaPosList.RemoveAt(0);
            curCoro = StartCoroutine(coroFollow(tarTran, deltaPost, speed, () =>
            {
                if (tranList.Count > 0)
                {
                    if (curCoro != null)
                        StopCoroutine(curCoro);
                    MoveTo(speedList, tranList, deltaPosList);
                }
            }));
        }

        IEnumerator coroFollow(Transform tarTran, Vector3 delta, float speed, Action callback)
        {
            //bool canStop = false;
            while (true)
            {
                Vector3 nextPoint = Vector3.zero;
                Vector3 tarPoint = tarTran.position + delta;
                float leftMoves = SongeUtil.MoveToPoint(transform.position, tarPoint, speed, ref nextPoint);
                transform.position = nextPoint;
                //canStop = leftMoves > 0;
                if (leftMoves > 0 && callback != null)
                    callback();
                yield return null;
            }
        }


    }

}


