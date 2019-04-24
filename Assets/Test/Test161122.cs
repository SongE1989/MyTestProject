using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System;
using System.Collections.Generic;

/// <summary>测试植物生长</summary>
public class Test161122 : MonoBehaviour
{

    void Start()
    {
        string jsonStr = File.ReadAllText(Application.streamingAssetsPath + "/normalPlantStateConfig.txt");
        Debug.Log(Application.streamingAssetsPath + "/normalPlantStateConfig.txt");
        JsonData configJd = JsonMapper.ToObject(jsonStr);
        Debug.Log(configJd.ToJson());
        StateManager sm = new StateManager().ReadJsonData(configJd) as StateManager;
        Debug.Log(sm.ToJsonData().ToJson().ToString());
    }

    void Update()
    {
        
    }



}

public class StateManager: IJsonData
{
    public List<State> StateList;
    public List<int> StateCircle;

    public JsonData ToJsonData()
    {
        JsonData jd = new JsonData();
        jd["StateList"] = StateList.ToJsonDataList();
        jd["StateCircle"] = JsonMapper.ToObject(JsonMapper.ToJson(StateCircle));
        return jd;
    }

    public IJsonData ReadJsonData(JsonData jd)
    {
        StateList = jd["StateList"].ToItemVOList<State>();
        StateCircle = JsonMapper.ToObject<List<int>>(jd["StateCircle"].ToJson());
        return this;
    }
}


public class State : IJsonData
{
    public string StateName;
    public int TimeToNext;
    public int EnergyToNext;

    public IJsonData ReadJsonData(JsonData jd)
    {
        StateName = jd.ReadString("StateName");
        TimeToNext = jd.ReadInt("TimeToNext");
        EnergyToNext = jd.ReadInt("EnergyToNext");
        return this;
    }

    public JsonData ToJsonData()
    {
        JsonData jd = new JsonData();
        jd["StateName"] = StateName;
        jd["TimeToNext"] = TimeToNext;
        jd["EnergyToNext"] = EnergyToNext;
        return jd;
    }
}
