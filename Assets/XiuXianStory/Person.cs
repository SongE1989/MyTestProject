using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XiuXianStory
{
    public enum EnumSex { Male, Female }

    public class Attr
    {
        public const string Age = "Age";
        public const string LifeTime = "LifeTime";
        public const string MaxHP = "MaxHP";
        public const string CurHP = "CurHP";

    }

    public class Person : ILevel, INameID
    {
        public string NameID { get; set; }
        //public int Age;
        //public int LifeTime { get; set; }
        public EnumSex Sex;
        public Level TheLevel { get; set; }
        public Dictionary<Item, int> Bag = new Dictionary<Item, int>();
        public BodySystem TheBodySystem;
        public ManaSystem TheManaSystem;
        //public float MaxHP;
        //public float CurHP;


        Dictionary<string, float> attrDic = new Dictionary<string, float>();
        public float this[string key] {
            get
            {
                if (!attrDic.ContainsKey(key))
                    attrDic.Add(key, 0);
                return attrDic[key];
            }
            set
            {
                if (!attrDic.ContainsKey(key))
                    attrDic.Add(key, 0);
                attrDic[key] = value;
            }
        }
        
        /// <summary>人物根源需求</summary>
        public List<Desire> RootDesireList = new List<Desire>();
        public List<Story> StoryList = new List<Story>();
        public Desire CurLongTimeDesire = null;
        public Desire CurShortTimeDesire = null;
        public PersonBehavior CurBehavior;
        public PersonBehaviorProxy CurBehaviorProxy;
        //TODO 如何处理一次行为(游历), 永远生效的问题?(类似ItemConsume标记是否可行?)
        //TODO 如何处理有特定目标的行为(结交某人)?(是否需要引入BehaviorProxy?)
        /// <summary>记录某一行为是否执行过true/false(例如外出游历)</summary>
        public Dictionary<BehaviorDesire, bool> BehaviorDesireHaveDoneDic = new Dictionary<BehaviorDesire, bool>();
        public List<Magic> LearnedMagicList = new List<Magic>();

        public bool IsAlive = true;

        public Person Initilize()
        {
            initBodySystem();
            return this;
        }

        public string Title
        {
            get
            {
                if (IsAlive)
                    return NameID + "[" + TheLevel.LevelName + "] " + (int)(this[Attr.Age] / 12) + "/" + (int)(this[Attr.LifeTime] / 12) + "(" + (int)((this[Attr.LifeTime] - this[Attr.Age]) / 12) + ")";
                else
                    return "(死亡)" + NameID + "[" + TheLevel.LevelName + "] 享年" + this[Attr.Age] / 12;
            }
        }
        public string HPState { get { return NameID + "(" + this[Attr.CurHP] + "/" + this[Attr.MaxHP] + ")"; } }

        void initBodySystem()
        {
            /*
            丹田
            气 血 法
            三魂七魄

            宠物系统

            修仙者与其他生物的区别(妖兽,类人生物,鲲)
             
            肉身 气 法力 神念
             
            战斗系统
            初级修仙者的战斗 少量法术+胜负取决于法术强弱
            中级..           成体系的攻防战, 比拼法力/气血等基础能力
            高级..           难以互相击杀, 逃走相对容易(元婴出逃, 夺舍), 除非以多打少或是设下埋伏

            法宝 抵挡 攻击性 防御性 辅助性
            克制 污秽>普通>辟邪>污秽 五行相克

            第一版战斗系统 简单比大小 先把法力系统实现了

            初级阶段战斗(练气-筑基)
            攻击手段
            --法术(需要施法, 消耗法力)
            --法宝(稀有,需要一定法力支撑)
            --符箓(消耗,昂贵,无需法力支撑)

            数学模型(攻击-血量-法力)
            炼气期 1
            筑基期 3
            结丹期 6
            元婴期 10
            保证战力层次不会轻易跨过, 同时具有一定随机性
            力修,剑仙与近战(战斗是否分远程与近战阶段)
            遁术对战斗的影响(长跑, 短跑)
            第一版设计是否引入法术?(需要增加法术选择AI)

            考虑多人战斗, 法阵的引入
            法阵 需提前布置, 并由修仙者主持, 阵眼被破会导致阵法失效
            一个基础法阵设定: 必须先攻破敌方法阵才能攻击法阵内人员
            法阵需要法力支持, 法阵攻击力, 法阵血量
            双方法阵都破坏后, 开始进入捉对厮杀阶段(王对王)(此处需要战力分配逻辑, 与多对多战斗逻辑)

            单人战斗模型
            练气对练气 释放法术对轰, 烈阳斩 轰击在 飞石术
            飞弹术 攻击-血量 1-1-0.2 普通低模法术
            烈阳斩 攻击-血量 2-1-0.3 高伤害法术(一旦命中伤害增加)
            飞石术 攻击-血量 1-2-0.5 高血量法术(同级法术对轰中仍能削减对方血量)
            土墙术 攻击-血量 0-3-0.7 防御性法术

            法力回复概念(调整气息, 调动法力)
            法力池概念(能通过调整气息, 回复的法力总量)
            法力上限 通过调整气息所能获得并保持的法力总量

            法术加入法术面积概念(偷袭类法术法术面积低, 防御性法术法术面积高)
            0法术面积意味不可阻挡的偷袭法术(无法术体积, 例如诅咒)
            0.1法术面积意味易穿透的偷袭法术
            1法术面积意味全方位防御法术(占据整个对轰通道)
            0 VS 1 = 互相穿透
            0.5 VS 0.5 = 互相碰撞
            0.4 VS 0.3 = (1-0.3) - 0.4 = 0.3互相穿透
            a VS B = Math.Max(0, 1-a-b) 穿透率


            人物 法力容量 1, 法力回复0.5(每两回合释放一个法术)
            考虑换气式法力回复(使用一个或数个回合回复法力到最大值, 期间为脆弱期)
            --高容量人物爆发高, 换气短人物破绽少
            

             */

            var mainBody = new BodyPart("躯干");
            TheBodySystem = new BodySystem() { MainBody = mainBody };
            mainBody.AddChild("头部");
            mainBody.AddChild("左臂");
            mainBody.AddChild("右臂");
            mainBody.AddChild("左腿");
            mainBody.AddChild("右腿");
            mainBody["头部"].AddChild("左眼");
            mainBody["头部"].AddChild("右眼");
            mainBody["头部"].AddChild("左耳");
            mainBody["头部"].AddChild("右耳");
            mainBody["头部"].AddChild("嘴");
            mainBody["头部"].AddChild("鼻");

            TheManaSystem = new ManaSystem(1, 1, 1);

        }

        public Person AddRootDesire(Desire desire)
        {
            RootDesireList.Add(desire);
            if (desire is BehaviorDesire && !BehaviorDesireHaveDoneDic.ContainsKey(desire as BehaviorDesire))
            {
                BehaviorDesireHaveDoneDic.Add(desire as BehaviorDesire, false);
            }
            return this;
        }

        public void UpdateGameLogic()
        {
            if (!IsAlive)
                return;

            //从长期需求池中选出[当前长期需求], 并选出对应的[当前需求](若当前长期需求对应行为无法直接执行时)
            if (CurBehaviorProxy == null)
            {
                refreshCurrentDesireAndBehavior();

                //TODO 190623 扣除Condition中物品消耗
                //TODO 此处缺陷: 应在决定执行某行为时, 同时确定要满足(消耗)的非必要条件
                //TODO 需要引入人物性格系统(谨慎-冒险), 来确定是否满足非必要条件
                for (int i = 0; i < CurBehavior.ConditionList.Count; i++)
                {
                    var con = CurBehavior.ConditionList[i];
                    //TODO BUG 此处若IsNecessary为true, checkCondition不会执行
                    if (con.IsNecessary || checkCondition(con))
                    {
                        //if(con.TheDesire)
                    }
                }

                if (CurBehavior != null)
                {
                    CurBehaviorProxy = startBehavior(CurBehavior);
                }
            }
            bool isFinish = false;
            if (CurBehaviorProxy != null)
            {
                isFinish = processBehaviorProxy(CurBehaviorProxy);
            }

            string why = "";
            if (CurLongTimeDesire == null)
                why = "心中无欲无求";
            else if (CurLongTimeDesire != null && CurShortTimeDesire != null)
            {
                if (CurLongTimeDesire == CurShortTimeDesire)
                    why = "希望实现 " + CurLongTimeDesire.NameID;
                else
                    why = "希望实现 " + CurShortTimeDesire.NameID;//NOTE 此处暂时隐去对应的长期需求的显示
            }
            else if (CurLongTimeDesire != null && CurShortTimeDesire == null)
                why = "希望实现 " + CurLongTimeDesire.NameID + ", 却无计可施";
            else
                why = "Error";

            if (CurBehaviorProxy == null)
                recordStory(why, "无所事事", 0);
            else
                recordStory(why, CurBehaviorProxy.ToStory(), CurBehaviorProxy.GetMessageLevel());

            if (isFinish)
            {
                CurBehaviorProxy = null;
                CurBehavior = null;
            }

            this[Attr.Age]++;
            if (this[Attr.Age] > this[Attr.LifeTime])
            {
                Die();
            }
        }

        public void Die()
        {
            IsAlive = false;
            recordStory("寿元耗尽", "寿终正寝, 享年" + this[Attr.Age] / 12, TheLevel.LevelIndex + 1);
        }

        PersonBehaviorProxy startBehavior(PersonBehavior behavior)
        {
            var proxy = new PersonBehaviorProxy()
            {
                TargetBehavior = behavior,
                DayUsed = 0,
            };
            return proxy;
        }

        void satisfyDesire(Desire desire)
        {
            if(desire.AsItemDesire != null)
            {
                if(desire.AsItemDesire.Consume)
                {
                    //TODO 此处应计入结果
                    Bag.AddValue(desire.AsItemDesire.TheItem, -desire.AsItemDesire.Num);
                }
            }
            else if(desire.AsMultiDesire != null)
            {
                for (int i = 0; i < desire.AsMultiDesire.ChildList.Count; i++)
                {
                    var childDesire = desire.AsMultiDesire.ChildList[i];
                    satisfyDesire(childDesire);
                }
            }
        }

        /// <summary></summary>
        /// <param name="behaviorProxy"></param>
        /// <returns>行为是否完成</returns>
        bool processBehaviorProxy(PersonBehaviorProxy behaviorProxy)
        {
            //TODO 此处未考虑执行时间==0的情况
            behaviorProxy.DayUsed++;
            if (behaviorProxy.DayUsed >= behaviorProxy.TargetBehavior.UseTime)
            {
                finishBehaviorProxy(behaviorProxy);
                return true;
            }
            return false;
        }

        void finishBehaviorProxy(PersonBehaviorProxy behaviorProxy)
        {
            behaviorProxy.IsFinish = true;
            //TODO 处理各种possible加成(采矿加成)
            Dictionary<Vector2, ResultGroup> possibleRange2ResultGroup = new Dictionary<Vector2, ResultGroup>();
            float curRange = 0;
            foreach (var pair in behaviorProxy.TargetBehavior.ResultGroupDic)
            {
                if (pair.Value > 0)//NOTE 为0的部分不加入计算, 防止多个为0的部分Key冲突
                {
                    possibleRange2ResultGroup.Add(new Vector2(curRange, curRange + pair.Value), pair.Key);
                    curRange += pair.Value;
                }
            }
            var ran = Random.Range(0, curRange);
            ResultGroup finalResultGroup = null;
            foreach (var pair in possibleRange2ResultGroup)
            {
                if (ran >= pair.Key.x && ran < pair.Key.y)
                {
                    finalResultGroup = pair.Value;
                    break;
                }
            }
            if (finalResultGroup != null)
            {
                for (int iResult = 0; iResult < finalResultGroup.ResultList.Count; iResult++)
                {
                    var result = finalResultGroup.ResultList[iResult];
                    doResult(result);
                }
            }
            else
            {
                Debug.LogError("Cant find final result group of " + behaviorProxy.TargetBehavior.NameID);
            }

            if (finalResultGroup.RecordAsBehaviorDone)
            {
                foreach (var pair in BehaviorDesireHaveDoneDic)
                {
                    if (pair.Key.TheBehavior == behaviorProxy.TargetBehavior)
                    {
                        BehaviorDesireHaveDoneDic[pair.Key] = true;
                        break;
                    }
                }
            }
            behaviorProxy.TheResultGroup = finalResultGroup;
        }

        //TODO 抢夺行为, A 物品增加, B物品减少(同时还要检查A物品数量是否足够抢夺)
        void doResult(Result result)
        {
            if (result.AsItemChangeResult != null)
            {
                foreach (var pair in result.AsItemChangeResult.ItemDic)
                {
                    int itemHave = 0;
                    if (!Bag.ContainsKey(pair.Key))
                        Bag.Add(pair.Key, 0);
                    //TODO 考虑是否需要加入物品不足警告
                    Bag[pair.Key] = Mathf.Max(0, Bag[pair.Key] + pair.Value);
                }
            }
            else if (result.AsLevelChangeResult != null)
            {
                TheLevel = result.AsLevelChangeResult.TargetLevel;
            }
            else if (result.AsHealthResult != null)
            {
                //TODO 添加Health系统
                //TODO 添加BUFF系统
            }
            else if (result.AsDataResult != null)
            {
                attrDic.AddValue(result.AsDataResult.AttrName, result.AsDataResult.DeltaValue);
            }
        }

        /*
        罗列Desire并根据Peronal赋予权重
        罗列Desire对应的Behavior并赋予权重
        根据权重选择行为

        对某一Desire-检查所有对应行为
        --从所有对应行为中选出偏好的行为(TODO)
        ----检查行为的每一条件
        ----1.全部必要条件满足 => 
        ------1.执行该行为
        ------2.检查非必要条件完成情况, 再根据人物性格(稳重/果断)决定是继续完成非必要条件还是直接执行该行为
        ----2.某条件不满足 => 检查条件Desire
         */
        //TODO 管理人物需求, 决定优先满足, 最终决定当前行为
        //TODO 控制人物对需求的选择, 以及对对应行为的筛选(有所为有所不为)
        void refreshCurrentDesireAndBehavior()
        {
            if (CurLongTimeDesire == null || checkDesire(CurLongTimeDesire))
            {
                CurLongTimeDesire = null;
                for (int i_des = 0; i_des < RootDesireList.Count; i_des++)
                {
                    var desire = RootDesireList[i_des];
                    if (!checkDesire(desire))
                        CurLongTimeDesire = desire;
                }
            }
            if (CurLongTimeDesire == null)
                return;


            Dictionary<PersonBehavior, int> behavior2LayerDic = new Dictionary<PersonBehavior, int>();
            Dictionary<PersonBehavior, Desire> behaviorReasonDic = new Dictionary<PersonBehavior, Desire>();
            getAllBehaviorToBeDone(CurLongTimeDesire, behavior2LayerDic, behaviorReasonDic, 0);

            int curLayer = 0;
            PersonBehavior tempBehavior = null;
            //优先做底层任务(既layer较大的)
            foreach (var pair in behavior2LayerDic)
            {
                if (tempBehavior == null || pair.Value > curLayer)
                {
                    tempBehavior = pair.Key;
                    CurShortTimeDesire = behaviorReasonDic[pair.Key];
                    curLayer = pair.Value;
                }
            }
            CurBehavior = tempBehavior;
        }

        void recordStory(string why, string what, float messageLevel)
        {
            StoryList.Add(new Story()
            {
                When = XiuXianStory.Instance.Day,
                Where = "某地",//TODO 加入地点系统
                Who = NameID,
                What = what,
                Why = why,
                MessageLevel = messageLevel,
            });
        }

        void getAllBehaviorToBeDone(Desire desire, Dictionary<PersonBehavior, int> behavior2LayerDic, Dictionary<PersonBehavior, Desire> behaviorReasonDic, int layer)
        {
            List<PersonBehavior> behaviorList = new List<PersonBehavior>();
            DataManager.Instance.GetBehaviorListByDesire(desire, ref behaviorList);
            for (int iBehavior = 0; iBehavior < behaviorList.Count; iBehavior++)
            {
                var behavior = behaviorList[iBehavior];

                //只记录满足条件的
                if (checkConditionList(behavior.ConditionList))
                {
                    //由于优先做低级任务, 所有Layer记录最子级的
                    int oldLayer;
                    if (behavior2LayerDic.TryGetValue(behavior, out oldLayer))
                    {
                        if (oldLayer < layer)
                        {
                            behavior2LayerDic[behavior] = layer;
                            behaviorReasonDic[behavior] = desire;
                        }
                    }
                    else
                    {
                        behavior2LayerDic.Add(behavior, layer);
                        behaviorReasonDic.Add(behavior, desire);
                    }
                }
                else
                {
                    for (int iCondition = 0; iCondition < behavior.ConditionList.Count; iCondition++)
                    {
                        var condition = behavior.ConditionList[iCondition];
                        if (checkCondition(condition))
                            continue;
                        if (layer < 20)//TODO TEST
                            getAllBehaviorToBeDone(condition.TheDesire, behavior2LayerDic, behaviorReasonDic, layer + 1);
                        else
                            Debug.LogError(desire.NameID + " 查找层级>20, 检查是否Condition-Behavior环状结构");
                    }
                }
            }
        }

        //TODO 检查lockbag是否需要作为成员变量(防止物品数据丢失)

        bool checkDesire(Desire desire)
        {
            return checkDesireWithRecord(desire);
        }

        /// <summary>检查一组条件是否"同时"满足(物品消耗类型需要锁物品)</summary>
        bool checkConditionList(List<Condition> conditionList)
        {
            if (conditionList == null)
                return true;

            Dictionary<Item, int> recordBag = new Dictionary<Item, int>();
            for (int i = 0; i < conditionList.Count; i++)
            {
                if (!checkDesireWithRecord(conditionList[i].TheDesire))
                {
                    return false;
                }
            }
            return true;
        }

        bool checkCondition(Condition condition)
        {
            return checkDesireWithRecord(condition.TheDesire);
        }

        /// <summary>累计物品需求</summary>
        bool checkDesireWithRecord(Desire desire, Dictionary<Item, int> recordBag = null)
        {
            if (recordBag == null)
                recordBag = new Dictionary<Item, int>();
            var itemDesire = desire as ItemDesire;
            if (itemDesire != null)
            {
                int haveNum;
                if (Bag.TryGetValue(itemDesire.TheItem, out haveNum))
                {
                    int recordNum = 0;
                    recordBag.TryGetValue(itemDesire.TheItem, out recordNum);
                    if (haveNum >= itemDesire.Num + recordNum)
                    {
                        recordBag.Add(itemDesire.TheItem, itemDesire.Num);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            var levelDesire = desire as LevelDesire;
            if (levelDesire != null)
            {
                return (TheLevel.LevelIndex >= levelDesire.TheLevel.LevelIndex)
                    && TheLevel.LevelType == levelDesire.TheLevel.LevelType;
            }
            var behaviorDesire = desire as BehaviorDesire;
            if (behaviorDesire != null)
            {
                bool haveDone = false;
                BehaviorDesireHaveDoneDic.TryGetValue(behaviorDesire, out haveDone);
                return haveDone;
            }
            var multiDesire = desire as MultiDesire;
            if (multiDesire != null)
            {
                for (int i = 0; i < multiDesire.ChildList.Count; i++)
                {
                    var childDesire = multiDesire.ChildList[i];
                    if (!checkDesireWithRecord(childDesire, recordBag))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public string GetAllHistoryStr(float messageLevel)
        {
            StringBuilder history = new StringBuilder();
            for (int i = StoryList.Count - 1; i >= 0; i--)
            {
                if (StoryList[i].MessageLevel >= messageLevel)
                    history.AppendLine(StoryList[i].ToString());
            }

            return history.ToString();
        }

        public Story GetLastStory(float messageLevel)
        {
            var last = StoryList[StoryList.Count - 1];
            if (last.MessageLevel >= messageLevel)
                return last;
            else
                return null;
        }

        public string GetStateInfo()
        {
            return "姓名:" + NameID + "\r\n"
                + "年龄:" + this[Attr.Age] + "\r\n"
                + "性别:" + (Sex == EnumSex.Male ? "男" : "女") + "\r\n"
                + "等级:" + TheLevel.LevelName + "\r\n"
                + "----------物品栏----------\r\n" + DicHelper.GetBagInfo(Bag) + "\r\n";
        }

    }



    ////管理人物需求
    //public class DesireSystem
    //{
    //    public Person ThePerson;

    //}

    ////控制人物对需求的选择, 以及对对应行为的筛选(有所为有所不为)
    //public class PersonalSystem
    //{
    //    public Person ThePerson;

    //}
}