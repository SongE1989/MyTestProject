using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.Text;

namespace testTownNS
{
    public class TestTown171015 : MonoBehaviour
    {
        public static TestTown171015 Instance;

        public List<Group> TheGroupList = new List<Group>();
        // Use this for initialization

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Farmer f1 = new Farmer();
            f1.Population = 1;
            f1.Items.Add("Wheat", 40f);
            f1.Items.Add("Meat", 40f);
            TheGroupList.Add(f1);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                TimeManager.Instance.UpdateDate(TheGroupList);
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                TheGroupList.ForEach(g => {
                    Debug.Log(g.ToJsonData().ToJson());
                });
            }
        }



    }

    public class TimeManager
    {
        static TimeManager _inst;
        public static TimeManager Instance
        {
            get
            {
                if (_inst == null)
                    _inst = new TimeManager();
                return _inst;
            }
        }

        #region 时间

        public int CurYear = 0;
        public int CurMonth = 2;
        public int CurDay = 1;
        public void ToNextDate(ref int year, ref int month, ref int day)
        {
            day++;
            if (day > 30)
            {
                day = 1;
                month++;
                if (month > 12)
                    year++;
            }
        }

        /// <summary>当前月份是否产出粮食</summary>
        public bool IsCurMonthGrownFood()
        {
            return CurMonth >= 2 && CurMonth <= 10;
        }

        #endregion

        public void UpdateDate(List<Group> groupList)
        {
            Debug.Log("日期: " + CurYear + "年 " + CurMonth + "月 " + CurDay + "日");
            foreach (var group in groupList)
            {
                if (CurDay == 1)
                    group.OnMonthStart();
                group.OnDayStart();

            }

            foreach (var group in groupList)
            {
                if (CurDay == 30)
                    group.OnMonthEnd();
                group.OnDayEnd();
            }

            ToNextDate(ref CurYear, ref CurMonth, ref CurDay);
        }
    }
    


    public class Group : IJsonData
    {
        [LitJson]
        public ItemContainer2 Items = new ItemContainer2();

        [LitJson]
        public int Population = 1;
        //[LitJson]
        //public bool IsDead = false;

        public Group()
        {
        }

        #region TimePass
        public virtual void OnDayStart()
        {
        }
        public virtual void OnDayEnd()
        {
            UseFood();
        }

        public virtual void OnMonthStart()
        {

        }
        public virtual void OnMonthEnd()
        {

        }

        #endregion

        #region Food Behavior
        //public float MaxEngery = 1f;

        //public virtual void EnergyLost()
        //{
        //    float lackNum = 0;
        //    bool isEnouph = Items.Add("Engery", -1f, true, out lackNum);
        //    if(!isEnouph)
        //    {
        //        Starve(lackNum);
        //    }
        //}

        //public virtual void Starve(float lackNum)
        //{
        //    Items.Add("HP", -lackNum * 10f);
        //    CheckHP();
        //}

        public virtual void UseFood()
        {
            float foodPerPerson = 1f;
            var needNum = Population * foodPerPerson;
            float consumeNum = needNum;
            var wheatNum = Items["Wheat"];
            var meatNum = Items["Meat"];
            Dictionary<string, float> comsumeDic;
            var stillNeed = Comsume(needNum, new List<string>() { "Wheat", "Meat" }, Items, out comsumeDic);

            StringBuilder consumInfo = new StringBuilder("\r\n预计消耗食物: " + consumeNum + ", 实际情况:");
            foreach (var pair in comsumeDic)
                consumInfo.Append(pair.Key+":"+pair.Value+"份");
            Info.Append(consumInfo);

            Info.AppendLine("预计消耗食物: "+ consumeNum);

            if(stillNeed > 0)
            {
                float loseRate = (stillNeed / needNum) * 0.1f;
                int loseNum = Mathf.CeilToInt(Population * loseRate);
                Population -= loseNum;
                Info.AppendLine("缺少" + stillNeed + "份食物, 有"+loseNum+"人口流失");
            }
        }

        public float Comsume(float needNum, List<string> nameList, ItemContainer2 itemDic, out Dictionary<string, float> consumeDic)
        {
            nameList.Sort((a, b) => {
                if (itemDic[a] > itemDic[b])
                    return 1;
                else if (itemDic[a] < itemDic[b])
                    return -1;
                else
                    return 0;
            });
            float stillNeed = needNum;
            float curThrehold =0;
            consumeDic = new Dictionary<string, float>();
            for (int iList = 0, nList = nameList.Count; iList < nList; iList++)
            {
                var itemName = nameList[iList];
                float delta = itemDic[itemName] - curThrehold;
                float leftNames = nList - iList;
                float affordNum = delta * leftNames;

                if(affordNum >= stillNeed)
                {
                    float costDelta = stillNeed / leftNames;
                    curThrehold = curThrehold + costDelta;

                    itemDic.Add(itemName, -curThrehold);
                    stillNeed = 0f;
                }
                else
                {
                    curThrehold = itemDic[itemName];

                    itemDic[itemName] = 0;
                    stillNeed -= affordNum;
                }
                consumeDic.Add(itemName, curThrehold);
            }
            return stillNeed;
        }


        #endregion

        //public float MaxHP = 100f;
        //public void CheckHP()
        //{
        //    if(Items["HP"] <=0)
        //    {
        //        Die();
        //    }
        //}
        //public virtual void Die()
        //{
        //    IsDead = true;
        //}

        public StringBuilder Info = new StringBuilder();

        public IJsonData ReadJsonData(JsonData jd)
        {
            return this.AutoReadJsonData(jd);

        }

        public JsonData ToJsonData()
        {
            return this.AutoToJsonData();
        }
        
    }

    public class NeedManager
    {
        //1.食物
        //2.衣服
        //3.居住
        //4.日常
    }


    /// <summary>最小单元, 以最低效率获取各项资源, 并进行简单加工</summary>
    public class Family : Group
    {
        public override void OnDayEnd()
        {
            base.OnDayEnd();
            searchFood();
        }

        void searchFood()
        {
            string[] foodNameArr = new string[] { "Mushroom", "Fruit" };
            int foodKindIndex = Random.Range(1, foodNameArr.Length);
            float foodNum = Random.Range(1f, 1.5f);
            Items.Add(foodNameArr[foodKindIndex], Population * foodNum);
        }
    }

    public class Farmer : Group
    {
        public override void OnMonthEnd()
        {
            base.OnMonthEnd();
            if (TimeManager.Instance.IsCurMonthGrownFood())
            {
                Items.Add("Wheat", Population * 1.5f);
                Info.AppendLine("月末生产 " + "Wheat " + Population * 1.5f + "份");
            }
        }
    }

    public class Miner : Group
    {
        public override void OnDayEnd()
        {
            base.OnDayEnd();
            Items.Add("IronOre", Population * 1f);
        }
    }

    public class Hunter : Group
    {
        public override void OnDayEnd()
        {
            base.OnDayEnd();
            Items.Add("Meat", Random.Range(0.3f, 2f));
            Items.Add("Fur", Random.Range(0.5f, 1f));
        }
    }

    public class BlackSmith : Group
    {
        public override void OnDayEnd()
        {
            base.OnDayEnd();
            float lackNum;
            Items.Add("IronOre", -Population, true, out lackNum);
            float uesdNum = Population - lackNum;
            Items.Add("Tool", uesdNum * 0.2f);
        }
    }

    public class GroupBehavior
    {

    }

    public class Field
    {
        public bool IsReclaimed;
        public Seed CurSeed;
    }

    public class Seed
    {
        public int DayToGrow;
        public string ResultName;
        public float ResultNum;
        public float IrragationBonus;
        public HashSet<int> SowingMonths;
    }

    public class Farming : GroupBehavior
    {
        public Field CurField;
        public Seed CurSeed;

        //1. select a field
        //2. field reclaimed ? go on : 2.1reclaim field
        //3. select seed
        //4. sowing
        //5. irrigate
        //6. harvest

        public void Process()
        {
            
        }

    }


    //public class Behavior

    public class ItemContainer2 : IJsonData
    {
        [LitJson]
        public Dictionary<string, float> ItemDic = new Dictionary<string, float>();

        public float this[string key]
        {
            get
            {
                if (!ItemDic.ContainsKey(key))
                    ItemDic[key] = 0;
                return ItemDic[key];
            }
            set
            {
                ItemDic[key] = value;
            }
        }

        public bool Add(string itemName, float itemNum)
        {
            float lackNum = 0;
            return Add(itemName, itemNum, true, out lackNum);
        }

        public bool Add(string itemName, float itemNum, bool preventNagetive)
        {
            float lackNum = 0;
            return Add(itemName, itemNum, preventNagetive, out lackNum);
        }

        public bool Add(string itemName, float itemNum, bool preventNagetive, out float lackNum)
        {
            if (!ItemDic.ContainsKey(itemName))
                ItemDic[itemName] = 0;

            var oldValue = ItemDic[itemName];
            ItemDic[itemName] += itemNum;

            if (ItemDic[itemName] < 0)
            {
                lackNum = -ItemDic[itemName];
                if (preventNagetive)
                    ItemDic[itemName] = 0;
                return false;
            }
            else
            {
                lackNum = 0;
                return true;
            }
        }

        public IJsonData ReadJsonData(JsonData jd)
        {
            return this.AutoReadJsonData(jd);

        }

        public JsonData ToJsonData()
        {
            return this.AutoToJsonData();
        }

    }


}
