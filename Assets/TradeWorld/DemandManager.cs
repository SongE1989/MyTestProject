using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class DemandManager
{
    List<List<Demand>> AllDemandList = new List<List<Demand>>();

    public DemandManager()
    {
        _init();
    }

    public Func<string, int, bool> CheckDemandFunc;
    public bool CheckDemand(string demandName, int level)
    {
        if (CheckDemandFunc == null)
            return false;
        else
            return CheckDemandFunc(demandName, level);
    }

    void _init()
    {
        JsonData saveJD = JsonUtils.ReadJsonFile(Application.streamingAssetsPath + "/testTown/DemandList.jd");
        JsonData levelArrJD = saveJD["data"]["Levels"];
        AllDemandList.Clear();

        for (int iLevel = 0; iLevel < levelArrJD.Count; iLevel++)
        {
            JsonData levelJD = levelArrJD[iLevel];
            JsonData DemandArrJD = levelJD["Demands"];
            List<Demand> demandList = new List<Demand>();
            for (int iDemand = 0; iDemand < DemandArrJD.Count; iDemand++)
            {
                JsonData demandJD = DemandArrJD[iDemand];
                demandList.Add(new Demand(demandJD));
            }
            AllDemandList.Add(demandList);
        }
        Debug.Log(AllDemandList);

        //AllDemandList = new List<List<Demand>>() {
        //    new List<Demand>() {
        //        new Demand("Food", 1),
        //        new Demand("Wear", 1),
        //        new Demand("House", 1)
        //    },
        //    new List<Demand>() {
        //        new Demand("Food", 2),
        //        new Demand("Wear", 2),
        //        new Demand("House", 2),
        //        new Demand("Safty", 1),
        //        new Demand("Enjoyment", 1),
        //        new Demand("Wealth", 1)
        //    },
        //    new List<Demand>() {
        //        new Demand("Food", 3),
        //        new Demand("Wear", 3),
        //        new Demand("House", 3),
        //        new Demand("Safty", 2),
        //        new Demand("Enjoyment", 2),
        //        new Demand("Wealth", 2)
        //    },
        //    new List<Demand>() {
        //        new Demand("Food", 4),
        //        new Demand("Wear", 4),
        //        new Demand("House", 4),
        //        new Demand("Safty", 3),
        //        new Demand("Enjoyment", 3),
        //        new Demand("Wealth", 3)
        //    }
        //};
    }
}

public class Demand:IJsonData
{
    [LitJson]
    public string DemandName;
    [LitJson]
    public int DemandLevel;
    [LitJson]
    public int Weigth;

    public Demand(JsonData jd)
    {
        ReadJsonData(jd);
    }

    public Demand(string demandName, int demandLevel, int weight)
    {
        DemandName = demandName;
        DemandLevel = demandLevel;
        Weigth = weight;
    }

    public JsonData ToJsonData()
    {
        return this.AutoToJsonData();
    }

    public IJsonData ReadJsonData(JsonData jd)
    {
        return this.AutoReadJsonData(jd);
    }
}
