using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

namespace testTownNS
{
    /// <summary>地点</summary>
    public class Location :IJsonData
    {
        public string Name = "";
        public List<GroupEntry> groupEntryList = new List<GroupEntry>();

        #region Trade
        ItemContainer tradeProxy = new ItemContainer();//市场代理(用来暂存集体交易物资/金额)
        ItemContainer allDemand = new ItemContainer();//所有物品的总需求表
        ItemContainer allSupply = new ItemContainer();//所有物品的总供给表
        /// <summary>记录当前市场价</summary>
        [LitJson("MartkePrice")]
        public Dictionary<string, float> marketPriceDic = new Dictionary<string, float>();

        /// <summary>按Group记录某物的需求</summary>
        Dictionary<string, Dictionary<GroupEntry, float>> demandDic = new Dictionary<string, Dictionary<GroupEntry, float>>();
        /// <summary>按Group记录某物的供给</summary>
        Dictionary<string, Dictionary<GroupEntry, float>> supplyDic = new Dictionary<string, Dictionary<GroupEntry, float>>();

        #endregion

        public void Next()
        {
            tradeProxy.Reset();
            allDemand.Reset();
            allSupply.Reset();

            demandDic.Clear();
            supplyDic.Clear();

            updateGroupLogic();//更新各个群体的属性
            updateDemandSupplyLogic();//更新市场供需
            updateMarketLogic();//实现交易

        }

        //NOTE 171126 先决定供需, 还是先决定价格?
        //如果价格依赖于供需比, 则先决定供需比, 反之亦然
        //但若两者互相依赖, 则会导致价格抖动的问题
        //例如回合一 价格过高, 减少对其需求, 导致本回合价格下跌, 下回合提高需求, 导致价格再次回升
        //考虑如何使此波动趋于平滑和收敛
        //==> 在市场供需比的基础上调整供需, 使比例不超过市场供需比

        void updateGroupLogic()
        {
            for (int iList = 0, nList = groupEntryList.Count; iList < nList; iList++)
            {
                var group = groupEntryList[iList];
                group.Next();
                group.RefreshDemandSupplyDic(marketPriceDic);
            }
        }

        /// <summary>遍历并统计供需值</summary>
        void updateDemandSupplyLogic()
        {
            //刷新供需值
            for (int i_item = 0, n_item = ItemManager.Instance.AllItemList.Count; i_item < n_item; i_item++)
            {
                var item = ItemManager.Instance.AllItemList[i_item];
                float basePrice = ItemManager.Instance.ItemClassDic[item.ItemName].BasePrice;
                supplyDic[item.ItemName] = new Dictionary<GroupEntry, float>();
                demandDic[item.ItemName] = new Dictionary<GroupEntry, float>();

                for (int iList = 0, nList = groupEntryList.Count; iList < nList; iList++)
                {
                    var group = groupEntryList[iList];
                    float delta;
                    if (group.m_kDemandSupplyDic.ContainsKey(item.ItemName))
                        delta = group.m_kDemandSupplyDic[item.ItemName];
                    else
                        delta = 0;
                    if (delta > 0)
                    {
                        supplyDic[item.ItemName].Add(group, delta);
                        allSupply.Add(item.ItemName, delta);
                    }
                    else
                    {
                        delta = -delta;
                        //金钱不足时, 减少需求
                        if (group.Items.Money < delta * basePrice)
                            delta = Mathf.Floor(group.Items.Money / basePrice);

                        demandDic[item.ItemName].Add(group, delta);
                        allDemand.Add(item.ItemName, delta);
                    }
                }
            }
        }

