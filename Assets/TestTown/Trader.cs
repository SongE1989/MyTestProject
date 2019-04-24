using UnityEngine;
using System.Collections;
using System;

namespace testTownNS
{
    public class Trader: IItemOwnner
    {
        public Location CurLocation;
        public string Name;

        ItemContainer _items = new ItemContainer();
        public ItemContainer Items
        {
            get { return _items; }
        }


    }
}