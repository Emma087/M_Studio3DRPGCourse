using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拾取物品
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    public ItmeData_SO itmeData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //物品装进背包  
            InventoryManager.Instance.inventorySOData.AddItem(itmeData, itmeData.itemNumber);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            
            //测试装备武器
            // GameManager.Instance.playerStats.EquipWeapon(itmeData);

            Destroy(gameObject);
        }
    }
}