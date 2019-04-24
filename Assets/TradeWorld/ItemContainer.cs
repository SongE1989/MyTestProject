using UnityEngine;
using System.Collections;
using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

public class ItemContainer : IJsonData
{
    [LitJson]
    public float Money;
    [LitJson]
    public Dictionary<string, float> ItemDic = new Dictionary<string, float>();

    public void Reset()
    {
        Money = 0;
        ItemDic.Clear();
    }

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
    public void ChangeMoney(float num)
    {
        Money += num;
    }

    public void Add(string itemName, float itemNum)
    {
        ChangeItem(itemName, itemNum);
    }
    public void ChangeItem(string itemName, float itemNum)
    {
        if (ItemDic.ContainsKey(itemName))
            ItemDic[itemName] += itemNum;
        else
            ItemDic[itemName] = itemNum;
    }

    public IJsonData ReadJsonData(JsonData jd)
    {
        return this.AutoReadJsonData(jd);

    }

    public JsonData ToJsonData()
    {
        return this.AutoToJsonData();
    }

    public string GetInfo(string prefix = "")
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(prefix+"Money:").Append(Money).AppendLine();
        foreach (var pair in ItemDic)
        {
            if(pair.Value != 0)
                sb.Append(prefix).Append(pair.Key).Append(":").Append(pair.Value).AppendLine();
        }
        return sb.ToString();
    }
}

public interface IItemOwnner
{
    ItemContainer Items { get; }
}
