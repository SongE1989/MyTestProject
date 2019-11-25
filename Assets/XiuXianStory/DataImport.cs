using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XiuXianStory
{
    public class DataManager
    {
        static DataManager _instance;
        public static DataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataManager();
                    _instance.initBaseDefine();
                    _instance.initDesires();
                    _instance.behaviorAddConditions();
                    _instance.addResultToBehavior();
                    _instance.connectDesireBehavior();
                }
                return _instance;
            }
        }

        //具有唯一性,方便引用
        public Dictionary<string, Level> LevelDic;
        public Dictionary<string, PersonBehavior> BehaviorDic;
        public Dictionary<string, Item> ItemDic;
        public Dictionary<string, Desire> DesireDic;
        public Dictionary<Desire, List<PersonBehavior>> AvaliableDic;
        public Dictionary<string, Magic> MagicDic;

        /// <summary>Levels, Behaviors, Items 需要经常被引用的数据</summary>
        void initBaseDefine()
        {
            LevelDic = new Dictionary<string, Level>();
            LevelDic.AddNameIDItemList(new List<Level>()
            {
                new Level() { LevelName = "炼气期", LevelIndex = 1, LevelType = "修仙大道" },
                new Level() { LevelName = "筑基期", LevelIndex = 2, LevelType = "修仙大道" },
                new Level() { LevelName = "金丹期", LevelIndex = 3, LevelType = "修仙大道" },
                new Level() { LevelName = "元婴期", LevelIndex = 4, LevelType = "修仙大道" },
                new Level() { LevelName = "仙人", LevelIndex = 5, LevelType = "修仙大道" },
                new Level() { LevelName = "一级妖兽", LevelIndex = 1, LevelType = "妖修之道" },
                new Level() { LevelName = "二级妖兽", LevelIndex = 1, LevelType = "妖修之道" },
                new Level() { LevelName = "三级妖兽", LevelIndex = 1, LevelType = "妖修之道" },
            });

            BehaviorDic = new Dictionary<string, PersonBehavior>();
            BehaviorDic.AddNameIDItemList(new List<PersonBehavior>()
            {
                new PersonBehavior(){ NameID = "渡过飞升雷劫", UseTime = 16, MessageLevel = 4},
                new PersonBehavior(){ NameID = "凝结元婴", UseTime = 8, MessageLevel = 3},
                new PersonBehavior(){ NameID = "结成金丹", UseTime = 4, MessageLevel = 2},
                new PersonBehavior(){ NameID = "凝气筑基", UseTime = 2, MessageLevel = 1},
                //new PersonBehavior(){ NameID = "筑基试炼", UseTime = 3},
                new PersonBehavior(){ NameID = "修炼", UseTime = 3},
                new PersonBehavior(){ NameID = "采集灵石", UseTime = 3},
                //new PersonBehavior(){ NameID = "打理灵田", UseTime = 3},
                new PersonBehavior(){ NameID = "凝聚灵石", UseTime = 2},
                new PersonBehavior(){ NameID = "炼制寿元丹", UseTime = 7, MessageLevel = 3},
                new PersonBehavior(){ NameID = "服用寿元丹", UseTime = 2, MessageLevel = 3},
                //new PersonBehavior(){ NameID = "开垦灵田", UseTime = 10, MessageLevel = 1},
            });

            ItemDic = new Dictionary<string, Item>();
            ItemDic.AddNameIDItemList(new List<Item>()
            {
                new Item(){NameID = "灵石"},
                new Item(){NameID = "筑基丹"},
                new Item(){NameID = "金髓丸"},
                new Item(){NameID = "凝婴丹"},
                new Item(){NameID = "金灵石"},
                new Item(){NameID = "木灵石"},
                new Item(){NameID = "水灵石"},
                new Item(){NameID = "火灵石"},
                new Item(){NameID = "土灵石"},
                new Item(){NameID = "灵田"},//TODO 地点系统上线前, 暂时用物品代替设施
                new Item(){NameID = "寿元丹"},
            });

            MagicDic = new Dictionary<string, Magic>();
            MagicDic.AddNameIDItemList(new List<Magic>()
            {
                new BulletMagic(){NameID = "气弹术", Damage = 1, MaxHP = 1, Size = 0.2f, ManaCost=1 },
                new BulletMagic(){NameID = "烈阳斩", Damage = 1, MaxHP = 2, Size = 0.5f, ManaCost=1 },
                new BulletMagic(){NameID = "寒冰刺", Damage = 2, MaxHP = 1, Size = 0.3f, ManaCost=1 },
                new BulletMagic(){NameID = "土墙术", Damage = 0, MaxHP = 3, Size = 0.7f, ManaCost=1 },
            });
        }

        void initDesires()
        {
            DesireDic = new Dictionary<string, Desire>();
            List<Desire> desireList = new List<Desire>() { 
                //behavior desires
                new BehaviorDesire() { TheBehavior = BehaviorDic["渡过飞升雷劫"] },

                //level desires
                new LevelDesire(LevelDic["炼气期"]),
                new LevelDesire(LevelDic["筑基期"]),
                new LevelDesire(LevelDic["金丹期"]),
                new LevelDesire(LevelDic["元婴期"]),
                new AttrDesire(AttrDesire.AttrDesireType.Add, "LifeTime", 0),//此处是抽象需求, 作为索引的Key
            };
            for (int i = 0; i < desireList.Count; i++)
            {
                var desire = desireList[i];
                DesireDic.Add(desire.NameID, desire);
            }
            foreach (var item in ItemDic.Values)
            {
                DesireDic.Add(item.NameID, new ItemDesire(item, 1, true));//ItemDesire有数量区别
            }
        }

        void behaviorAddConditions()
        {
            BehaviorDic["凝气筑基"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["炼气期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["筑基丹"],1, true), IsNecessary = false }.AddBuff(new Buff("成功",1.5f)),
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],100, true), IsNecessary = true},
            };
            BehaviorDic["结成金丹"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["筑基期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["金髓丸"],1, true), IsNecessary = false }.AddBuff(new Buff("成功",1.5f)),
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],500, true), IsNecessary = true },
            };
            BehaviorDic["凝结元婴"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["金丹期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["凝婴丹"],1, true), IsNecessary = false }.AddBuff(new Buff("成功",1.5f)),
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],1000, true), IsNecessary = true },
            };
            BehaviorDic["渡过飞升雷劫"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["元婴期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],10000, true), IsNecessary = true },
                new Condition(){ TheDesire = new MultiDesire(){ MultiDesireName="获取五行灵珠", ChildList=new List<Desire>(){
                        new ItemDesire(ItemDic["金灵石"],1, true),
                        new ItemDesire(ItemDic["木灵石"],1, true),
                        new ItemDesire(ItemDic["水灵石"],1, true),
                        new ItemDesire(ItemDic["火灵石"],1, true),
                        new ItemDesire(ItemDic["土灵石"],1, true),
                    } }, IsNecessary = false}.AddBuff(new Buff("成功",1.5f)),
            };
            //BehaviorDic["打理灵田"].ConditionList = new List<Condition>() {
            //    new Condition(){ TheDesire = new ItemDesire(ItemDic["灵田"], 1, false), IsNecessary =true }
            //};
            BehaviorDic["炼制寿元丹"].ConditionList = new List<Condition>() {
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],10000, true), IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["金灵石"], 1, true), IsNecessary =true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["木灵石"], 1, true), IsNecessary =true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["水灵石"], 1, true), IsNecessary =true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["火灵石"], 1, true), IsNecessary =true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["土灵石"], 1, true), IsNecessary =true },
            };
            BehaviorDic["服用寿元丹"].ConditionList = new List<Condition>() {
                new Condition(){ TheDesire = new ItemDesire(ItemDic["寿元丹"], 1, true), IsNecessary =true }
            };
            //BehaviorDic["开垦灵田"].ConditionList = new List<Condition>() {
            //    new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"], 10000, true), IsNecessary =true }
            //};

            foreach (var pair in BehaviorDic)
            {
                if (pair.Value.ConditionList == null)
                    pair.Value.ConditionList = new List<Condition>();
            }
        }

        void addResultToBehavior()
        {
            BehaviorDic["凝气筑基"].ResultGroupDic = new Dictionary<ResultGroup, float>()
            {
                { new ResultGroup(){ GroupName = "成功", MessageLevelDelta = 1, ResultList = new List<Result>(){
                    new LevelResult(){ TargetLevel = LevelDic["筑基期"]},
                    new AttrResult("Age", DataHelper.GetRandomLifeTimeDelta(1))
                } },100f },
                { new ResultGroup(){ GroupName = "失败" ,RecordAsBehaviorDone = false },100f },
                { new ResultGroup(){ GroupName = "失败(重)" ,RecordAsBehaviorDone = false, ResultList = new List<Result>(){
                    new HealthResult(){ Effect = "轻伤(静脉受损)"}
                } },1f },
            };
            BehaviorDic["结成金丹"].ResultGroupDic = new Dictionary<ResultGroup, float>() {
                { new ResultGroup(){
                    GroupName = "成功", MessageLevelDelta = 1, ResultList = new List<Result>()
                    {
                        new LevelResult(){ TargetLevel = LevelDic["金丹期"]},
                        new HealthResult(){ Effect = "领悟提升"},
                        new AttrResult("Age", DataHelper.GetRandomLifeTimeDelta(1)),
                    }
                }, 100f },
                { new ResultGroup(){ GroupName = "失败" ,RecordAsBehaviorDone = false }, 100f},
                { new ResultGroup()
                {
                    GroupName = "失败(重)" ,RecordAsBehaviorDone = false, ResultList= new List<Result>(){ new HealthResult() { Effect = "轻伤(静脉受损)" } }
                }, 100f}
            };
            BehaviorDic["凝结元婴"].ResultGroupDic = new Dictionary<ResultGroup, float>() {
                { new ResultGroup(){
                    GroupName = "成功", MessageLevelDelta = 1,ResultList = new List<Result>()
                    {
                        new LevelResult(){ TargetLevel = LevelDic["元婴期"]},
                        new HealthResult(){ Effect = "神功大成"},
                        new AttrResult("Age", DataHelper.GetRandomLifeTimeDelta(1)),
                    }
                }, 100f },
                { new ResultGroup(){ GroupName = "失败" ,RecordAsBehaviorDone = false }, 100f},
                { new ResultGroup()
                {
                    GroupName = "失败(重)" ,RecordAsBehaviorDone = false, ResultList= new List<Result>(){ new HealthResult() { Effect = "轻伤(静脉受损)" } }
                }, 100f}
            };
            BehaviorDic["渡过飞升雷劫"].ResultGroupDic = new Dictionary<ResultGroup, float>() {
                { new ResultGroup(){
                    GroupName = "成功", MessageLevelDelta =1, ResultList = new List<Result>()
                    {
                        new LevelResult(){ TargetLevel = LevelDic["仙人"]},
                        new HealthResult(){ Effect = "神功大成"},
                        new AttrResult("Age", DataHelper.GetRandomLifeTimeDelta(1)),
                    }
                }, 100f },
                { new ResultGroup(){ GroupName = "失败" ,RecordAsBehaviorDone = false}, 0},
                { new ResultGroup()
                {
                    GroupName = "失败(重)" ,RecordAsBehaviorDone = false, ResultList= new List<Result>(){ new HealthResult() { Effect = "轻伤(静脉受损)" } }
                }, 100}
            };
            BehaviorDic["采集灵石"].ResultGroupDic = new Dictionary<ResultGroup, float>() {
                { new ResultGroup(){ GroupName = "结果1" , ResultList = DataHelper.SingleItemChangeList("灵石",100) }, 20 },
                { new ResultGroup(){ GroupName = "结果2" , ResultList = DataHelper.SingleItemChangeList("灵石",1000) }, 10f },
                { new ResultGroup(){ GroupName = "结果3" , ResultList = DataHelper.SingleItemChangeList("灵石",10000) }, 1f },
                { new ResultGroup(){ GroupName = "结果4" , ResultList = DataHelper.SingleItemChangeList("金灵石",1) }, 1f },
                { new ResultGroup(){ GroupName = "结果5" , ResultList = DataHelper.SingleItemChangeList("木灵石",1) }, 1f },
                { new ResultGroup(){ GroupName = "结果6" , ResultList = DataHelper.SingleItemChangeList("水灵石",1) }, 1f },
                { new ResultGroup(){ GroupName = "结果7" , ResultList = DataHelper.SingleItemChangeList("火灵石",1) }, 1f },
                { new ResultGroup(){ GroupName = "结果8" , ResultList = DataHelper.SingleItemChangeList("土灵石",1) }, 1f },
                //TODO 在炼丹, 交易系统完成前, 暂时由采集完成药物的收集
                { new ResultGroup(){ GroupName = "结果9" , ResultList = DataHelper.SingleItemChangeList("筑基丹",1) }, 1f },
                { new ResultGroup(){ GroupName = "结果10" , ResultList = DataHelper.SingleItemChangeList("金髓丸",1) }, 1f },
                { new ResultGroup(){ GroupName = "结果11" , ResultList = DataHelper.SingleItemChangeList("凝婴丹",1) }, 1f },
            };
            //BehaviorDic["打理灵田"].ResultGroupDic = new Dictionary<ResultGroup, float>(){
            //    { new ResultGroup(){ GroupName = "结果1" , ResultList = DataHelper.SingleItemChangeList("灵石",500) }, 20 },
            //    { new ResultGroup(){ GroupName = "结果2" , ResultList = DataHelper.SingleItemChangeList("灵石",5000) }, 10f },
            //    { new ResultGroup(){ GroupName = "结果3" , ResultList = DataHelper.SingleItemChangeList("灵石",50000) }, 1f },
            //};
            BehaviorDic["炼制寿元丹"].ResultGroupDic = new Dictionary<ResultGroup, float>(){
                { new ResultGroup(){ GroupName = "炼制成功", ResultList = new List<Result>(){ DataHelper.SingleItemChange("寿元丹", 1) }, MessageLevelDelta = 0.5f }, 50f},
                { new ResultGroup(){ GroupName = "炼制失败", ResultList = new List<Result>() }, 50f},
            };
            BehaviorDic["服用寿元丹"].ResultGroupDic = new Dictionary<ResultGroup, float>(){
                { new ResultGroup(){ GroupName = "寿元增加", ResultList = new List<Result>(){new AttrResult("LifeTime", 100) } }, 100f}
            };
            //BehaviorDic["开垦灵田"].ResultGroupDic = new Dictionary<ResultGroup, float>(){
            //    { new ResultGroup(){ GroupName = "开垦成功", ResultList = DataHelper.SingleItemChangeList("灵田",1) }, 50f},
            //    { new ResultGroup(){ GroupName = "开垦失败", ResultList = new List<Result>() }, 50f}
            //};
            //TODO 添加开坑灵田
        }

        //此处应只联结抽象需求与行为. 具体需求按照父级的抽象需求, 来查找行为. 然后按自身提交来筛选行为
        void connectDesireBehavior()
        {
            AvaliableDic = new Dictionary<Desire, List<PersonBehavior>>();
            foreach (var desire in DesireDic.Values)
            {
                foreach (var behavior in BehaviorDic.Values)
                {
                    if (behavior.ResultGroupDic == null)
                        continue;
                    if (desire is BehaviorDesire)
                    {
                        if ((desire as BehaviorDesire).TheBehavior == behavior)
                            AvaliableDic.AddToList(desire, behavior);
                    }
                    else
                    {
                        foreach (var resultGroup in behavior.ResultGroupDic.Keys)
                        {
                            if (resultGroup.ResultList == null)
                                continue;
                            for (int i = 0; i < resultGroup.ResultList.Count; i++)
                            {
                                var result = resultGroup.ResultList[i];
                                if (result is LevelResult && desire is LevelDesire)
                                {
                                    if ((result as LevelResult).TargetLevel == (desire as LevelDesire).TheLevel)
                                        AvaliableDic.AddToList(desire, behavior);
                                }
                                else if (result is ItemResult && desire is ItemDesire)
                                {
                                    if ((result as ItemResult).ItemDic.ContainsKey((desire as ItemDesire).TheItem))
                                        AvaliableDic.AddToList(desire, behavior);
                                }
                                else if (result is AttrResult && desire is AttrDesire)
                                {
                                    var ar = result as AttrResult;
                                    var ad = desire as AttrDesire;
                                    if (ar.AttrName == ad.AttrName)
                                    {
                                        switch (ad.Type)
                                        {
                                            case AttrDesire.AttrDesireType.Add:
                                                if (ar.DeltaValue > 0 && ar.DeltaValue >= ad.TargetValue)
                                                    AvaliableDic.AddToList(desire, behavior);
                                                break;
                                            case AttrDesire.AttrDesireType.Minus:
                                                if (ar.DeltaValue < 0 && ar.DeltaValue <= ad.TargetValue)
                                                    AvaliableDic.AddToList(desire, behavior);
                                                break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

            }

        }

        public void GetBehaviorListByDesire(Desire desire, ref List<PersonBehavior> list)
        {
            if (list == null)
                list = new List<PersonBehavior>();
            if (desire.AsItemDesire != null)
            {
                //此处为处理抽象需求与具体需求的实例
                foreach (var pair in AvaliableDic)
                {
                    if (pair.Key is ItemDesire
                        && pair.Key.AsItemDesire.TheItem.NameID == desire.AsItemDesire.TheItem.NameID)
                    {
                        list.AddRange(pair.Value);
                        break;
                    }
                }
            }
            else if (desire.AsMultiDesire != null)
            {
                for (int i = 0; i < desire.AsMultiDesire.ChildList.Count; i++)
                {
                    GetBehaviorListByDesire(desire.AsMultiDesire.ChildList[i], ref list);
                }
            }
            else if(desire.AsAttrDesire != null)
            {
                //此处为处理抽象需求与具体需求的实例
                foreach (var pair in AvaliableDic)
                {
                    if(desire.AsAttrDesire.IsSameKind(pair.Key))
                    {
                        list.AddRange(pair.Value);
                        break;
                    }
                }
            }
            else
            {
                List<PersonBehavior> _list;
                AvaliableDic.TryGetValue(desire, out _list);
                list.AddRange(_list);
            }
            if(list.Count == 0)
            {
                Debug.LogError("未找到Desire"+ desire+"对应的行为!");
            }
        }

    }
}