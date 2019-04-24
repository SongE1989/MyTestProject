using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem
{
    public HashSet<IMessageMemeber> MemeberList = new HashSet<IMessageMemeber>();
    public Dictionary<string, HashSet<XXEntry>> RecordEntryDic = new Dictionary<string, HashSet<XXEntry>>();
    public HashSet<Action<string>> RecordListChangeCallbackList = new HashSet<Action<string>>();

    public void AddMemeber(IMessageMemeber tar)
    {
        if (MemeberList.Contains(tar))
            return;
        MemeberList.Add(tar);
        tar.mMessageSystem = this;
        RecordListChangeCallbackList.Add(tar.OnRecordEntryListChange);
    }

    public void RemoveMemeber(XXEntry tar)
    {
        if (!MemeberList.Contains(tar))
            return;
        MemeberList.Remove(tar);
        RecordListChangeCallbackList.Remove(tar.OnRecordEntryListChange);
    }

    public void AddRecordEntry(string recordType, XXEntry tar)
    {
        if (!RecordEntryDic.ContainsKey(recordType))
            RecordEntryDic.Add(recordType, new HashSet<XXEntry>());
        var tarSet = RecordEntryDic[recordType];
        if (!tarSet.Contains(tar))
        {
            tarSet.Add(tar);
            notifyRecordEntryListChange(recordType);
        }
    }

    public void RemoveRecordEntry(string recordType, XXEntry tar)
    {
        if (!RecordEntryDic.ContainsKey(recordType))
            return;
        var tarSet = RecordEntryDic[recordType];
        if(tarSet.Contains(tar))
        {
            tarSet.Remove(tar);
            notifyRecordEntryListChange(recordType);
        }
    }

    public void AddRecordEntryList(string recordType, IEnumerable<XXEntry> tarList)
    {
        if (!RecordEntryDic.ContainsKey(recordType))
            RecordEntryDic.Add(recordType, new HashSet<XXEntry>());
        var tarSet = RecordEntryDic[recordType];

        tarSet.UnionWith(tarList);
        //TODO 判断前后是否有变化
        notifyRecordEntryListChange(recordType);
    }

    public bool CheckRecordEntry(string recordType, XXEntry tar)
    {
        HashSet<XXEntry> tarList;
        if (!RecordEntryDic.TryGetValue(recordType, out tarList))
            return false;
        else
            return tarList.Contains(tar);
    }

    void notifyRecordEntryListChange(string recordType)
    {
        if (RecordListChangeCallbackList != null)
        {
            foreach (var callback in RecordListChangeCallbackList)
            {
                if (callback != null)
                    callback(recordType);
            }
        }
    }
}

public interface IMessageMemeber
{
    void OnRecordEntryListChange(string recordType);
    MessageSystem mMessageSystem { get; set; }
}


//public class MessageSystemManager
//{
//    static MessageSystemManager _instance;
//    public static MessageSystemManager Instance
//    {
//        get {
//            if (_instance == null)
//                _instance = new MessageSystemManager();
//            return _instance;
//        }
//    }
//}
