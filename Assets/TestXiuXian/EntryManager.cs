using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntryManager
{
    static EntryManager _instance;
    public static EntryManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EntryManager();
            return _instance;
        }
    }

    public Dictionary<string, HashSet<XXEntry>> mEntryDic = new Dictionary<string, HashSet<XXEntry>>();


    public T CreateActionEntry<T>(string prefabName) where T : XXEntry
    {
        GameObject prefab = Resources.Load("XiuXian/" + prefabName) as GameObject;
        var go = GameObject.Instantiate(prefab);
        //TODO 此处需要配置
        if(prefabName == "Grass")
        {
            go.transform.SetParent(GameXiuXian.Instance.EnvirLayer);
        }
        else
        {
            go.transform.SetParent(GameXiuXian.Instance.MoveLayer);
        }
        var comp = go.GetComponent<T>();
        comp.ReUse();
        if (!mEntryDic.ContainsKey(prefabName))
            mEntryDic.Add(prefabName, new HashSet<XXEntry>());
        mEntryDic[prefabName].Add(comp);
        return comp;
    }

    /// <summary>按名称类型获取实体</summary>
    public HashSet<XXEntry> GetEntrySetByName(string entryName)
    {
        HashSet<XXEntry> entrySet = null;
        mEntryDic.TryGetValue(entryName, out entrySet);
        return entrySet;
    }
}