        void updateMarketLogic()
        {
            HashSet<string> tradeHS = new HashSet<string>(allDemand.ItemDic.Keys);
            tradeHS.UnionWith(allSupply.ItemDic.Keys);

            float minTradeNum = 1f;

            foreach (var itemName in tradeHS)
            {
                float basePrice = ItemManager.Instance.ItemClassDic[itemName].BasePrice;
                var demand = allDemand[itemName];
                var supply = allSupply[itemName];

                //交易价格
                float lastPrice = basePrice;
                marketPriceDic.TryGetValue(itemName, out lastPrice);
                //市场对供需的反应应该是迟钝的, 市场价格不应该立刻变为当前供需比, 而是由当前价格趋向目标
                float tarTradePrice = basePrice * TradeManager.GetPriceBiasBySupplyDemand(supply, demand);//目标价格
                float deltaPric = tarTradePrice - lastPrice;//目标价格与上次价格之差
                float deltaBias = 0.3f;//价格变化参数(每次向目标价格增加0.3的差价)
                float tradePrice = deltaPric * deltaBias + lastPrice;
                tradePrice = Mathf.Ceil(tradePrice);
                if (!float.IsNaN(tradePrice))
                    marketPriceDic.AddRep(itemName, tradePrice);

                //交易量
                float tradeWindow = Mathf.Min(demand, supply);//交易窗口
                if (tradeWindow <= minTradeNum)
                    continue;

                float tradeNum = tradeWindow * 0.3f;//交易量修正值, 保证需求缺口被填平前, 价格有足够的时间进行调整来适应需求/供给比例
                tradeNum = (int)tradeNum;
                if (tradeNum < minTradeNum && minTradeNum <= tradeWindow)
                    tradeNum = minTradeNum;//保证最小交易量


                //先购买后出售
                foreach (var pair in demandDic[itemName])
                {
                    float groupTradeNum = pair.Value / demand * tradeNum;
                    TradeManager.TryTrade(tradeProxy, pair.Key.Items, itemName, tradePrice, groupTradeNum, false);

                }

                foreach (var pair in supplyDic[itemName])
                {
                    float groupTradeNum = pair.Value / supply * tradeNum;
                    TradeManager.TryTrade(pair.Key.Items, tradeProxy, itemName, tradePrice, groupTradeNum, false);

                }

            }

            //统计总需求/供给
            //按比例实现分配到给单位
            //结算金额

            //进行交易 将高/低于基本库存的量作为供给/需求
        }

        //TODO NOTE 每个商队交易前需要更新 updateDemandSupplyLogic
        /// <summary>外部交易者获取交易信息</summary>
        /// <param name="itemName"></param>
        /// <param name="isBuy">外部交易者是否是购买</param>
        /// <param name="tradePrice"></param>
        /// <param name="maxTradeNum"></param>
        /// <returns></returns>
        public Action<float> GetTradeInfo(string itemName, bool isBuy, out float tradePrice, out float maxTradeNum)
        {
            Action<float> tradeFunc = null;
            //价格
            float basePrice = ItemManager.Instance.ItemClassDic[itemName].BasePrice;
            tradePrice = basePrice;
            marketPriceDic.TryGetValue(itemName, out tradePrice);
            float _tradePrice = tradePrice;

            //单次交易量
            float demand = 0;
            float supply = 0;
            float tradeAmountBuff = 0.3f;//交易量加成(在不使价格变化的情况下, 一次能交易的量)
            
            if (isBuy)
            {
                supply = allSupply[itemName];
                maxTradeNum = supply * tradeAmountBuff;
                float _maxTradeNum = maxTradeNum;
                tradeFunc = tradeNum => {
                    if (tradeNum > _maxTradeNum)
                        tradeNum = _maxTradeNum;

                    foreach (var pair in supplyDic[itemName])
                    {
                        float groupTradeNum = pair.Value / supply * tradeNum;
                        TradeManager.TryTrade(pair.Key.Items, tradeProxy, itemName, _tradePrice, groupTradeNum, false);
                    }
                };
            }
            else
            {
                demand = allDemand[itemName];
                maxTradeNum = demand * tradeAmountBuff;
                float _maxTradeNum = maxTradeNum;
                tradeFunc = tradeNum => {
                    if (tradeNum > _maxTradeNum)
                        tradeNum = _maxTradeNum;
                    foreach (var pair in demandDic[itemName])
                    {
                        float groupTradeNum = pair.Value / demand * tradeNum;
                        TradeManager.TryTrade(tradeProxy, pair.Key.Items, itemName, _tradePrice, groupTradeNum, false);
                    }
                };
            }
            return tradeFunc;


            //先购买后出售

            //foreach (var pair in supplyDic[itemName])
            //{
            //    float groupTradeNum = pair.Value / supply * tradeNum;
            //    TradeManager.TryTrade(pair.Key.Items, tradeProxy, itemName, tradePrice, groupTradeNum, false);

            //}
        }

        #region JsonData

        public JsonData ToJsonData()
        {
            JsonData jd = this.AutoToJsonData();
            jd["Groups"] = groupEntryList.ToJsonDataList();
            return jd;
        }

        public IJsonData ReadJsonData(JsonData jd)
        {
            this.AutoReadJsonData(jd);
            groupEntryList = jd["Groups"].ToItemVOList<GroupEntry>();
            return this;
        }
        #endregion
        
        public string GetInfo()
        {
            string info = "Location: "+ Name + "\r\n";
            info += "---------------------------\r\n";
            for (int i_group = 0, n_group = groupEntryList.Count; i_group < n_group; i_group++)
            {
                var group = groupEntryList[i_group];
                info += group.GetInfo("--")+"\r\n";
            }
            info += "MarketInfo:\r\n" + marketPriceDic.ForeachToString("\r\n");
            info += "---------------------------";
            return info;
        }
    }

}