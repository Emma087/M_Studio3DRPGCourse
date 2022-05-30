using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 拖拽的物品信息相关，主要是位置和父级的记录
/// </summary>
public class DragData
{
    /// <summary>
    /// 拖拽物品原始的槽位物体，里面可以拿到一些列值，比如原始的物品信息等
    /// </summary>
    public SlotHoder originaHolder;

    /// <summary>
    /// 原始槽位所在的父级物体
    /// </summary>
    public RectTransform originalParent;
}

/// <summary>
/// 存储区域管理器
/// 关系图是 InventoryManager > ContainerUI > SlotHoder > ItemUI
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    /// <summary>
    /// 背包的当前 SO 数据
    /// </summary>
    [Header("背包区域")] public InventoryData_SO inventorySOData;

    /// <summary>
    /// 临时使用的背包数据
    /// </summary>
    public InventoryData_SO inventorySOTemplate;

    /// <summary>
    /// 背包的 Containers 容器UI
    /// </summary>
    public ContainerUI inventoryUI;

    /// <summary>
    /// 背包面板【手动拖拽赋值】
    /// </summary>
    public GameObject bagPanel;

    /// <summary>
    /// 快捷栏当前的 SO 数据
    /// </summary>
    [Header("快捷栏区域")] public InventoryData_SO actionSOData;

    /// <summary>
    /// 临时使用的快捷栏数据
    /// </summary>
    public InventoryData_SO actionSOTemplate;

    /// <summary>
    /// 快捷栏的 Containers 容器UI
    /// </summary>
    public ContainerUI actionUI;


    /// <summary>
    /// 玩家面板当前的 SO 数据
    /// </summary>
    [Header("玩家信息面板区域")] public InventoryData_SO equipmentSOData;


    /// <summary>
    /// 临时使用的玩家信息面板数据
    /// </summary>
    public InventoryData_SO equipmentSOTemplate;

    /// <summary>
    /// 玩家信息面板【手动拖拽赋值】
    /// </summary>
    public GameObject statsPanel;

    /// <summary>
    /// 玩家的血条 UI
    /// </summary>
    public TMP_Text healthText;

    /// <summary>
    /// 玩家的攻击力 UI
    /// </summary>
    public TMP_Text attackText;

    /// <summary>
    /// 玩家信息面板的 Containers 容器UI
    /// </summary>
    public ContainerUI equipmentUI;

    [Header("物品的信息描述 tips板")] public ItemToolTip itmeToolTip;

    /// <summary>
    /// 这个画布处于最上层，使得正在拖拽得物体，渲染一定在最上面
    /// </summary>
    [Header("供拖拽的最上面的 Canvas")] public Canvas DragCavans;

    /// <summary>
    /// 正在拖拽的物体
    /// </summary>
    public DragData currentDrag;

    /// <summary>
    /// 背包面板 和 玩家信息面板 是否开启状态
    /// </summary>
    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
        if (inventorySOTemplate != null)
            inventorySOData = Instantiate(inventorySOTemplate);
        if (actionSOTemplate != null)
            actionSOData = Instantiate(actionSOTemplate);
        if (equipmentSOTemplate != null)
            equipmentSOData = Instantiate(equipmentSOTemplate);
    }

    private void Start()
    {
        LoadData();

        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    /// <summary>
    /// 背包三个面板相关的数据存储
    /// </summary>
    public void SaveData()
    {
        SaveManager.Instance.Save(inventorySOData, inventorySOData.name);
        SaveManager.Instance.Save(actionSOData, actionSOData.name);
        SaveManager.Instance.Save(equipmentSOData, equipmentSOData.name);
    }

    /// <summary>
    /// 背包三个面板相关的数据读取
    /// </summary>
    public void LoadData()
    {
        Debug.Log("加载背包数据");
        SaveManager.Instance.Load(inventorySOData, inventorySOData.name);
        SaveManager.Instance.Load(actionSOData, actionSOData.name);
        SaveManager.Instance.Load(equipmentSOData, equipmentSOData.name);
    }

    /// <summary>
    /// 更新玩家信息面板的，血量信息，攻击力信息
    /// </summary>
    /// <param name="health"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void UpdateStatsText(int health, int min, int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + " - " + max;
    }

    private void Update()
    {
        //键盘 B控制开启背包面板
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
    }

    #region 检查拖拽物品是否在每一个 Slot 范围内

    /// <summary>
    /// 检测拖拽的物品是否在 背包的范围内
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryUI.slotHoders.Length; i++)
        {
            RectTransform t = inventoryUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 检测拖拽的物品是否在 快捷栏的范围内
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHoders.Length; i++)
        {
            RectTransform t = actionUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 检测拖拽的物品是否在 玩家的信息面板范围内
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckInequipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHoders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}