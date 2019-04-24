using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>物品容器系统(背包/装备系统)</summary>
public class ContainerSystem
{
    public List<ContainerVO> ContainerList;

    public virtual bool TryAddItem(ItemVO item)
    {
        return false;
    }
}

/// <summary>容器</summary>
public class ContainerVO
{
    /// <summary>可接收的类型</summary>
    public EnumItemType AcceptItemType;
    public Dictionary<Vector2Int, ItemVO> ItemDic = new Dictionary<Vector2Int, ItemVO>();
    public Dictionary<ItemVO, Vector2Int> PosDic = new Dictionary<ItemVO, Vector2Int>();

    public Vector2Int Size;
    public bool[,] PlaceHolder;

    public void Init(Vector2Int _size, EnumItemType _accept = EnumItemType.All)
    {
        Size = _size;
        PlaceHolder = new bool[Size.x, Size.y];
    }

    public virtual bool ContainsItem(ItemVO item)
    {
        return ItemDic.ContainsValue(item);
    }
    
    /// <summary>TODO自动填入物品</summary>
    public virtual bool TryAddItem(ItemVO item)
    {
        //TODO 检查可堆叠

        return false;
    }

    public virtual void RemoveItem(ItemVO item)
    {
        if(ContainsItem(item))
        {
            Vector2Int pos = PosDic[item];
            PosDic.Remove(item);
            ItemDic.Remove(pos);

            for (int ix = 0; ix < item.Size.x; ix++)
            {
                for (int iy = 0; iy < item.Size.y; iy++)
                {
                    PlaceHolder[pos.x + ix, pos.y + iy] = false;
                }
            }
        }
    }

    public virtual bool CheckPosEmpty(Vector2Int pos, Vector2Int size)
    {
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (PlaceHolder[pos.x + ix, pos.y + iy])
                    return false;
            }
        }
        return true;
    }

    public virtual bool AddItemTo(Vector2Int pos, ItemVO item)
    {
        //TODO 检查堆叠 
        if (CheckPosEmpty(pos, item.Size))
        {
            ItemDic.Add(pos, item);
            PosDic.Add(item, pos);
            for (int ix = 0; ix < item.Size.x; ix++)
            {
                for (int iy = 0; iy < item.Size.y; iy++)
                {
                    PlaceHolder[pos.x + ix, pos.y + iy] = true;
                }
            }
            return true;
        }
        else
            return false;
    }

    /// <summary>背包内移动</summary>
    public virtual bool MoveItemTo(Vector2Int pos, ItemVO item)
    {
        //TODO 两物品位置对换
        if(!ContainsItem(item))
        {
            Debug.LogError("CantMoveItem, donst contains it");
            return false;
        }
        Vector2Int oldPos = PosDic[item];


        Stack<Vector2Int> changedPos = new Stack<Vector2Int>();

        for (int ix = 0; ix < item.Size.x; ix++)
        {
            for (int iy = 0; iy < item.Size.y; iy++)
            {
                PlaceHolder[oldPos.x + ix, oldPos.y + iy] = false;
                changedPos.Push(new Vector2Int(oldPos.x + ix, oldPos.y + iy));
            }
        }

        for (int ix = 0; ix < item.Size.x; ix++)
        {
            for (int iy = 0; iy < item.Size.y; iy++)
            {
                if (!PlaceHolder[pos.x + ix, pos.y + iy])
                {
                    PlaceHolder[pos.x + ix, pos.y + iy] = true;
                    changedPos.Push(new Vector2Int(pos.x + ix, pos.y + iy));
                }
                else
                {
                    //失败 回滚操作
                    while (changedPos.Count > 0)
                    {
                        var p = changedPos.Pop();
                        PlaceHolder[p.x, p.y] = !PlaceHolder[p.x, p.y];
                    }
                    return false;
                }
            }
        }
        PosDic[item] = pos;
        return true;//顺利迁移完成
    }
}


