using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public RectTransform DraggingPanel;
    public Canvas mCanvas;

    private void Awake()
    {
        Instance = this;
    }

    //public void OnDrop(PointerEventData eventData)
    //{
    //    //Canvas xx;xx.cam
    //    //Debug.Log("UIManager.OnDrop" + eventData.selectedObject+", "+eventData.pointerCurrentRaycast);
    //    //var screenPoint = RectTransformUtility.WorldToScreenPoint(mCanvas.worldCamera,  eventData.pointerPressRaycast.worldPosition);
        
    //    for (int i = 0,length = eventData.hovered.Count; i < length; i++)
    //    {
    //        var slot = eventData.hovered[i].GetComponent<UIComp_Slot>();
    //        if(slot != null)
    //        {
    //            slot.OnItemDrop(eventData);
    //            break;
    //        }
    //    }
    //    //eventData.
    //}
}
