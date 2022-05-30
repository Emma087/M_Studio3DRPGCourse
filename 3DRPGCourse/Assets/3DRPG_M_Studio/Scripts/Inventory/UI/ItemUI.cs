using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包 UI面板下 > COntainer下 > SlotHoder下 > ItemUI
/// 主要是每一个背包格子中的物品相关信息修改和显示一类的
/// </summary>
public class ItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amount;

    /// <summary>
    /// 存储区 SO的类型，是背包，还是快捷栏，拿到这个值以后是用于赋值给自己的刷新
    /// 虽然是 Public修饰符，但是作为一个 Porperty在 Inspector面板是不可见的
    /// </summary>
    public InventoryData_SO InventoryDataSOType { get; set; }

    /// <summary>
    /// 在储存区域的储存序号，是第几个
    /// 虽然是 Public修饰符，但是作为一个 Porperty在 Inspector面板是不可见的
    /// </summary>
    public int Index { get; set; } = -1;

    /// <summary>
    /// 刷新背包某一个格子的物品信息 UI
    /// </summary>
    /// <param name="putItem">放进来的那个物品</param>
    /// <param name="putItemNumber">那个物品的数量</param>
    public void SetupItemUI(ItmeData_SO putItem, int putItemNumber)
    {
        //如果物品为 0了，隐藏图标
        if (putItemNumber == 0)
        {
            InventoryDataSOType.items[Index].ItmeSOData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (putItem != null)
        {
            icon.sprite = putItem.itemIcon;
            amount.text = putItemNumber.ToString();
            icon.gameObject.SetActive(true);
        }
        else
            icon.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取 item列表下，指定 Index（初始化时候，index被赋的值）的 ItemSO数据
    /// </summary>
    /// <returns></returns>
    public ItmeData_SO GetItem()
    {
        return InventoryDataSOType.items[Index].ItmeSOData;
    }
}