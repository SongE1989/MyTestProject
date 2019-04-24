using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World170729 : MonoBehaviour
{
    List<Town> TheTownList;
    // Use this for initialization
    void Start()
    {
        TheTownList = new List<Town>();
        GameObject worldFolder = GameObject.Find("World");
        int townCount = worldFolder.transform.childCount;
        for (int i = 0; i < townCount; i++)
        {
            Transform townTran = worldFolder.transform.GetChild(i);
            Town town = townTran.GetComponent<Town>();
            if (town != null)
            {
                town.Init();
                TheTownList.Add(town);
            }

        }
    }

    void startGame()
    {
        //ItemContainer ic = new ItemContainer();
        //ic.ChangeItem("rice", 123);
        //ic.ChangeMoney(20);
        //Debug.Log(ic.ToJsonData().ToJson());

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            updateLogic();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            updateLogic();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            print();
    }

    void print()
    {
        for (int iList = 0, nList = TheTownList.Count; iList < nList; iList++)
        {
            var town = TheTownList[iList];
            Debug.Log(town.ToJsonData().ToJson());
        }
    }

    void updateLogic()
    {
        for (int iList = 0, nList = TheTownList.Count; iList < nList; iList++)
        {
            var town = TheTownList[iList] ;
            town.UpdateLogic();
        }
    }

    //TODO (完成)物品管理器, (完成)物品拥有者, (完成)交易管理器
    //TODO (完成)城镇 (完成)产物 
    //TODO 市场 价格
    //方案1. 需求-供给公式(需要引入消耗系统, 食物的存储系统, 以及价值判断系统)
    //方案2. 产出物作为额外物资, 在市场低价出售(如不引入递减机制, 会导致最佳商路的出现)
}
