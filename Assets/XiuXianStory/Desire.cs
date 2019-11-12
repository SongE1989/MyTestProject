using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XiuXianStory
{
    public class Desire : INameID
    {
        public virtual string NameID { get { return "需求"; } }
        public override bool Equals(object obj)
        {
            return NameID == ((Desire)obj).NameID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        //public bool CanAcheive() { return false; }
        //public bool TryAcheive() { return false; }//TODO should be at PersonSystem

        public MultiDesire AsMultiDesire { get { return this as MultiDesire; } }
        public LevelDesire AsLevelDesire { get { return this as LevelDesire; } }
        public BehaviorDesire AsBehaviorDesire { get { return this as BehaviorDesire; } }
        public ItemDesire AsItemDesire { get { return this as ItemDesire; } }
        public AttrDesire AsAttrDesire { get { return this as AttrDesire; } }

        public override string ToString()
        {
            return NameID;
        }
    }

    /// <summary>加入此类以实现[需求整套道具]的需求</summary>
    public class MultiDesire : Desire
    {
        public List<Desire> ChildList;
        //TODO override NameID
        public string MultiDesireName = "多项需求";
        public override string NameID
        {
            get
            {
                return MultiDesireName;
            }
        }
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
        /// <summary>true:使用(消耗)某物, false拥有某物</summary>
        public bool Consume;
        public override string NameID
        {
            get
            {
                return (Consume? "使用":"拥有")+TheItem.NameID + " " + Num;
            }
        }
        public ItemDesire(Item item, int num, bool consume)
        {
            TheItem = item;
            Num = num;
            Consume = consume;
        }
    }

    public class AttrDesire : Desire
    {
        //tpye 增加(至少)某值, 减少(至少)某值
        public enum AttrDesireType {
            Add = 1,
            Minus = 2,
        }
        public AttrDesireType Type;
        public string AttrName;
        public float TargetValue;

        public AttrDesire(AttrDesireType type, string attrName, float targetValue)
        {
            Type = type;
            AttrName = attrName;
            TargetValue = targetValue;
        }

        string nameID;
        public override string NameID
        {
            get
            {
                return "属性改变: "+AttrName+" "+DataHelper.GetChangeString(TargetValue);
            }
        }

        public bool IsSameKind(Desire desire)
        {
            var tarDesire = desire as AttrDesire;
            return (tarDesire != null && tarDesire.AttrName == AttrName && tarDesire.Type == Type);
        }
    }
}
