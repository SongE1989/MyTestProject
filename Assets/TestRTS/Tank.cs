using UnityEngine;
using System.Collections;
namespace rtsNS
{
    public class Tank : MonoBehaviour
    {
        public GameObject Head;
        public float Acc = 0.0001f;
        public float StopAcc = 0.004f;
        //public float TurningAcc = 0.001f;
        public float MaxSpeed = 0.08f;
        public float RotateSpeed = 0.5f;//转向速度(angle/s)

        public float CurSpeed = 0;



        Coroutine curCoro;
        public void ToPosition(Transform tar)
        {
            if (curCoro != null)
                StopCoroutine(curCoro);
            curCoro = StartCoroutine(CoroToPosition(tar));
        }

        IEnumerator CoroToPosition(Transform tar)
        {
            while (true)
            {
                if (tar == null)
                    yield return null;

                //1.旋转车头朝向目标
                //2.开始朝目标加速前进
                //3.判断是否该开始减速
                //4.减速停车
                //5.到达后调整车头朝向目标朝向

                float deviationDistance = 0.01f;
                float deviationDegree = 1f;

                bool isReach = Vector3.Distance(transform.position, tar.position) < deviationDistance;
                bool isRotateTowardFinish = Vector3.Angle(transform.forward, tar.position - transform.position) < deviationDegree;

                if (!isReach)
                {
                    if (!isRotateTowardFinish)
                        isRotateTowardFinish = MoveUtils.RotateTowardsHorizontal(transform, tar.position - transform.position, RotateSpeed, deviationDegree);//车身朝向目标
                    else
                        MoveUtils.MoveWithAcc(transform, tar.position, ref CurSpeed, MaxSpeed,0, Acc, StopAcc, deviationDistance);//开始向目标前进
                    MoveUtils.RotateTowardsHorizontal(Head.transform, transform.forward, RotateSpeed, deviationDegree);//炮塔指向目标前方
                }
                else
                {
                    MoveUtils.RotateTowardsHorizontal(Head.transform, tar.forward, RotateSpeed, deviationDegree);//炮塔指向目标前方
                }
                yield return null;
            }
        }

        public void Die()
        {
            if (curCoro != null)
                StopCoroutine(curCoro);
            transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);
            ColorManager.GetIns().ChangeColor(this, Color.red);
        }

    }
}