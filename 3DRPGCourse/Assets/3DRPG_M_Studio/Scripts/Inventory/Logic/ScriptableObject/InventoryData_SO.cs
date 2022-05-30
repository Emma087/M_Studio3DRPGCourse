using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储区域的整体 ScriptableObject类型文件
/// </summary>
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    /// <summary>
    /// 储存数据库区域用以存储物品信息的 List列表
    /// </summary>
    public List<InventoryItem> items = new List<InventoryItem>();

    /// <summary>
    /// 储存数据库区域中增加某一个物品
    /// </summary>
    /// <param name="putItemData">要放进背包的物品</param>
    /// <param name="number"></param>
    public void AddItem(ItmeData_SO putItemData, int number)
    {
        bool found = false;
        if (putItemData.stackable) //可以堆叠
        {
            foreach (var item in items) //循环列表找一下有没有跟放进来的物品一样的
            {
                if (item.ItmeSOData == putItemData)
                {
                    //有一样的物品，就将背包的数量加上放进来的物品就可以
                    item.amount += number;
                    found = true;
                    break;
                }
            }
        }

        //找到最近的一个空白格子，将物品放下去
        for (int i = 0; i < items.Count; i++)
        {
            //如果最近的某个格子为空白，而且也没有找到相同的物体，
            if (items[i].ItmeSOData == null && !found)
            {
                items[i].ItmeSOData = putItemData;
                items[i].amount = number;
                break;
            }
        }
    }
}

/// <summary>
/// 储存数据库区域每一个格子的信息
/// </summary>
[Serializable]
public class InventoryItem
{
    /// <summary>
    /// 当前格子内，物品的信息
    /// </summary>
    public ItmeData_SO ItmeSOData;

    /// <summary>
    /// 在当前格子内，物品的总数
    /// </summary>
    public int amount;
}