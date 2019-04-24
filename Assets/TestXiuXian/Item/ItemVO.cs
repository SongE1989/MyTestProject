using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EnumItemType
{
    None = 0,
    Weapon = 1,
    Equip = 1 << 1,
    Consumable = 1 << 2,
    Container = 1 << 3,
    All = 1 << 32 - 1//暂定32种
}

//public class ItemCfg
//{
//    public uint ItemID;
//    public EnumItemType ItemType;
//    public string ItemName;
//}

//public class ItemCongimanager
//{
//    static ItemCongimanager _instance;
//    public static ItemCongimanager Instance
//    {
//        get {
//            if (_instance == null)
//                _instance = new ItemCongimanager();
//            return _instance;
//        }
//    }


//}

public class ItemFactory
{
    
}


public class ItemVO
{
    public uint ItemID;
    public EnumItemType ItemType;
    public string ItemName;
    public string ItemIconPath;

    //Stack
    public bool Stackable;
    public uint MaxStackNum;
    public uint CurStackNum;

    public Vector2Int Size;

    //private ItemCfg itemConfig;
    //public ItemCfg ItemConfig
    //{
    //    get
    //    {
    //        if(itemConfig == null)
    //        {

    //        }
    //        return itemConfig;
    //    }
    //}

    //public void Reuse()
    //{
    //    ItemID = 0;
    //    itemConfig = null;
    //}
}

