using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RimWorldNS
{
    public class HealthModule
    {
        #region 全局静态
        static Dictionary<AnimalState, int> normalEnergyConsumeDic;
        public static Dictionary<AnimalState, int> GetNormalEnergyConsumeDic()
        {
            if(normalEnergyConsumeDic == null)
            {
                normalEnergyConsumeDic = new Dictionary<AnimalState, int>();
                normalEnergyConsumeDic.Add(AnimalState.normal, 5);
                normalEnergyConsumeDic.Add(AnimalState.sleep, 2);
                normalEnergyConsumeDic.Add(AnimalState.move, 10);
                normalEnergyConsumeDic.Add(AnimalState.active, 30);
            }
            return normalEnergyConsumeDic;
        }

        public static int GetConsumeByState(AnimalState state)
        {
            int energyConsumeRate = 0;
            foreach (AnimalState type in Enum.GetValues(typeof(AnimalState)))
            {
                if(type != AnimalState.errorState)
                    if ((type & state) == type)
                        energyConsumeRate = Mathf.Max(GetNormalEnergyConsumeDic()[type], energyConsumeRate);
            }
            return energyConsumeRate;
        }

        #endregion

        public Value_int HealthPoint;
        public Value_int EnergyPoint;
        public Value_int RestPoint;

        public AnimalState TheState;

        public HealthModule()
        {

        }

        public HealthModule(int maxHP, int minHP, int curHP, int maxEP, int minEP, int curEP, int maxRP, int minRP, int curRP)
        {
            HealthPoint = new Value_int(maxHP, minHP, curHP);
            EnergyPoint = new Value_int(maxEP, minEP, curEP);
            RestPoint = new Value_int(maxRP, minRP, curRP);
        }

        public void RefreshHealth()
        {
            EnergyPoint.CurValue -= GetConsumeByState(TheState);//TODO 无法获取全局状态AnimalState!
        }
    }
}
