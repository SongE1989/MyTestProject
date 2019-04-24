using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISystem_Backpack : MonoBehaviour
{
    public RectTransform UI_SelfRect;
    public GridLayoutGroup UI_Grid;
    public Dictionary<Vector2Int, UIComp_Slot> SlotDic;
    public HashSet<UIComp_Item> ItemSet;
    public RectTransform ItemContainer;

    public ContainerVO mContainerVO;

    public void SetData(ContainerVO container)
    {
        mContainerVO = container;
        //创建Slot
        float width = UI_Grid.padding.left + UI_Grid.padding.right
            + UI_Grid.cellSize.x * mContainerVO.Size.x
            + (UI_Grid.spacing.x * mContainerVO.Size.x - 1) + 1;
        float height = UI_Grid.padding.top + UI_Grid.padding.bottom
            + UI_Grid.cellSize.y * mContainerVO.Size.y
            + (UI_Grid.spacing.y * mContainerVO.Size.y - 1) + 1;
        UI_SelfRect.sizeDelta = new Vector2(width, height);
        var slotPrefab = Resources.Load("XiuXian/UI/bag/UIComp_BackpackSlot") as GameObject;
        var slotNum = mContainerVO.Size.x * mContainerVO.Size.y;
        SlotDic = new Dictionary<Vector2Int, UIComp_Slot>();
        for (int iy = 0; iy < mContainerVO.Size.y; iy++)
        {
            for (int ix = 0; ix < mContainerVO.Size.x; ix++)
            {
                var slot = Instantiate(slotPrefab);
                //slot.transform.parent = UI_Grid.transform;
                slot.transform.SetParent(UI_Grid.transform);
                slot.name = ix + "_" + iy;
                var slotComp = slot.GetComponent<UIComp_Slot>();
                SlotDic.Add(new Vector2Int(ix, iy), slotComp);
                slotComp.OnItemDropCallback = onItemDropSlot;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(UI_Grid.transform as RectTransform);

        //插入Item
        ItemSet = new HashSet<UIComp_Item>();
        var itemPrefab = Resources.Load("XiuXian/UI/bag/UIComp_BackpackItem") as GameObject;
        foreach (var pair in mContainerVO.ItemDic)
        {
            var itemGO = Instantiate(itemPrefab) as GameObject;
            itemGO.transform.SetParent(ItemContainer);
            //Debug.Log(slot.name + ","+ slot.transform.position);
            //item.transform.position = slot.transform.position;
            var comp = itemGO.GetComponent<UIComp_Item>();
            ItemSet.Add(comp);
            comp.SetData(pair.Value);

            var slot = SlotDic[new Vector2Int(pair.Key.x, pair.Key.y)];
            comp.SetPosBySlot(slot);
        }
    }

    void onItemDropSlot(UIComp_Slot slot, UIComp_Item item)
    {
        foreach (var pair in SlotDic)
        {
            if (pair.Value == slot)
            {
                var sucFlag = mContainerVO.MoveItemTo(pair.Key, item.mItemVO);
                if (sucFlag)
                {
                    //item.transform.position = slot.transform.position;
                    item.SetPosBySlot(slot);
                    Debug.Log(slot.name + "," + slot.transform.position);
                }
                else
                    item.CancelDrag();
                break;
            }
        }
        //refreshItemPos();
    }

    //void refreshItemPos()
    //{
    //    foreach (var pair in ItemSet)
    //    {
    //        pair
    //    }
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnBeginDrag"+eventData.pointerDrag);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnDrag" + eventData.pointerDrag);
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnEndDrag" + eventData.selectedObject);
    //    Debug.Log("OnEndDrag" + eventData.pointerCurrentRaycast + "," + eventData.pointerDrag);
    //}

}
