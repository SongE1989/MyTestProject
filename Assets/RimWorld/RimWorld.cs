using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RimWorldNS
{
    [Flags]
    public enum AnimalState
    {
        errorState = 0,
        normal = 1<<0,
        sleep = 1<<1,
        move = 1<<2,
        active = 1<<3
    };


    public class RimWorld : MonoBehaviour
    {
        public Dictionary<string, GameObject> dic;
        public List<GameObject> list;

        void Start()
        {

            SceneManager.Instance.Init();
        }

        void Update()
        {
            SceneManager.Instance.Update();
        }
    }

    #region Managers

    public class SceneManager
    {
        private static SceneManager instance;
        public static SceneManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SceneManager();
                return instance;
            }
        }

        public int SceneX = 20;
        public int SceneZ = 20;


        #region Update & Init

        public void Init()
        {
            BlockFolder = new GameObject().transform;
            BlockFolder.name = "BlockFolder";
            EnvironmentFolder = new GameObject().transform;
            EnvironmentFolder.name = "EnvironmentFolder";
            CreatureFolder = new GameObject().transform;
            CreatureFolder.name = "CreatureFolder";
            TargetFolder = new GameObject().transform;
            TargetFolder.name = "TargetFolder";

            forAllBlock(pos =>
            {
                Block b = AddBlock<Block>(pos);
                TheEnvironmentDic.Add(pos, null);
            });

            Animal ani = AddCreature<Animal>();//TEST

            blockRefreshCD = new CountDown(30, onCountDown);
            animalRefreshCD = new CountDown(1, onAnimalCD);

        }

        public void Update()
        {
            blockRefreshCD.Count();
            animalRefreshCD.Count();
        }

        CountDown blockRefreshCD;
        CountDown animalRefreshCD;
        void onCountDown()
        {
            forAllBlock(pos =>
            {
                Block block = TheBlockDic[pos];
                Environment env = TheEnvironmentDic[pos];
                if (env == null)
                {
                    int dice = UnityEngine.Random.Range(0, 100);
                    if (dice > 95)
                    {
                        AddEnvironment<Grass>(pos);
                    }
                }

            });
        }

        void onAnimalCD()
        {
            for (int iCrea = 0, nCrea = TheCreList.Count; iCrea < nCrea; iCrea++)
            {
                Creature crea = TheCreList[iCrea];
                crea.OnUpdate();
            }
        }
        #endregion

        public void forAllBlock(Action<Vector3> callback)
        {
            for (int iz = 0; iz < SceneZ; iz++)
            {
                for (int ix = 0, nx = SceneX; ix < nx; ix++)
                {
                    callback(new Vector3(ix, 0, iz));
                }
            }
        }

        #region Block
        public Transform BlockFolder;
        public Dictionary<Vector3, Block> TheBlockDic = new Dictionary<Vector3, Block>();
        public T AddBlock<T>(Vector3 pos) where T : Block
        {
            GameObject go = new GameObject();
            T t = go.AddComponent<T>();
            t.transform.parent = BlockFolder;
            t.transform.position = new Vector3(pos.x, 0, pos.z);
            TheBlockDic.AddRep(pos, t);
            return t;
        }
        #endregion

        #region Environment
        public Transform EnvironmentFolder;
        public Dictionary<Vector3, Environment> TheEnvironmentDic = new Dictionary<Vector3, Environment>();
        public T AddEnvironment<T>(Vector3 pos) where T : Environment
        {
            GameObject go = new GameObject();
            T t = go.AddComponent<T>();
            t.transform.parent = EnvironmentFolder;
            t.transform.position = new Vector3(pos.x, 0, pos.z);
            TheEnvironmentDic.AddRep(pos, t);
            return t;
        }
        #endregion

        #region Creature
        public Transform CreatureFolder;
        public List<Creature> TheCreList = new List<Creature>();
        public T AddCreature<T>() where T : Creature
        {
            GameObject go = new GameObject();
            T t = go.AddComponent<T>();
            t.transform.parent = CreatureFolder;
            TheCreList.Add(t);
            return t;
        }
        #endregion

        #region otheGameObject

        Transform TargetFolder;
        public VirtualMoveTarget CreateVirtualMoveTarget(Vector3 tarPos)
        {
            VirtualMoveTarget moveTarget = new GameObject().AddComponent<VirtualMoveTarget>();

            moveTarget.name = "moveTarget";
            moveTarget.transform.parent = TargetFolder;
            moveTarget.transform.position = tarPos;
            return moveTarget;

        }

        public void RemoveTarget(DisplayObj tar)
        {
            if(tar is VirtualMoveTarget)
                UnityEngine.Object.Destroy(tar.gameObject);
        }

        #endregion
    }


    #endregion

    public class VirtualMoveTarget : DisplayObj
    {

    }

    public class DisplayObj : MonoBehaviour
    {
        public string ModelUrl;
        public GameObject ModelContent;
        public void CreateModel()
        {
            if (ModelContent == null)
            {
                GameObject prefab = Resources.Load<GameObject>(ModelUrl);
                if (prefab != null)
                {
                    ModelContent = Instantiate(prefab);
                    ModelContent.transform.parent = transform;
                    ModelContent.transform.localPosition = Vector3.zero;
                }
            }
        }

        /// <summary>游戏逻辑 由SceneManager统一调用</summary>
        public virtual void OnUpdate()
        {
        }
    }

    #region Block

    public class Block : DisplayObj
    {
        void Start()
        {
            name = "block";
            ModelUrl = "RimWorldPrefabModel/Block";
            CreateModel();
        }
    }

    #endregion

    #region Environment
    public class Environment : DisplayObj
    {
    }

    public class Grass : Environment
    {
        void Start()
        {
            name = "grass";
            ModelUrl = "RimWorldPrefabModel/Grass";
            CreateModel();
        }
    }

    #endregion

    #region Createture

    public class Creature : DisplayObj
    {
        public Vector3 Position { get; set; }
    }

    #endregion

    #region Utils

    public class CountDown
    {
        public int Max;
        public int Left;
        public Action Callback;
        public CountDown(int max, Action callback)
        {
            Max = max;
            Left = Max;
            Callback = callback;
        }
        public void Count()
        {
            Left--;
            if (Left == 0)
            {
                Callback();
                Left = Max;
            }
        }

    }
    #endregion



}


