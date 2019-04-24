using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System;

namespace TownNS
{

    public class TestTown : MonoBehaviour
    {
        Town t1;

        void Start()
        {
            t1 = Town.CreateNewTown();
        }

        void Update()
        {
            t1.OnDayPass();
        }
    }

    /*
        在满足各项需求的条件下, 人口会逐渐增加直到到达当前城市等级所能容纳的最大值
        城市等级在人口数量和设施建设都达到要求后就会提升, 之后人口才能继续增长, 并解锁新的设施建设

        */
    public class Town :MonoBehaviour
    {
        #region Create


        public static Town CreateNewTown()
        {

            Town t1 = new GameObject().AddComponent<Town>();
            t1.BuildingList.Add(new Building("农场"));
            t1.BuildingList.Add(new Building("矿场"));
            t1.BuildingList.Add(new Building("林场"));
            t1.BuildingList.Add(new Building("渔场"));

            t1.BuildingList.Add(new Building("水井"));
            t1.BuildingList.Add(new Building("仓库"));
            t1.BuildingList.Add(new Building("磨坊"));
            t1.BuildingList.Add(new Building("镇中心"));
            t1.BuildingList.Add(new Building("杂货店"));
            t1.BuildingList.Add(new Building("铁匠铺"));
            t1.BuildingList.Add(new Building("裁缝铺"));
            t1.BuildingList.Add(new Building("肉铺"));
            t1.BuildingList.Add(new Building("旅舍"));
            t1.BuildingList.Add(new Building("交易所"));
            t1.BuildingList.Add(new Building("工坊"));
            t1.BuildingList.Add(new Building("船厂"));
            return t1;
        }

        #endregion

        #region 城市发展与设施
        public static string[] TownLevelArr = new string[] { "聚落", "小村庄", "村庄", "小镇", "城镇" };


        #endregion

        //当前等级
        public int CurTownLevel = 0;

        Dictionary<int, List<string>> canBuildDic;
        Dictionary<int, int> levelupPopulationNeed;
        Dictionary<int, List<string>> levelupBuildingNeed;

        void levelConfigInit()
        {
            //获取当前等级能够建造的设施
            canBuildDic = new Dictionary<int, List<string>>();
            canBuildDic.Add(0, new List<string>());
            canBuildDic[0].Add("水井");

            canBuildDic.Add(1, new List<string>());
            canBuildDic[1].Add("大水井");
            canBuildDic[1].Add("仓库");

            canBuildDic.Add(2, new List<string>());
            canBuildDic[2].Add("镇中心");
            canBuildDic[2].Add("铁匠铺");
            canBuildDic[2].Add("杂货铺");
            canBuildDic[2].Add("裁缝铺");
            canBuildDic[2].Add("工坊");
            canBuildDic[2].Add("旅舍");

            canBuildDic.Add(3, new List<string>());
            canBuildDic[3].Add("交易所");
            canBuildDic[3].Add("大旅社");
            canBuildDic[3].Add("船厂");

            //获取当前等级升级所需的人口数量
            levelupPopulationNeed = new Dictionary<int, int>();
            levelupPopulationNeed.Add(0, 10);
            levelupPopulationNeed.Add(1, 50);
            levelupPopulationNeed.Add(2, 100);
            levelupPopulationNeed.Add(3, 300);

            //获取当前等级升级所需的设施
            levelupBuildingNeed = new Dictionary<int, List<string>>();
            levelupBuildingNeed.Add(0, new List<string>());
            levelupBuildingNeed[0].Add("水井");

            levelupBuildingNeed.Add(1, new List<string>());
            levelupBuildingNeed[1].Add("大水井");
            levelupBuildingNeed[1].Add("仓库");

            levelupBuildingNeed.Add(2, new List<string>());
            levelupBuildingNeed[2].Add("镇中心");
            levelupBuildingNeed[2].Add("铁匠铺");
            levelupBuildingNeed[2].Add("杂货铺");
            levelupBuildingNeed[2].Add("旅舍");

            levelupBuildingNeed.Add(3, new List<string>());
            levelupBuildingNeed[3].Add("交易所");
            levelupBuildingNeed[3].Add("大旅社");
        }




        //当前设施
        public List<Building> BuildingList = new List<Building>();

        //人口
        public int Population;

        //需求


        //周期
        public void OnDayPass()
        {
            Population++;





            refreshTownInfo();
        }



        //信息显示
        public string TownInfo="";
        void refreshTownInfo()
        {
            TownInfo = "城市等级为: "+ TownLevelArr[CurTownLevel]+ ", 当前人口: "+Population+", 拥有设施"+BuildingList.ForeachToString(", ");
        }
    }

    public class Building
    {
        public string BuildingName;

        public Building(string buildingName)
        {
            this.BuildingName = buildingName;
        }

        public Action<Town> OnDayPass;
        public Action<Town> OnSeasonPass;
        public Action<Town> OnYearPass;

        public override string ToString()
        {
            return BuildingName;
        }
    }

}
