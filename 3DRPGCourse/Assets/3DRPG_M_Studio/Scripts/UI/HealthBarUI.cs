using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// UI相关，敌人的血条 UI 显示和刷新控制
public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab; //血条 UI预制体
    public Transform HPPoint; //每个角色设定好的 血条的位置
    public bool alwaysVisible; //是否长久的显示出来

    public float visibleTime; //可视化血条的持续时间
    private float timeLife; //计时器

    private Image healthSlider; //实际血量可以控制滑动的那个
    private Transform UIbar; //这个是 UI血条的底，就是血条的父物体
    private Transform camare; //相机，用以 UI 朝着相机保持角度

    private CharacterStats currentStats; //当前的角色属性值信息

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar; //事件添加订阅
    }

    private void OnEnable()
    {
        camare = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                // UIbar.parent = gameObject.transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    /// <summary>
    /// 刷新血条的 UI
    /// </summary>
    /// <param name="currentHealth"></param>
    /// <param name="maxHealth"></param>
    private void UpdateHealthBar(int currentHealth, int maxHealth) //刷新血量 UI
    {
        if (currentHealth <= 0)
        {
            currentStats.UpdateHealthBarOnAttack -= UpdateHealthBar;
            Destroy(UIbar.gameObject);
            return;
        }

        UIbar.gameObject.SetActive(true);
        timeLife = visibleTime;

        float sliderPercent = (float) currentHealth / maxHealth; //为了保险转以下 float
        healthSlider.fillAmount = sliderPercent;
    }


    /// <summary>
    /// 主要是用于更新血条 UI 实时跟随敌人
    /// </summary>
    private void LateUpdate()
    {
        if (UIbar != null && UIbar.gameObject.activeSelf)
        {
            UIbar.position = HPPoint.position;
            UIbar.forward = -camare.forward; //保证 血条UI 朝着相机

            if (timeLife <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLife -= Time.deltaTime;
        }
    }
}