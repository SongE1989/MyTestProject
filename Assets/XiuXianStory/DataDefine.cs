using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XiuXianStory
{
    public interface INameID
    {
        string NameID { get; }
    }

    public class Condition
    {
        public Desire TheDesire;
        public bool IsNecessary;//是否必要条件
        /// <summary>目标结果组-提升几率</summary>
        public List<Buff> BuffList;

        public Condition AddBuff(Buff buff)
        {
            if (BuffList == null)
                BuffList = new List<Buff>();
            BuffList.Add(buff);
            return this;
        }

        public override string ToString()
        {
            return TheDesire.NameID;
        }
    }

    /// <summary>作为行为实体, 记录行为状态执行状态, 执行目标等)</summary>
    public class PersonBehaviorProxy
    {
        public PersonBehavior TargetBehavior;
        public int DayUsed = 0;
        public bool IsFinish = false;
        public ResultGroup TheResultGroup = null;

        public string ToStory()
        {
            //TODO 加入其它状态
            string resultStr = "";
            if (TheResultGroup != null)
            {
                resultStr = "\r\n结果:\r\n";
                for (int i = 0; i < TheResultGroup.ResultList.Count; i++)
                {
                    var res = TheResultGroup.ResultList[i];
                    resultStr += res.ToString() + "\r\n";
                }
            }
            return "做着" + TargetBehavior.NameID + "一事已有" + DayUsed + "天,共需" + TargetBehavior.UseTime + "天" + resultStr;
        }

        public float GetMessageLevel()
        {
            if (TheResultGroup == null)
                return TargetBehavior.MessageLevel;//NOTE 若有特殊的TargetBehavior(例如尝试突破境界), 可以返回更高的Level
            else
                return TargetBehavior.MessageLevel + TheResultGroup.MessageLevelDelta;

        }


        public override string ToString()
        {
            return TargetBehavior.NameID;
        }
    }

    /// <summary>作为一种行为的抽象概念</summary>
    public class PersonBehavior : INameID
    {
        public string NameID { get; set; }
        public List<Condition> ConditionList;
        /// <summary>结果组-权值</summary>
        public Dictionary<ResultGroup, float> ResultGroupDic;
        public float UseTime = 1;
        public override bool Equals(object obj)
        {
            return NameID == ((PersonBehavior)obj).NameID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public float MessageLevel = 0;

        public override string ToString()
        {
            return NameID;
        }
    }

    public class ResultGroup
    {
        public string GroupName;
        /// <summary>本结果是否看做行为成功完成</summary>
        public bool RecordAsBehaviorDone = true;
        public List<Result> ResultList = new List<Result>();
        //TODO 对ResultGroup标记是否受某种属性加成(例如采集加成 X1.05)
        /// <summary>在行为MessageLevel基础上, 结果带来的信息重要性变动, 例如进阶成功+0.5, 突破成功+1.0</summary>
        public float MessageLevelDelta = 0;
    }


    //TODO 如何简化描述?
    public class Result//对象 Person, 门派, 地点, 其他
    {

        public HealthResult AsHealthResult { get { return this as HealthResult; } }
        public LevelResult AsLevelChangeResult { get { return this as LevelResult; } }
        public ItemResult AsItemChangeResult { get { return this as ItemResult; } }
        public AttrResult AsDataResult { get { return this as AttrResult; } }
    }

    public class HealthResult : Result
    {
        public string Effect;

        public override string ToString()
        {
            return "身体状况发生变化: " + Effect;
        }
    }

    public class LevelResult : Result
    {
        public Level TargetLevel;
        public override string ToString()
        {
            return "等级发生变化: " + TargetLevel.LevelName;
        }

    }

    public class AttrResult:Result
    {
        public string AttrName;
        public float DeltaValue;
        public override string ToString()
        {
            return "属性变化: "+AttrName + DataHelper.GetChangeString(DeltaValue);
        }

        public AttrResult(string dataName, float deltaValue)
        {
            AttrName = dataName;
            DeltaValue = deltaValue;
        }

        public AttrResult(string dataName, int deltaValue)
        {
            AttrName = dataName;
            DeltaValue = deltaValue;
        }

    }

    public class ItemResult : Result
    {
        public Dictionary<Item, int> ItemDic;
        public override string ToString()
        {
            return "物品发生变化:\r\n" + DicHelper.GetBagInfo(ItemDic, true);
        }
    }

    public interface ILevel
    {
        Level TheLevel { get; set; }
    }

    public struct Level : INameID
    {
        public string NameID { get { return LevelName; } }
        public string LevelType;
        public string LevelName;
        public int LevelIndex;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return NameID == ((Level)obj).NameID;
        }

        public static bool operator ==(Level lhs, Level rhs)
        {
            return lhs.NameID == rhs.NameID;
        }
        public static bool operator !=(Level lhs, Level rhs)
        {
            return lhs.NameID != rhs.NameID;
        }
        //public static bool operator >(Level a, Level b)
        //{
        //    if (a.LevelType == b.LevelType)
        //        return a.LevelIndex > b.LevelIndex;
        //    return false;
        //}
        //public static bool operator <(Level a, Level b)
        //{
        //    if (a.LevelType == b.LevelType)
        //        return a.LevelIndex < b.LevelIndex;
        //    return false;
        //}
        //public static bool operator >=(Level a, Level b)
        //{
        //    if (a.LevelType == b.LevelType)
        //        return a.LevelIndex >= b.LevelIndex;
        //    return false;
        //}
        //public static bool operator <=(Level a, Level b)
        //{
        //    if (a.LevelType == b.LevelType)
        //        return a.LevelIndex <= b.LevelIndex;
        //    return false;
        //}
    }

    public struct Item : INameID
    {
        public string NameID { get; set; }
    }

    public class StoryBase
    {

    }

    public class Story : StoryBase
    {
        public int When;
        public string Where;
        public string Who;
        public string What;
        public string Why;
        public float MessageLevel;

        public override string ToString()
        {
            return When + "日, 于" + Where + ", " + Who + " 由于" + Why + ",而" + What;
        }
    }

    public class BodySystem
    {
        public BodyPart MainBody;
    }

    public class BodyPart
    {
        public string PartName;
        public Dictionary<string, BodyPart> ChildPart = new Dictionary<string, BodyPart>();

        public BodyPart this[string partName]
        {
            get
            {
                return ChildPart[partName];
            }
        }

        public BodyPart()
        {

        }

        public BodyPart(string partName)
        {
            PartName = partName;
        }

        public BodyPart AddChild(string partName)
        {
            ChildPart.Add(partName, new BodyPart(partName));
            return this;
        }
    }

    public class Magic : INameID
    {
        public string NameID { get; set; }
        public float ManaCost;
        //public int TimeCose;//TODO 引入施法时间, 引入BUFF技能, 与BUFF系统
    }

    /// <summary>飞弹法术(包括盾)</summary>
    public class BulletMagic : Magic
    {
        public float Damage;
        public float MaxHP;
        public float CurHP;
        public float Size;
    }

    public class ManaSystem
    {
        public float ManaPool;
        public float ManaRegain;
        public float ManaCapactiy;

        public ManaSystem()
        {

        }
        public ManaSystem(float pool, float regain, float capacity)
        {
            ManaPool = pool;
            ManaRegain = regain;
            ManaCapactiy = capacity;
        }
        public void Set(float pool, float regain, float capacity)
        {
            ManaPool = pool;
            ManaRegain = regain;
            ManaCapactiy = capacity;
        }
    }

}