using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiuXianStory
{
    public class BattleManager
    {
        /*
        1V1战斗 
        回合制 导出战斗记录 人物战斗对话


        */

        public static Battle StartCombat(Person p1, Person p2)
        {
            var combat = new Battle() { P1 = p1, P2 = p2};
            p1[Attr.CurHP] = p1[Attr.MaxHP];
            p2[Attr.CurHP] = p2[Attr.MaxHP];
            return combat;
        }
    }

    public class BattleMessage
    {
        public string Msg;
        public BattleMessage()
        {
        }
        public BattleMessage(string msg)
        {
            Msg = msg;
        }
        public override string ToString()
        {
            return Msg;
        }
    }

    public class DamageMessage : BattleMessage
    {
        public Person Src;
        public Person Tar;
        public Magic TheMagic;
        public float Result;
        public DamageMessage(Person src, Person tar, Magic magic, float result)
        {
            Src = src;
            Tar = tar;
            TheMagic = magic;
            Result = result;
            Msg = Src.HPState + " 对 " + Tar.HPState + " 释放 " + TheMagic.NameID + ", 造成" + Result + "点伤害";
        }
    }

    public class Battle
    {
        public int Round = 0;
        public Person P1;
        public Person P2;

        public List<List<BattleMessage>> CombatMessageList = new List<List<BattleMessage>>();
        public Action OnBattleOver;
        bool isOver = false;

        public void UpdateLogic()
        {
            if (isOver)
                return;
            Round++;

            //NOTE TODO 加入技能选择AI, 战斗AI
            var magic1 = pickMagic(P1) as BulletMagic;
            var magic2 = pickMagic(P1) as BulletMagic;

            var collisionRate = 1f - magic1.Size - magic2.Size;
            collisionRate = Mathf.Max(0, collisionRate);
            if (UnityEngine.Random.Range(0f, 1f) <= collisionRate)//飞弹对冲
            {
                //飞弹减少多少血量, 最终造成伤害也成比例减少
                magic1.CurHP -= magic2.Damage;
                magic2.CurHP -= magic1.Damage;
                dealDamage(P1, P2, magic1);
                dealDamage(P2, P1, magic2);
            }
            else
            {
                dealDamage(P1, P2, magic1);
                dealDamage(P2, P1, magic2);
            }

            if (P1[Attr.CurHP] <= 0)
            {
                addMsg(new BattleMessage(P1.NameID + "失去意识!"));
                isOver = true;
            }
            if (P2[Attr.CurHP] <= 0)
            {
                addMsg(new BattleMessage(P2.NameID + "失去意识!"));
                isOver = true;
            }
            if (isOver)
            {
                addMsg(new BattleMessage("战斗结束"));
                if(OnBattleOver != null)
                    OnBattleOver();
            }
        }

        void dealDamage(Person src, Person tar, BulletMagic magic)
        {
            var damage = magic.Damage * (magic.CurHP / magic.MaxHP);
            if(damage > 0)
            {
                tar[Attr.CurHP] -= damage;
                addMsg(new DamageMessage(src, tar, magic, damage));
            }
            else
            {
                addMsg(new BattleMessage(src.HPState + " 对 " + tar.HPState + " 释放 " + magic.NameID + ", 未能造成伤害"));
            }
        }

        Magic pickMagic(Person person)
        {
            var magicIndex = UnityEngine.Random.Range(0, person.LearnedMagicList.Count);
            var magic = person.LearnedMagicList[magicIndex] as BulletMagic;
            magic.CurHP = magic.MaxHP;
            return magic;
        }

        void addMsg(BattleMessage msg)
        {
            if(CombatMessageList.Count < Round)
            {
                CombatMessageList.Add(new List<BattleMessage>());
            }
            CombatMessageList[Round - 1].Add(msg);
        }

        public string GetCurrentMessage()
        {
            if (Round <= 0)
                return "战斗未开始";
            else
            {
                string msg = P1.NameID + " 对 "+P2.NameID+ " 回合" +Round+"\r\n";
                var msgList = CombatMessageList[Round - 1];
                for (int i = 0; i < msgList.Count; i++)
                {
                    msg += msgList[i].ToString() + "\r\n";
                }
                return msg;
            }
        }

        public string GetAllMessage()
        {
            if (Round <= 0)
                return "战斗未开始";
            else
            {
                string msg = P1.NameID + " 对 " + P2.NameID+"\r\n";
                for (int round = 0; round < CombatMessageList.Count; round++)
                {
                    //+" 回合" + Round +
                    msg += "回合" + (round +1)+ "\r\n";
                    var msgList = CombatMessageList[round];
                    for (int i = 0; i < msgList.Count; i++)
                    {
                        msg += msgList[i].ToString() + "\r\n";
                    }
                }
                return msg;
            }
        }
    }

}