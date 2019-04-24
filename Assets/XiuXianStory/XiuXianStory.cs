using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
namespace XiuXianStory
{
    public interface INameID
    {
        string NameID { get; }
    }

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
                    _instance.initBehaviorConditions();
                    _instance.initDesireAvailableBehaviorList();
                }
                return _instance;
            }
        }

        //具有唯一性,方便引用
        public Dictionary<string, Level> LevelDic;
        public Dictionary<string, PersonBehavior> BehaviorDic;
        public Dictionary<string, Item> ItemDic;
        public Dictionary<string, Desire> DesireDic;

        /// <summary>Level Behavior Item</summary>
        void initBaseDefine()
        {
            LevelDic = new Dictionary<string, Level>();
            List<Level> levelList = new List<Level>() {
                new Level() { LevelName = "炼气期", LevelIndex = 1, LevelType = "修仙大道" },
                new Level() { LevelName = "筑基期", LevelIndex = 2, LevelType = "修仙大道" },
                new Level() { LevelName = "金丹期", LevelIndex = 3, LevelType = "修仙大道" },
                new Level() { LevelName = "元婴期", LevelIndex = 4, LevelType = "修仙大道" },
                new Level() { LevelName = "一级妖兽", LevelIndex = 1, LevelType = "妖修之道" },
                new Level() { LevelName = "二级妖兽", LevelIndex = 1, LevelType = "妖修之道" },
                new Level() { LevelName = "三级妖兽", LevelIndex = 1, LevelType = "妖修之道" },
            };
            for (int i = 0; i < levelList.Count; i++)
            {
                var level = levelList[i];
                LevelDic.Add(level.LevelName, level);
            }

            BehaviorDic = new Dictionary<string, PersonBehavior>();
            List<PersonBehavior> behaviorList = new List<PersonBehavior>()
            {
                new PersonBehavior(){ NameID = "渡过飞升雷劫", BasePossibility = 0.1f },
                new PersonBehavior(){ NameID = "凝结元婴", BasePossibility = 0.2f },
                new PersonBehavior(){ NameID = "结成金丹", BasePossibility = 0.3f },
                new PersonBehavior(){ NameID = "凝气筑基", BasePossibility = 0.4f },
                new PersonBehavior(){ NameID = "修炼", BasePossibility = 0.5f },
                new PersonBehavior(){ NameID =  "开采灵石", BasePossibility = 1f},//应该设置行为的结果, Desire的AvailableBehaviorList从所有行为中遍历有对应结果的
            };
            for (int i = 0; i < behaviorList.Count; i++)
            {
                var behavior = behaviorList[i];
                BehaviorDic.Add(behavior.NameID, behavior);
            }

            ItemDic = new Dictionary<string, Item>();
            List<Item> itemList = new List<Item>()
            {
                new Item(){NameID = "灵石"},
                new Item(){ NameID = "筑基丹"},
                new Item(){NameID = "金灵石"},
                new Item(){NameID = "木灵石"},
                new Item(){NameID = "水灵石"},
                new Item(){NameID = "火灵石"},
                new Item(){NameID = "土灵石"},
            };
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                ItemDic.Add(item.NameID, item);
            }
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

        void initBehaviorConditions()
        {
            BehaviorDic["凝气筑基"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["炼气期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["筑基丹"],1, true), IsNecessary = false, PossiblityMulti = 1.3f },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],100, true), IsNecessary = false, PossiblityMulti = 1.3f },
            };
            BehaviorDic["结成金丹"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["筑基期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["金髓丸"],1, true), IsNecessary = false, PossiblityMulti = 1.3f },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],1000, true), IsNecessary = false, PossiblityMulti = 1.3f },
            };
            BehaviorDic["凝结元婴"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["金丹期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["凝婴丹"],1, true), IsNecessary = false, PossiblityMulti = 1.3f },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],10000, true), IsNecessary = false, PossiblityMulti = 1.3f },
            };
            BehaviorDic["渡过飞升雷劫"].ConditionList = new List<Condition>(){
                new Condition(){ TheDesire = DesireDic["元婴期"] , IsNecessary = true },
                new Condition(){ TheDesire = new ItemDesire(ItemDic["灵石"],100000, true), IsNecessary = false, PossiblityMulti = 1.3f },
                new Condition(){ TheDesire = new MultiDesire(){ ChildList=new List<Desire>(){
                        new ItemDesire(ItemDic["金灵石"],1, true),
                        new ItemDesire(ItemDic["木灵石"],1, true),
                        new ItemDesire(ItemDic["水灵石"],1, true),
                        new ItemDesire(ItemDic["火灵石"],1, true),
                        new ItemDesire(ItemDic["土灵石"],1, true),
                    } }, IsNecessary = false, PossiblityMulti = 1.3f},
            };
        }

        void initDesireAvailableBehaviorList()
        {
            //对境界的需求, 可由单人的独立行为完成
            //对物品的需求, 可能会产生多人行为(索要,购买,抢夺等)
            //AvailableBehaviorList = new List<PersonBehavior>() {
            //    BehaviorDic["凝气筑基"]
            //}
            DesireDic["筑基期"].AvailableBehaviorList = new List<PersonBehavior>(){
                BehaviorDic["凝气筑基"]
            };
            DesireDic["金丹期"].AvailableBehaviorList = new List<PersonBehavior>(){
                BehaviorDic["结成金丹"]
            };
            DesireDic["元婴期"].AvailableBehaviorList = new List<PersonBehavior>(){
                BehaviorDic["凝结元婴"]
            };
            DesireDic["渡过飞升雷劫"].AvailableBehaviorList = new List<PersonBehavior>(){
                BehaviorDic["渡过飞升雷劫"]
            };


        }

        public class Condition
        {
            public Desire TheDesire;
            public bool IsNecessary;//是否必要条件
            public float PossiblityMulti;//可能性加成 1.1 = 10%
        }

        public class PersonBehavior : INameID
        {
            public string NameID { get; set; }
            public List<Condition> ConditionList;
            public float BasePossibility = 0.5f;
            public Dictionary<Result, float> ResultDic;//TODO 如何描述互斥结果?
        }

        public class Result
        {
            public List<Desire> DesireList;
        }

        public class Level : INameID
        {
            public string NameID { get { return LevelType + "-" + LevelName; } }
            public string LevelType;
            public string LevelName;
            public int LevelIndex;
        }

        //与某人切磋, 拜访某人, 飞升(通过具体事件完成)
        //达到某种境界(LevelDesire通过Level满足完成)
        //拥有某些物品(ItemDesire通过拥有物品完成)
        //拜某人为师, (RelationDesire关系状态)
        //成为门主(PositionDesire)

        //TODO 考虑不欲模式, 行为-结果中包含负面结果, 由此带来对行为选择偏好改变

        //desire来源: 
        //1.初衷/本心(修仙大道, 剑道造诣, 炼丹造诣, 长生, 逍遥, 权势)
        //2.当前遭遇/状态(情仇, 负伤)
        public class Desire : INameID
        {
            public static void Initilize()
            {
                //飞升 需要:行为-渡过飞升天劫
                //行为-渡过飞升天劫 需要  Condition-境界[元婴后期]大圆满
                //Condition-境界[元婴后期]大圆满 需要 境界[元婴后期] + 提升法力

                //行为=>结果? 结果=需求?
                //行为的前置条件 是 结果?/需求?/其他

                //筑基-结丹-元婴-飞升

                //var finalDesire = new Desire()
                //{
                //    DesireName = "飞升仙界",
                //    AvailableBehaviorList = new List<PersonBehavior> {
                //        new PersonBehavior(){
                //            BehaviorName = "渡过飞升雷劫",
                //            ConditionList = new List<Desire>{
                //                new LevelDesire() {
                //                    TheLevel = new Level(){ LevelName = "元婴期" },
                //                    AvailableBehaviorList = new List<PersonBehavior>(){
                //                        new PersonBehavior(){
                //                            BehaviorName =  "凝结元婴" ,
                //                            ConditionList = new List<Desire>(){
                //                                new LevelDesire() {
                //                                    TheLevel = new Level() { LevelName = "结丹期"} ,
                //                                    AvailableBehaviorList = new List<PersonBehavior>(){ new PersonBehavior() {
                //                                        BehaviorName = "结成金丹",
                //                                        ConditionList = new List<Desire>(){ new LevelDesire() { TheLevel = new Level() { LevelName = "筑基期"} } }
                //                                    } }
                //                                } } }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //};

            }

            public virtual string NameID { get { return "欲求"; } }
            public List<PersonBehavior> AvailableBehaviorList;
        }

        public class MultiDesire : Desire
        {
            public List<Desire> ChildList;
            //TODO override NameID
        }

        public class BehaviorDesire : Desire
        {
            public PersonBehavior TheBehavior;
            public override string NameID
            {
                get
                {
                    return TheBehavior.NameID;
                }
            }
            public BehaviorDesire()
            {

            }
            public BehaviorDesire(PersonBehavior behavior)
            {
                TheBehavior = behavior;
            }
        }

        public class LevelDesire : Desire
        {
            public Level TheLevel;
            public override string NameID
            {
                get
                {
                    return TheLevel.LevelName;
                }
            }
            public LevelDesire(Level level)
            {
                TheLevel = level;
            }
        }

        public class ItemDesire : Desire
        {
            public Item TheItem;
            public int Num;
            public bool Consume;
            public override string NameID
            {
                get
                {
                    return TheItem.NameID + " " + Num;
                }
            }
            public ItemDesire(Item item, int num, bool consume)
            {
                TheItem = item;
                Num = num;
                Consume = consume;
            }
        }

        public class Item : INameID
        {
            public string NameID { get; set; }
        }

        public class DesireSystem
        {
            public Person ThePerson;
            public Desire FinalDesire;

            public void Initilize()
            {
                //FinalDesire = new Desire() { DesireName = "飞升"};
            }

            public Desire GetCurrentDesire()
            {
                return null;
            }
        }

        public class PersonalSystem
        {
            public Person ThePerson;

            public void Initilize()
            {

            }

            public PersonBehavior GetBehaviorByDesire(Desire desire)
            {
                return null;//TODO
            }
        }


        public enum EnumSex { Male, Female }

        public class Person
        {
            public string Name;
            public int Age;
            public EnumSex Sex;
            public DesireSystem TheDesireSystem;
            public PersonalSystem ThePersonalSystem;
            public Level TheLevel;

            public Person Initilize()
            {
                TheDesireSystem = new DesireSystem() { ThePerson = this };
                ThePersonalSystem = new PersonalSystem() { ThePerson = this };
                return this;
            }

            public void UpdateGameLogic()
            {

            }
        }

        public class StoryBase
        {

        }


        public class Story : StoryBase
        {
            public string When;
            public string Where;
            public string Who;
            public string What;
            public string Why;
        }


        public class XiuXianStory : MonoBehaviour
        {
            public int Day = 1;
            public List<Person> PersonList;


            private void Start()
            {
                PersonList.Add(new Person() { Name = "王立", Age = 50, Sex = EnumSex.Male, TheLevel = new Level() { LevelName = "结丹期" } }.Initilize());
                PersonList.Add(new Person() { Name = "李寻仙", Age = 20, Sex = EnumSex.Male, TheLevel = new Level() { LevelName = "结丹期" } }.Initilize());
                PersonList.Add(new Person() { Name = "张玉", Age = 18, Sex = EnumSex.Female, TheLevel = new Level() { LevelName = "结丹期" } }.Initilize());
            }

            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    updateGameLogic();
                }
            }

            void updateGameLogic()
            {

            }

        }
    }
}