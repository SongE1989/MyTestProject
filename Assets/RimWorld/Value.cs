using UnityEngine;
using System.Collections;
using System;

namespace RimWorldNS
{

    public class Value_int
    {
        public int Max;
        public int Min;
        int curValue;
        public int CurValue
        {
            get
            {
                return curValue;
            }

            set
            {
                if (value < Min && OnValueBelow!=null)
                    OnValueBelow();
                if (value > Max && OnValueAbove!=null)
                    OnValueAbove();
                this.curValue = Mathf.Clamp(value, Min, Max);
            }
        }

        public Action OnValueBelow;
        public Action OnValueAbove;

        public Value_int(int _max, int _min, int _value)
        {
            this.Max = _max;
            this.Min = _min;
            this.CurValue = _value;
            if (_max < _min)
                Debug.LogError("max can not below min! " + Max + "<" + Min);
        }
    }

    public class Value_float
    {
        public float Max;
        public float Min;
        float curValue;
        public float CurValue
        {
            get
            {
                return curValue;
            }

            set
            {
                if (value < Min)
                    OnValueBelow();
                if (value > Max)
                    OnValueAbove();
                this.curValue = Mathf.Clamp(value, Min, Max);
            }
        }

        public Action OnValueBelow;
        public Action OnValueAbove;

        public Value_float(float _max, float _min, float _value)
        {
            this.Max = _max;
            this.Min = _min;
            this.CurValue = _value;
            if (_max < _min)
                Debug.LogError("max can not below min! " + Max + "<" + Min);
        }
    }
}
