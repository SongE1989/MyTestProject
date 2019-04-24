using UnityEngine;
using System.Collections;
using System;
using LitJson;

public class Town : MonoBehaviour, IItemOwnner,IJsonData
{
    ItemContainer items = new ItemContainer();
    public ItemContainer Items
    {
        get
        {
            return items;
        }
    }

    public DemandManager Demands;
    public void Init()
    {
        Demands = new DemandManager();
        Demands.CheckDemandFunc = checkDemand;
    }

    bool checkDemand(string demandName, int level)
    {
        return false;
    }

    //TODO 建立需求管理器 并以此驱动城市发展
    //1.需求管理器
    //2.每回合根据需求决策行动
    //3.模型发展过程

    //根据[需求] 做出[选择], 包括建造, 与市场定价(范围定价, 多订单)

    [ContextMenu("UpdateLogic")]
    public void UpdateLogic()
    {
        //Producer p = GetComponent<Producer>();
        //items.ItemDic.AddDic(p.GetChangeDic(), true, false);
        //items.ChangeMoney(p.MoneyChange);

        //遍历并检查当前需求
        //遍历并检查当前可执行行为
    }



    #region JsonData

    public IJsonData ReadJsonData(JsonData jd)
    {
        items.ReadJsonData(jd.ReadJsonData("Items"));
        return this.AutoReadJsonData(jd);
    }

    public JsonData ToJsonData()
    {
        JsonData jd = this.AutoToJsonData();
        jd["Items"] = items.ToJsonData();
        return jd;
    }
    #endregion
}

