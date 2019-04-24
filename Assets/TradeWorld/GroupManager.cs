using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System;
using System.Text;

namespace testTownNS
{

    public class GroupManager
    {
        static GroupManager _instance;
        public static GroupManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GroupManager();
                    _instance.init();
                }
                return _instance;
            }
        }

        Dictionary<string, GroupClass> m_kGroupTypeDic = new Dictionary<string, GroupClass>();

        void init()
        {
            string jdPath = Application.streamingAssetsPath + "/testTown/GroupList.jd";
            JsonData jd = JsonUtils.ReadJsonFile(jdPath);
            Debug.Log(jd.ToJson());
            List<GroupClass> groupTypeList = jd["data"].ToItemVOList<GroupClass>();
            Debug.Log(groupTypeList.ToJsonDataList().ToJson());
            for (int iList = 0, nList = groupTypeList.Count; iList < nList; iList++)
            {
                var gt = groupTypeList[iList] ;
                m_kGroupTypeDic.Add(gt.GroupName, gt);
            }
        }

        public void InitGroupType(GroupClass gt, string groupName)
        {
            if(m_kGroupTypeDic.ContainsKey(groupName))
            {
                 gt.ReadJsonData(m_kGroupTypeDic[groupName].ToJsonData());
            }
            else
            {
                Debug.LogError("cant find groupName: " + groupName);
            }
        }
    }

    //public class ItemConversion
    //{

    //}

    public class ItemEntry:IJsonData
    {
        [LitJson]
        public string ItemName;
        [LitJson]
        public float Num;

        public IJsonData ReadJsonData(JsonData jd)
        {
            return this.AutoReadJsonData(jd);
        }

        public JsonData ToJsonData()
        {
            return this.AutoToJsonData();
        }
    }

    public class GroupClass : IJsonData
    {
        [LitJson]
        public string GroupName;
        [LitJson]
        public List<ItemEntry> Consume = new List<ItemEntry>();
        [LitJson]
        public List<ItemEntry> Produce = new List<ItemEntry>();
        [LitJson]
        public List<ItemEntry> Keep = new List<ItemEntry>();

        public IJsonData ReadJsonData(JsonData jd)
        {
            IJsonData res = this.AutoReadJsonData(jd);
            Consume = jd["Consume"].ToItemVOList<ItemEntry>();
            Produce = jd["Produce"].ToItemVOList<ItemEntry>();
            Keep = jd["Keep"].ToItemVOList<ItemEntry>();
            return res;
        }

        public JsonData ToJsonData()
        {
            JsonData jd = this.AutoToJsonData();
            jd["Consume"] = Consume.ToJsonDataList();
            jd["Produce"] = Produce.ToJsonDataList();
            jd["Keep"] = Keep.ToJsonDataList();
            return jd;
        }
    }

    public class GroupEntry : IJsonData , IItemOwnner
    {
        [LitJson]
        public string Name;
        [LitJson("Type")]
        public GroupClass m_kType = new GroupClass();
        [LitJson("Items")]
        public ItemContainer m_kItems = new ItemContainer();

        public Dictionary<string, float> m_kDemandSupplyDic = new Dictionary<string, float>();

        public ItemContainer Items
        {
            get
            {
                return m_kItems;
            }
        }

        public void Next()
        {
            for (int iList = 0, nList = m_kType.Produce.Count; iList < nList; iList++)
            {
                var produce = m_kType.Produce[iList] ;
                m_kItems.ChangeItem(produce.ItemName, produce.Num);
            }
            for (int i_consume = 0, n_consume = m_kType.Consume.Count; i_consume < n_consume; i_consume++)
            {
                var consume = m_kType.Consume[i_consume];
                m_kItems.ChangeItem(consume.ItemName, -consume.Num);
            }
        }

        //TODO 根据DemandManager, 以及价格, 对所有物品综合考虑来管理需求供给
        //TODO 需确保供给数<=持有数, 并尽量保证需求数*价格<=金钱数
        //TODO 考虑传入市场价格
        /// <summary>返回物品向市场需求/供给情况</summary>
        public Dictionary<string, float> RefreshDemandSupplyDic(Dictionary<string, float> marketPriceDic)
        {
            m_kDemandSupplyDic.Clear();
            for (int i_keep = 0, n_keep = m_kType.Keep.Count; i_keep < n_keep; i_keep++)
            {
                var keep = m_kType.Keep[i_keep];
                if (keep == null)
                    m_kDemandSupplyDic.Add(keep.ItemName, m_kItems[keep.ItemName]);
                else
                {
                    var delta = m_kItems[keep.ItemName] - keep.Num;
                    float basePrice = ItemManager.Instance.ItemClassDic[keep.ItemName].BasePrice;
                    float marketPrice = basePrice;
                    marketPriceDic.TryGetValue(keep.ItemName, out marketPrice);
                    delta = GetSupplyDemandBiasByPrice(keep.ItemName, basePrice, marketPrice) * delta;
                    m_kDemandSupplyDic.Add(keep.ItemName, delta);
                }
            }
            return m_kDemandSupplyDic;
        }

        /// <summary>根据市场价格, 调整供需值, 返回一个系数使价高时降低需求提供供给, 价低时降低供给提高需求</summary>
        public float GetSupplyDemandBiasByPrice(string itemName, float basePrice, float marketPrice)
        {

            return 1;//TODO 暂时屏蔽
        }

        public string GetInfo(string prefix = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(prefix + "GroupEntry:" +Name);
            sb.AppendLine(prefix + "Items:");
            sb.Append(Items.GetInfo(prefix+"--"));
            sb.AppendLine(prefix + "DemandSupply:");
            sb.AppendLine(prefix + "--" + m_kDemandSupplyDic.ForeachToString("\r\n"+ prefix + "--"));
            return sb.ToString();
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