using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储区域的容器 脚本
/// </summary>
public class ContainerUI : MonoBehaviour
{
    /// <summary>
    /// 槽位数组
    /// </summary>
    public SlotHoder[] slotHoders;

    /// <summary>
    /// 刷新槽位的显示 UI
    /// </summary>
    public void RefreshUI()
    {
        for (int i = 0; i < slotHoders.Length; i++)
        {
            slotHoders[i].itemUI.Index = i;
            slotHoders[i].UpdateSlotHoderItem();
        }
    }
}