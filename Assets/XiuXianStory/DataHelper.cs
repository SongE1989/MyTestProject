using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiuXianStory
{
    public static class DataHelper
    {
        public static ItemResult SingleItemChange(string itemName, int itemNum)
        {
            return new ItemResult()
            {
                ItemDic = new Dictionary<Item, int>()
                {
                    { DataManager.Instance.ItemDic[itemName], itemNum},
                }
            };
        }

        public static List<Result> SingleItemChangeList(string itemName, int itemNum)
        {
            return new List<Result>() { SingleItemChange(itemName, itemNum)};
        }

        public static bool CheckDesireIsCondition()
        {
            return false;
        }
        /// <summary>获取随机寿命(月)加成</summary>
        public static int GetRandomLifeTimeDelta(int stage)
        {
            switch (stage)
            {
                case 0://基础寿命
                    return 12 * Random.Range(50, 100);
                case 1://筑基寿命加成
                    return 12 * Random.Range(150, 200);
                case 2://结丹寿命加成
                    return 12 * Random.Range(500, 700);
                case 3://元婴寿命加成
                    return 12 * Random.Range(1000, 2000);
                default:
                    return 0;
            }
        }

        /// <summary>获取随机年龄-寿命(月)</summary>
        public static void GetRandomAgeAndLifeTime(int stage, out int age, out int lifeTime)
        {
            age = 0;
            lifeTime = 0;

            int curStage = stage;
            while(curStage >= 0)
            {
                var curStageDelta = GetRandomLifeTimeDelta(curStage);
                lifeTime += curStageDelta;
                if (curStage == stage)
                    age += Random.Range(0, curStageDelta);
                else
                    age += curStageDelta;
                curStage--;
            }
        }

        public static string GetChangeString(float value)
        {
            if (value > 0)
                return "+" + value;
            else if (value == 0)
                return "无变化";
            else
                return "-" + value;

        }
    }

    public static class DicHelper
    {
        public static string GetBagInfo(Dictionary<Item, int> dic, bool showSymbol = false)
        {
            string info = "";
            foreach (var pair in dic)
            {
                string valueStr;
                if (showSymbol && pair.Value > 0)
                    valueStr = "+" + pair.Value;
                else
                    valueStr = pair.Value.ToString();
                info += pair.Key.NameID + "  " + valueStr + "\r\n";
            }
            return info;
        }

        public static void AddNameIDItem<T>(this Dictionary<string, T> dic, T item) where T : INameID
        {
            dic.Add(item.NameID, item);
        }

        public static void AddNameIDItemList<T>(this Dictionary<string, T> dic, List<T> list) where T : INameID
        {
            for (int i = 0; i < list.Count; i++)
            {
                dic.AddNameIDItem(list[i]);
            }
        }


        public static void AddValue<K>(this Dictionary<K, int> dic, K key, int value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, 0);
            dic[key] += value;
        }
        public static void AddValue<K>(this Dictionary<K, float> dic, K key, float value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, 0);
            dic[key] += value;
        }
    }
}
