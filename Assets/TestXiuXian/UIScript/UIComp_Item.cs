using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIComp_Item : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public RectTransform SelfRect;
    public RawImage ItemIcon;
    public Text ItemName;
    public CanvasGroup mCanvasGroup;

    public ItemVO mItemVO;

    public void SetData(ItemVO item)
    {
        if (item == null)
            return;
        mItemVO = item;
        ItemIcon.texture = (Texture2D)Resources.Load(mItemVO.ItemIconPath);
        ItemName.text = mItemVO.ItemName;
    }

    #region drag
    Transform orignParent;
    //Vector3 orignLocalPosition;
    public bool IsDragging;
    UIComp_Slot oriSlot;

    public void SetPosBySlot(UIComp_Slot slot)
    {
        oriSlot = slot;
        transform.position = slot.transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        orignParent = SelfRect.parent;
        //orignLocalPosition = SelfRect.localPosition;
        SelfRect.SetParent(UIManager.Instance.DraggingPanel);
        IsDragging = true;
        mCanvasGroup.blocksRaycasts = false;
        setDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        setDraggedPosition(eventData);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        //TODO 整理此处
        //setDraggedPosition(eventData);

        if(eventData.pointerEnter != null)
        {
            var slot = eventData.pointerEnter.GetComponent<UIComp_Slot>();
            if(slot != null)
            {
                SelfRect.SetParent(orignParent);
                slot.OnItemDropCallback(slot, this);
                FinishDrag();
                return;
            }

        }

        if (IsDragging)//若拖拽事件未被其他系统处理并结束, 则恢复位置
        {
            CancelDrag();
        }
    }

    public void CancelDrag()
    {
        SelfRect.SetParent(orignParent);
        SetPosBySlot(oriSlot);
        //SelfRect.localPosition = orignLocalPosition;
        FinishDrag();
    }

    public void FinishDrag()
    {
        IsDragging = false;
        mCanvasGroup.blocksRaycasts = true;
    }

    Vector3 globalMousePos;
    void setDraggedPosition(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(SelfRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            SelfRect.position = globalMousePos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter:" + eventData.pointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit:" + eventData.pointerEnter);
    }
    #endregion

}
