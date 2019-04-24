using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace rtsNS
{

    public class TestRTS : MonoBehaviour
    {
        public static TestRTS Instance;

        //InfantryLine
        public Transform FrontLine;
        public Transform StartPoint;
        public Transform RegroupPoint;
        public Transform FrontLineLeftWing;
        public Transform FrontLineRigthWing;

        //TankLine
        public Transform StartPoint1;
        public Transform StartPoint2;
        public Transform StartPoint3;
        public Transform StartPoint4;
        public Transform EndPoint1;
        public Transform EndPoint2;
        public Transform EndPoint3;
        public Transform EndPoint4;
        Transform GetTankPoint(int index, bool isStart)
        {
            if(isStart)
            {
                if (index == 0)
                    return StartPoint1;
                else if (index == 1)
                    return StartPoint2;
                else if (index == 2)
                    return StartPoint3;
                else
                    return StartPoint4;
            }
            else
            {
                if (index == 0)
                    return EndPoint1;
                else if (index == 1)
                    return EndPoint2;
                else if (index == 2)
                    return EndPoint3;
                else
                    return EndPoint4;
            }
        }

        List<Transform> FrontLineTranList;
        List<Infantry> infantryList = new List<Infantry>();
        List<Tank> tankList = new List<Tank>();


        void Awake()
        {
            Instance = this;
            initGame();
        }

        void initGame()
        {
            FrontLineTranList = new List<Transform>();

            GameObject InfantryPrefab = Resources.Load("TestRTS/Infantry") as GameObject;
            for (int i = 0; i < 10; i++)
            {
                //前线兵位点
                Transform tran = new GameObject().transform;
                FrontLineTranList.Add(tran);
                tran.parent = FrontLine;
                Vector3 tarPos = Vector3.Lerp(FrontLineLeftWing.position, FrontLineRigthWing.position, i / 9f);
                tran.localPosition = tarPos;

                //初始士兵
                Infantry inf = Instantiate(InfantryPrefab).GetComponent<Infantry>();
                infantryList.Add(inf);
                inf.transform.position = tarPos;
                inf.MoveTo(new List<float>() { 0.03f},
                    new List<Transform>() { tran });
            }

            tankList = new List<Tank>();
            GameObject TankPrefab = Resources.Load("TestRTS/Tank") as GameObject;
            for (int i = 0; i < 4; i++)
            {
                Tank tank = Instantiate(TankPrefab).GetComponent<Tank>();
                tankList.Add(tank);
                Transform tarTran = GetTankPoint(i, false);
                tank.transform.position = tarTran.position;
                tank.ToPosition(tarTran);
            }



        }

        [ContextMenu("randomKill")]
        void randomKill()
        {
            int index = Random.Range(0, infantryList.Count);
            Infantry old = infantryList[index];
            old.Die();

            GameObject InfantryPrefab = Resources.Load("TestRTS/Infantry") as GameObject;
            Infantry inf = Instantiate(InfantryPrefab).GetComponent<Infantry>();

            infantryList[index] = inf;
            inf.MoveTo(new List<float>() { 0.06f, 0.03f },
                new List<Transform>() { RegroupPoint, FrontLineTranList[index]});
        }

        void randomKillTank()
        {
            int index = Random.Range(0, tankList.Count);
            Tank old = tankList[index];
            old.Die();

            GameObject TankPrefab = Resources.Load("TestRTS/Tank") as GameObject;
            Tank tank = Instantiate(TankPrefab).GetComponent<Tank>();
            tank.transform.position = GetTankPoint(index, true).position;
            tankList[index] = tank;
            tank.ToPosition(GetTankPoint(index, false));
        }

        [ContextMenu("testMove")]
        void testMove()
        {
            GameObject InfantryPrefab = Resources.Load("TestRTS/Infantry") as GameObject;
            Infantry inf = Instantiate(InfantryPrefab).GetComponent<Infantry>();
            int i = 3;
            Vector3 tarPos = Vector3.Lerp(FrontLineLeftWing.position, FrontLineRigthWing.position, i / 9f);
            inf.MoveTo(new List<float>() { 0.03f },
                new List<Transform>() { RegroupPoint, FrontLineLeftWing},
                new List<Vector3>() { Vector3.zero, tarPos - FrontLineLeftWing.position }
                );
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                randomKill();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                randomKillTank();
            }
        }

    }
}