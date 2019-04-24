using UnityEngine;
using System.Collections;

public class TradeManager
{

    //static TradeManager _instance;
    //public static TradeManager Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = new TradeManager();
    //        }
    //        return _instance;
    //    }
    //}

    #region Buy&Sell

    /// <summary>check seller has enough item & buyer has enough money</summary>
    public static bool CheckCanTrade(IItemOwnner seller, IItemOwnner buyer, string itemName, float price, float num)
    {
        return CheckCanTrade(seller.Items, buyer.Items, itemName, price, num);
    }

    /// <summary>check seller has enough item & buyer has enough money</summary>
    public static bool CheckCanTrade(ItemContainer seller, ItemContainer buyer, string itemName, float price, float num)
    {
        if (price <= 0)
            return false;
        return seller.ItemDic.GetValue(itemName) >= num && buyer.ItemDic.GetValue(itemName) >= num * price;
    }

    public static bool TryTrade(IItemOwnner seller, IItemOwnner buyer, string itemName, float price, float num, bool checkCanTrade = true)
    {
        return TryTrade(seller.Items, buyer.Items, itemName, price, num);
    }

    public static bool TryTrade(ItemContainer seller, ItemContainer buyer, string itemName, float price, float num, bool checkCanTrade = true)
    {
        if (!checkCanTrade || CheckCanTrade(seller, buyer, itemName, price, num))
        {
            seller.ChangeMoney(num * price);
            seller.ChangeItem(itemName, -num);
            buyer.ChangeMoney(-num * price);
            buyer.ChangeItem(itemName, num);
            return true;
        }
        else
            return false;
    }

    #endregion

    /// <summary>按供需获取价格偏移系数</summary>
    public static float GetPriceBiasBySupplyDemand(float supply, float demand)
    {
        //1/3~3
        float min = 0.3f;
        float max = 3;
        if (supply == 0 && demand != 0)
            return max;
        else if (supply != 0 && demand == 0)
            return min;
        else
            return Mathf.Clamp(demand / supply, min, max);
    }
}
