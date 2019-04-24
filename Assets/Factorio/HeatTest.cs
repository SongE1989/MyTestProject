using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeatTest : MonoBehaviour
{
    Dictionary<string, float> heatDic = new Dictionary<string, float>() {
        {"A",5/4f },
        {"B",5/2f },
        {"C",5/4f },
        {"D",8f }
    };

    [ContextMenu("测试燃值")]
    void testHeat()
    {
        Debug.Log(ss1());
        Debug.Log(ss2());
        Debug.Log(ss3A());
        Debug.Log(ss3B());
    }

    float heatA = 5 / 4f;
    float heatB = 5 / 2f;
    float heatC = 5 / 4f;
    float heatD = 8f;

    float A2B = 3 / 4f;
    float B2C = 2 / 3f;
    
    float ss1()
    {
        return (30f *A2B +30f) * heatB + 30 *heatC;
    }

    float ss2()
    {
        return (10 * A2B + 45) * heatB + 55 * heatC;
    }

    float ss3A()
    {
        return 10 * heatD + 25 * A2B * heatB;
    }

    float ss3B()
    {
        return (35 * A2B + 15) * heatB + 20 * heatC;
    }

}
