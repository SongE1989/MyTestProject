using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RimWorldNS
{
    public class Animal : Creature
    {
        public HealthModule TheHealthModule;
        private AnimalState curState = AnimalState.normal;
        public AnimalState CurState
        {
            get
            {
                return curState;
            }

            set
            {
                curState = value;
                TheHealthModule.TheState = curState;
            }
        }

        CountDown testAI_CD;// 测试用, 用来存放测试用AI逻辑
        CountDown healthCD;

        void Awake()
        {
            init();
        }

        void Start()
        {
            CreateModel();
        }

        void Update()
        {
            refreshViewPos();//View层逻辑, 可以放在自身Update里
        }

        protected void init()
        {
            name = "Animal";
            ModelUrl = "RimWorldPrefabModel/Animal";

            TheHealthModule = new HealthModule(
                1000, 0, 800,
                1000, 0, 800,
                1000, 0, 800);

            curState = AnimalState.normal;

            testAI_CD = new CountDown(360, testAI);
            healthCD = new CountDown(1, TheHealthModule.RefreshHealth);

        }

        override public void OnUpdate()
        {
            //base.OnUpdate();
            testAI_CD.Count();
            refreshPos();
            healthCD.Count();
        }

        void testAI()
        {
            Debug.Log("testAI");
            int posX = UnityEngine.Random.Range(0, SceneManager.Instance.SceneX);
            int posZ = UnityEngine.Random.Range(0, SceneManager.Instance.SceneZ);
            MoveTo(new Vector3(posX, 0, posZ));
        }

        #region Move

        public DisplayObj MoveTarget;
        public float MoveSpeed = 0.1f;

        bool isMove;
        public bool IsMove
        {
            get
            {
                return isMove;
            }

            set
            {
                isMove = value;
                if (isMove)
                    CurState |= AnimalState.move;
                else
                    CurState &= ~AnimalState.move;
            }
        }


        public void MoveTo(DisplayObj tar)
        {
            MoveTarget = tar;
        }
        public void MoveTo(Vector3 tarPos)
        {
            if (MoveTarget != null)
                SceneManager.Instance.RemoveTarget(MoveTarget);
            MoveTarget = SceneManager.Instance.CreateVirtualMoveTarget(tarPos);
        }

        void refreshPos()
        {
            if (MoveTarget != null)
            {
                Vector3 tarPos = MoveTarget.transform.position;
                Vector3 dir = tarPos - Position;
                float dis = Vector3.Distance(tarPos, Position);
                if (dis <= MoveSpeed)
                {
                    Position = tarPos;
                    IsMove = false;
                }
                else
                {
                    Vector3 moveDis = dir.normalized.GetScaledVector(MoveSpeed);
                    Position += moveDis;
                    IsMove = true;
                }
            }
        }

        void refreshViewPos()
        {
            Vector3 dir = Position - transform.position;
            float dis = Vector3.Distance(Position, transform.position);
            if (dis <= 0.2f)
            {
                transform.position = Position;
            }
            else
            {
                transform.position += dir.normalized * dis * 0.75f;
            }
        }

        #endregion


    }
}