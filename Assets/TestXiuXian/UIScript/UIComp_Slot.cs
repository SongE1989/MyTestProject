using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIComp_Slot : MonoBehaviour
{
    public Action<UIComp_Slot,UIComp_Item> OnItemDropCallback;
    public void OnItemDrop(PointerEventData eventData)
    {
        Debug.Log("OnItemDrop:"+name);
        var item = eventData.pointerDrag.GetComponent<UIComp_Item>();
        if (item != null)
        if (OnItemDropCallback != null)
            OnItemDropCallback(this, item);
    }
}
