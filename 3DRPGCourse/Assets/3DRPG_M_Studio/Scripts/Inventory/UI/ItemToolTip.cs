using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemToolTip : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemInfo;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UpdatePosition();
    }

    /// <summary>
    /// 设置文字的信息
    /// </summary>
    public void SetupToolTip(ItmeData_SO item)
    {
        itemName.text = item.itemName;
        itemInfo.text = item.description;
    }


    /// <summary>
    /// 鼠标的坐标，临时变量
    /// </summary>
    Vector3 mousePos;

    /// <summary>
    /// Tips窗口面板的四个角坐标
    /// </summary>
    private Vector3[] corners = new Vector3[4];

    /// <summary>
    /// 更新鼠标的坐标信息，Tips面板的大小信息
    /// </summary>
    public void UpdatePosition()
    {
        mousePos = Input.mousePosition;
        rectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        if (mousePos.y < height)
            rectTransform.position = mousePos + Vector3.up * height * 0.7f;
        else if (Screen.width - mousePos.x > width)
            rectTransform.position = mousePos + Vector3.right * width * 0.7f;
        else
            rectTransform.position = mousePos + Vector3.left * width * 0.7f;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        UpdatePosition();
    }
}