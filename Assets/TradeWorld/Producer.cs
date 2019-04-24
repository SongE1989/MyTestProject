using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Producer : MonoBehaviour
{
    public float MoneyChange = 0;
    public List<string> ItemNameList;
    public List<float> NumList;

    public Dictionary<string, float> GetChangeDic()
    {
        Dictionary<string, float> dic = new Dictionary<string, float>();
        int len = Mathf.Min(ItemNameList.Count, NumList.Count);
        for (int i = 0; i < len; i++)
        {
            var itemName = ItemNameList[i];
            var num = NumList[i];
            if(itemName != "" && itemName != null)
                dic.AddRep(itemName, num);
        }
        return dic;
    }
}
