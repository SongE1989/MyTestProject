using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;
using System;

public class ItemManager
{
    static ItemManager _instance;
    public static ItemManager Instance {
        get {
            if(_instance == null)
            {
                _instance = new ItemManager();
                _instance.init();
            }
            return _instance;
        }
    }
    public Dictionary<string, ItemClass> ItemClassDic = new Dictionary<string, ItemClass>();
    public List<ItemClass> AllItemList = new List<ItemClass>();
    void init()
    {
        string jdPath = Application.streamingAssetsPath+ "/testTown/ItemList.jd";
        JsonData jd = JsonUtils.ReadJsonFile(jdPath);
        Debug.Log(jd.ToJson());
        List<ItemClass> itemList = jd["data"].ToItemVOList<ItemClass>();

        ItemClassDic.Clear();
        AllItemList.Clear();

        for (int iList = 0, nList = itemList.Count; iList < nList; iList++)
        {
            var itemClass = itemList[iList] ;
            ItemClassDic.Add(itemClass.ItemName, itemClass);
            AllItemList.Add(itemClass);
        }
    }

    public class ItemClass :IJsonData
    {
        [LitJson]
        public string ItemName;
        public List<string> ItemTypeList;
        [LitJson]
        public int BasePrice;

        public JsonData ToJsonData()
        {
            JsonData jd = this.AutoToJsonData();
            jd["ItemType"] = JsonMapper.ToJson(ItemTypeList);
            return jd;
        }

        public IJsonData ReadJsonData(JsonData jd)
        {
            IJsonData tar = this.AutoReadJsonData(jd);
            ItemTypeList = JsonMapper.ToObject<List<string>>(jd["ItemType"].ToJson());
            return tar;
        }
    }

}
