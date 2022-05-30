using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 快捷栏相关的方法，主要是对应几个键盘按钮
/// </summary>
public class ActionBtn : MonoBehaviour
{
    /// <summary>
    /// 是哪一个键盘控制
    /// </summary>
    public KeyCode actionKey;

    SlotHoder currentSlotHoder;

    private void Awake()
    {
        currentSlotHoder = GetComponent<SlotHoder>();
    }

    private void Update()
    {
        //按了键，并且快捷栏有物品
        if (Input.GetKeyDown(actionKey) && currentSlotHoder.itemUI.GetItem())
        {
            currentSlotHoder.UseItem();
        }
    }
}