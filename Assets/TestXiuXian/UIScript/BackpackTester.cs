using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackTester : MonoBehaviour
{
    private void Start()
    {
        var backpackUI = GetComponent<UISystem_Backpack>();
        ContainerVO cont = new ContainerVO();
        cont.Init(new Vector2Int(4, 5));

        //果实
        cont.AddItemTo(Vector2Int.zero, 
            new ItemVO()
            {
                ItemID = 1,
                ItemName = "果实1",
                ItemType = EnumItemType.Consumable,
                Size = Vector2Int.one,
                ItemIconPath = "XiuXian/UI/icons/TVB1360",
                Stackable = true,
                MaxStackNum = 5,
                CurStackNum = 1
            }
        );
        cont.AddItemTo(Vector2Int.right,
            new ItemVO()
            {
                ItemID = 2,
                ItemName = "草药",
                ItemType = EnumItemType.Consumable,
                Size = Vector2Int.one,
                ItemIconPath = "XiuXian/UI/icons/TVB1246",
                Stackable = true,
                MaxStackNum = 5,
                CurStackNum = 1
            }
        );



        backpackUI.SetData(cont);
    }
}
