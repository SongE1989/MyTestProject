using UnityEngine;
using System.Collections;
namespace rtsNS
{

    public static class MoveUtils
    {
        public static void PlaneBrake(Transform tar, ref Vector3 curSpeedVec, float minSpeed, float stopAcc)
        {
            if(curSpeedVec.magnitude > minSpeed)
            {
                float newSpeed = Mathf.Max(0, curSpeedVec.magnitude - stopAcc);
                curSpeedVec = Vector3.ClampMagnitude(curSpeedVec, newSpeed);
            }
            tar.position += curSpeedVec;
        }

        public static bool PlaneMove(Transform tar, Vector3 des, ref float curSpeed, float maxSpeed, float minSpeed, float acc, float stopAcc)
        {
            tar.LookAt(des);
            float distance = Vector3.Distance(tar.position, des);//所剩距离
            if (curSpeed < maxSpeed)//2.加速
            {
                curSpeed = Mathf.Min(maxSpeed, curSpeed + acc);
            }
            bool isArrive = false;
            SongeUtil.Approach(tar.position, des, curSpeed, out isArrive);
            tar.position += tar.forward.GetScaledVector(curSpeed);
            return isArrive;
        }

        public static bool MoveWithAcc(Transform tar, Vector3 des, ref float curSpeed, float maxSpeed, float minSpeed, float acc, float stopAcc, float deviationDistance = 0.01f)
        {
            //判断当前是加速/匀速/减速阶段
            //1.判断是否需要减速,若否,判断是否可以加速
            float distance = Vector3.Distance(tar.position, des);//所剩距离

            if (distance < deviationDistance)
                return true;

            float timeNeed = (curSpeed > minSpeed ? curSpeed - minSpeed : 0) / stopAcc;//减速时间
            float stopDistance = curSpeed * timeNeed - stopAcc * stopAcc / 2f;//刹车距离
            float safeDistance = deviationDistance;//安全距离(提前刹车距离)
            if (distance - stopDistance < safeDistance)//1.减速
            {
                curSpeed = Mathf.Max(minSpeed, curSpeed - stopAcc);
            }
            else if (curSpeed < maxSpeed)//2.加速
            {
                curSpeed = Mathf.Min(maxSpeed, curSpeed + acc);
            }
            else//3.匀速
            {

            }
            tar.position += tar.forward.GetScaledVector(curSpeed);
            return false;
        }

        static GameObject directorGO;
        static Transform director {
            get {
                if (directorGO == null)
                {
                    directorGO = new GameObject();

                }
                return directorGO.transform;
            }
        }
        public static bool Rotate(Transform tar, Vector3 dir, float rotateSpeed, float deviationDegree = 1f)
        {
            director.parent = tar.parent;
            director.position = tar.position;
            director.LookAt(director.position + dir);
            float angleBetween = Quaternion.Angle(director.localRotation, tar.localRotation);
            Debug.Log(angleBetween);
            tar.localRotation = Quaternion.RotateTowards(tar.localRotation, director.localRotation, rotateSpeed);
            angleBetween = Quaternion.Angle(director.localRotation, tar.localRotation);
            Debug.Log("=>"+angleBetween);
            return angleBetween < 0.0001;
        }

        public static bool RotateTowards(Transform tar, Vector3 dir, float rotateSpeed, float deviationDegree = 1f)
        {
            bool isHor = RotateTowardsHorizontal(tar, dir, rotateSpeed, deviationDegree);
            bool isVer = RotateTowardsVertical(tar, dir, rotateSpeed, deviationDegree);
            return isHor && isVer;
        }

        public static bool RotateTowardsVertical(Transform tar, Vector3 dir, float rotateSpeed, float deviationDegree = 1f)
        {
            float d1 = SongeUtil.GetRotatV(tar.forward);
            float d2 = SongeUtil.GetRotatV(dir);
            float deltaDegree = d2 - d1;
            float rotateAngle = 0;
            if (deltaDegree < -deviationDegree)
            {
                rotateAngle = Mathf.Min(-deltaDegree, -rotateSpeed);
                tar.Rotate(Vector3.right, rotateAngle);
                return rotateAngle == -deltaDegree;
            }
            else if (deltaDegree > deviationDegree)
            {
                rotateAngle = Mathf.Min(deltaDegree, rotateSpeed);
                tar.Rotate(Vector3.right, rotateAngle);
                return rotateAngle == deltaDegree;
            }
            else
            {
                return true;
            }
        }

        public static bool RotateTowardsHorizontal(Transform tar, Vector3 dir, float rotateSpeed, float deviationDegree = 1f)
        {
            float d1 = SongeUtil.GetRotateY(tar.forward);
            float d2 = SongeUtil.GetRotateY(dir);
            float deltaDegree = d2 - d1;
            while (deltaDegree > 180)
                deltaDegree -= 360;
            while (deltaDegree < -180)
                deltaDegree += 360;
            float rotateAngle = 0;
            if (deltaDegree < -deviationDegree)
            {
                rotateAngle = Mathf.Min(-deltaDegree, -rotateSpeed);
                tar.Rotate(Vector3.up, rotateAngle);
                return rotateAngle == -deltaDegree;
            }
            else if (deltaDegree > deviationDegree)
            {
                rotateAngle = Mathf.Min(deltaDegree, rotateSpeed);
                tar.Rotate(Vector3.up, rotateAngle);
                return rotateAngle == deltaDegree;
            }
            else
            {
                return true;
            }
        }

    }
}