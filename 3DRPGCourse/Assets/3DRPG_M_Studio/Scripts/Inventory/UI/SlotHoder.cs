using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 槽位的类型
/// </summary>
public enum SlotType
{
    Bag, //背包
    Weapon, //武器
    Armor, //盾牌
    Action //快捷栏
}

/// <summary>
/// 指存储区域的每一个格子 槽位
/// </summary>
public class SlotHoder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;

    public ItemUI itemUI;

    /// <summary>
    /// 刷新物品信息
    /// </summary>
    public void UpdateSlotHoderItem()
    {
        switch (slotType)
        {
            case SlotType.Bag:
                //将自己的数据类型改为 背包数据
                itemUI.InventoryDataSOType = InventoryManager.Instance.inventorySOData;
                break;
            case SlotType.Weapon:
                itemUI.InventoryDataSOType = InventoryManager.Instance.equipmentSOData;
                //替换武器
                if (itemUI.InventoryDataSOType.items[itemUI.Index].ItmeSOData != null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.InventoryDataSOType.items[itemUI.Index]
                        .ItmeSOData);
                }
                else
                {
                    //如果玩家信息面板的武器为空了，卸下武器
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }

                break;
            case SlotType.Armor:
                itemUI.InventoryDataSOType = InventoryManager.Instance.equipmentSOData;
                break;
            case SlotType.Action:
                itemUI.InventoryDataSOType = InventoryManager.Instance.actionSOData;
                break;
        }

        // 拿到已经修改为某个类型存储区域的数据以后，拿到 List中指定 Index 的那个物品信息
        var item = itemUI.InventoryDataSOType.items[itemUI.Index];
        // 然后刷新到 UI 列表上面
        itemUI.SetupItemUI(item.ItmeSOData, item.amount);
    }


    /// <summary>
    /// 使用消耗品
    /// </summary>
    public void UseItem()
    {
        //如果物品为空，则返回
        if (itemUI.GetItem() == null) return;

        //保证物品类型是消耗品而非武器
        if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.InventoryDataSOType.items[itemUI.Index].amount > 0)
        {
            //主角加血
            GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableItemSoData.healthPoint);
            //刷新主角血量
            GameManager.Instance.playerStats.GetComponent<PlayerController>().UpdateHealthAndExp();
            //背包中消耗品数量 -1
            itemUI.InventoryDataSOType.items[itemUI.Index].amount -= 1;
        }

        UpdateSlotHoderItem();
    }

    /// <summary>
    /// 【实现接口】如果双击的话，使用消耗品
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    /// <summary>
    /// 【实现接口】开始悬停在某一个物体上
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.itmeToolTip.SetupToolTip(itemUI.GetItem());
            InventoryManager.Instance.itmeToolTip.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 【实现接口】停止悬停在某一个物体上
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.itmeToolTip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //关闭背包的时候，顺便关闭 TIps窗口
        InventoryManager.Instance.itmeToolTip.gameObject.SetActive(false);
    }
}