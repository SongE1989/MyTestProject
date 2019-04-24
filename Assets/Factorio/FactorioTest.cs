using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactorioTest : MonoBehaviour {
    //NOTE 重大发现 考虑交错布局 ABABAB

    //public string[] allItemList = new string[] {"电路版", "塑料", "铜线", "集成电路板"};

    void initItem()
    {
        List<Factory> factoryList = new List<Factory>();

        factoryList.Add(new Factory("集成电路板", "电路版", "塑料", "铜线"));

        factoryList.Add(new Factory("电路版", "铁板", "铜线"));

        factoryList.Add(new Factory("基础传送带", "铁板", "铁齿轮"));
        //连接
        factoryList.Add(new Factory("电力机械臂", "电路版", "铁板", "铁齿轮"));
        //连接
        factoryList.Add(new Factory("电力采矿机", "电路版", "铁板", "铁齿轮"));

        factoryList.Add(new Factory("内燃机", "刚才", "铁齿轮", "管道"));

        factoryList.Add(new Factory("穿甲弹夹", "制式弹夹", "钢材", "铜板"));

        factoryList.Add(new Factory("科技包1", "铜板", "铁齿轮"));

        factoryList.Add(new Factory("机枪炮塔", "铁板", "铁齿轮", "铜板"));

        factoryList.Add(new Factory("制式手雷", "铁板", "煤矿"));

        //遍历所有排列
        //并在每种排列中, 遍历所有线路排布
        //1+3: C42 12 13 14 23 24 34
        //1+2: C31
        //
    }

	// Use this for initialization
	void Start ()
    {
        int cc = 0;
        int[] IntArr = new int[] { 1, 2, 3, 4, 5,6,7,8,9 }; //整型数组
        List<int[]> ListCombination = PermutationAndCombination<int>.GetPermutation(IntArr, IntArr.Length); //求全部的5取3排列
        foreach (int[] arr in ListCombination)
        {
            string xx = "";
            foreach (int item in arr)
            {
                //Console.Write(item + " ");
                xx += item+" ";
            }
            //Console.WriteLine("");
            //Debug.Log(xx);
            cc++;
        }
        Debug.Log(cc);
    }

    static void PLZH(string[] args)
    {
        int[] IntArr = new int[] { 1, 2, 3, 4, 5 }; //整型数组
        List<int[]> ListCombination = PermutationAndCombination<int>.GetCombination(IntArr, 3); //求全部的3-3组合
        foreach (int[] arr in ListCombination)
        {
            foreach (int item in arr)
            {
                //Console.Write(item + " ");
            }
            //Console.WriteLine("");
        }
        //Console.ReadKey();
    }

    static void PL()
    {
        int[] IntArr = new int[] { 1, 2, 3, 4, 5 }; //整型数组
        List<int[]> ListCombination = PermutationAndCombination<int>.GetPermutation(IntArr, 3); //求全部的5取3排列
        foreach (int[] arr in ListCombination)
        {
            foreach (int item in arr)
            {
                //Console.Write(item + " ");
            }
            //Console.WriteLine("");
        }

    }
}

public class Factory
{
    public string[] dataArr;
    public string output;
    public string src1;
    public string src2;
    public string src3;
    //[O,X,X,X]
    //[X,X,X,O]
    public Factory(string _output, string _src1, string _src2=null, string _src3=null)
    {
        output = _output;
        src1 = _src1;
        src2 = _src2;
        src3 = _src3;
        dataArr = new string[] { };
        dataArr[0] = _output;
        dataArr[1] = _src1;
        dataArr[2] = _src2;
        if(_src3 != null)
            dataArr[3] = _src3;

    }
}
