using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 拖拽界面面板脚本
/// </summary>
public class DragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;

    private void Start()
    {
        canvas = InventoryManager.Instance.GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 【接口实现】拖拽面板
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta /
                                          canvas.scaleFactor; //拖拽面板
    }

    /// <summary>
    /// 【接口实现】点按面板
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);
    }
}