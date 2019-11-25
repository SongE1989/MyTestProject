using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiuXianStory
{
    public struct Buff
    {
        public string BuffName;
        public float BuffValue1;
        public float BuffValue2;
        public float BuffValue3;
        public float BuffValue4;
        public Buff(string buffName, float val1, float val2 = 0, float val3 = 0, float val4 = 0)
        {
            BuffName = buffName;
            BuffValue1 = val1;
            BuffValue2 = val1;
            BuffValue3 = val1;
            BuffValue4 = val1;
        }
    }
}