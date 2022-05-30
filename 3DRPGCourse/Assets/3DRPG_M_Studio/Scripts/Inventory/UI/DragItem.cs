using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 工具脚本：可以信息拖拽操作的控制脚本
/// </summary>
[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 当前的物品信息相关
    /// </summary>
    ItemUI currentItemUI;

    /// <summary>
    /// 当前的槽位
    /// </summary>
    SlotHoder currentHolder;

    /// <summary>
    /// 目标的槽位
    /// </summary>
    SlotHoder targetHoder;

    /// <summary>
    /// 正在操作
    /// </summary>
    private bool isCaopration = false;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHoder>();
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCaopration) return;
        isCaopration = true;
        //记录原始的拖拽数据
        InventoryManager.Instance.currentDrag = new DragData();
        InventoryManager.Instance.currentDrag.originaHolder = GetComponent<SlotHoder>();
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform) transform.parent;
        //修改正在拖拽物体的父级，主要是用于渲染在最上方
        transform.SetParent(InventoryManager.Instance.DragCavans.transform, true);
    }

    /// <summary>
    /// 跟随鼠标的位置
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// 抬起鼠标，结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //是否处于 UI 上（还是在世界坐标上）
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //是否处于 背包/快捷键/玩家信息面板 的范围内
            if (InventoryManager.Instance.CheckInInventoryUI(eventData.position) ||
                InventoryManager.Instance.CheckInActionUI(eventData.position) ||
                InventoryManager.Instance.CheckInequipmentUI(eventData.position))
            {
                //如果目标点物体有 SlotHoder槽位脚本，拿到这个槽位，或者从父物体拿到槽位（因为槽位的子物体是一个 image）
                Debug.Log(eventData.pointerEnter.gameObject.name);
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHoder>())
                    targetHoder = eventData.pointerEnter.gameObject.GetComponent<SlotHoder>();
                else //这可能是原本的槽位
                    targetHoder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHoder>();

                //判断要放下的槽位是否和原始槽位不是一个
                if (targetHoder != InventoryManager.Instance.currentDrag.originaHolder)
                    //目标的槽位是什么，按照老师的设计，武器槽是不可以放草莓消耗品这样子的
                    switch (targetHoder.slotType)
                    {
                        case SlotType.Bag:
                            SwapItem();
                            break;
                        case SlotType.Weapon:
                            if (currentItemUI.InventoryDataSOType.items[currentItemUI.Index].ItmeSOData.itemType ==
                                ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.Armor:
                            if (currentItemUI.InventoryDataSOType.items[currentItemUI.Index].ItmeSOData.itemType ==
                                ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.Action:
                            if (currentItemUI.InventoryDataSOType.items[currentItemUI.Index].ItmeSOData.itemType ==
                                ItemType.Useable)
                                SwapItem();
                            break;
                    }

                currentHolder.UpdateSlotHoderItem();
                targetHoder.UpdateSlotHoderItem();
            }
        }

        //设置拖拽物品的父级
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);
        RectTransform t = transform as RectTransform;
        //我的跟老师的不一样，我的 SlotHoder 偏移量是 0
        t.offsetMax = Vector2.zero;
        t.offsetMin = Vector2.zero;

        //这里是修改物品的 Rect 偏移值，这里现在是写死的 
        //t.offsetMax = -Vector2.one * 6;
        // t.offsetMin = Vector2.one * 6;
        isCaopration = false;
    }

    /// <summary>
    /// 交换物品
    /// </summary>
    public void SwapItem()
    {
        var targetItem = targetHoder.itemUI.InventoryDataSOType.items[targetHoder.itemUI.Index];
        var tempItem = currentHolder.itemUI.InventoryDataSOType.items[currentHolder.itemUI.Index];

        //判断两个需要交换的物品，是否类型一样
        bool isSameItem = tempItem.ItmeSOData == targetItem.ItmeSOData;

        //如果两个物品的类型相同，并且可以堆叠
        if (isSameItem && targetItem.ItmeSOData.stackable)
        {
            //就把两个物品合在一块
            targetItem.amount += tempItem.amount;
            tempItem.ItmeSOData = null;
            tempItem.amount = 0;
        }
        else //两个物品不同，或者也不能堆叠
        {
            currentHolder.itemUI.InventoryDataSOType.items[currentHolder.itemUI.Index] = targetItem;
            targetHoder.itemUI.InventoryDataSOType.items[targetHoder.itemUI.Index] = tempItem;
        }
    }
}