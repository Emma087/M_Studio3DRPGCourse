using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品的分类枚举
/// </summary>
public enum ItemType
{
    Useable, //使用物品，消耗品
    Weapon, //武器
    Armor //盔甲装备
}

///世界产生的地面上的物品的信息 ScriptableObject类型文件
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItmeData_SO : ScriptableObject
{
    [Header("物品类型")] public ItemType itemType;
    [Header("物品名称")] public string itemName;
    [Header("物品的图标")] public Sprite itemIcon;
    
    /// <summary>
    /// 当前物品数量
    /// </summary>
    [Header("物品数量")] public int itemNumber;

    [Header("物品描述信息")] [TextArea] public string description; //物品的信息描述

    /// <summary>
    /// 当前物品是否可以堆叠
    /// </summary>
    [Header("是否可以堆叠重复")] public bool stackable; //是否能够堆叠，比如武器一个格子只能放一把，材料一个格子放很多

    /// <summary>
    /// 关于消耗品的数值
    /// </summary>
    [Header("消耗品数值相关")] public UseableItemData_SO useableItemSoData;


    //世界中掉落的物品，以及
    [Header("Choose correspond weapon")] public GameObject weaponPrefab;

    /// <summary>
    /// 关于武器的攻击数值
    /// </summary>
    [Header("武器的攻击数值相关")] public AttackData_SO weaponAttackSOData;

    /// <summary>
    /// 武器配套的动画控制器
    /// </summary>
    [Header("武器配套的动画控制器")] public AnimatorOverrideController weaponAnimator;
}