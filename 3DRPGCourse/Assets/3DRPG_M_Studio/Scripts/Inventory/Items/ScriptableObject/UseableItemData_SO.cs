using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消耗品（血药）
/// </summary>
[CreateAssetMenu(fileName = "UseableItem", menuName = "Inventory/UseableItem Data")]
public class UseableItemData_SO : ScriptableObject
{
    /// <summary>
    /// 恢复血量数量
    /// </summary>
    public int healthPoint;
}