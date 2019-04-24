using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>数值字典便捷方法 </summary>
public static class FloatDictionaryTools
{
    public static float GetValue<T1>(this Dictionary<T1, float> dic, T1 key)
    {
        if (!dic.ContainsKey(key))
            dic.Add(key, 0);
        return dic[key];
    }

    /// <summary>设置值</summary>
    /// <param name="makePositiveResult">强制结果为正</param>
    /// <returns>设置后的值</returns>
    public static float SetValue<T1>(this Dictionary<T1, float> dic, T1 key, float value, bool makePositiveResult)
    {
        if (value < 0 && makePositiveResult)
            Debug.LogError("此字典不可设置为负");
        else
            dic[key] = value;
        return dic[key];
    }

    /// <summary>值修改</summary>
    /// <param name="delta">增加量</param>
    /// <param name="makePositiveResult">强制结果为正</param>
    /// <returns>原始结果是否为正</returns>
    public static bool AddValue<T1>(this Dictionary<T1, float> dic, T1 key, float delta, bool makePositiveResult)
    {
        if (!dic.ContainsKey(key))
            dic.Add(key, 0);
        dic[key] += delta;
        if (dic[key] < 0 && makePositiveResult)
        {
            dic[key] = 0;
            return false;
        }
        return true;
    }

    public static void AddDic<T1>(this Dictionary<T1, float> ori, Dictionary<T1, float> tarDic, bool isPlus, bool makePositiveResult)
    {
        foreach (var item in tarDic.Keys)
        {
            if (isPlus)
                ori.AddValue(item, tarDic[item], makePositiveResult);
            else
                ori.AddValue(item, -tarDic[item], makePositiveResult);
        }
    }

    public static void CopyDic<T1>(this Dictionary<T1, float> ori, Dictionary<T1, float> tarDic, bool makePositiveResult)
    {
        foreach (var item in tarDic.Keys)
        {
            ori.SetValue(item, tarDic[item], makePositiveResult);
        }
    }

}
