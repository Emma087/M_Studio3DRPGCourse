using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// UI相关，主角的血条 UI 显示和刷新控制
public class PlayerHealthUI : MonoBehaviour
{
    private TMP_Text levelText; //等级的文字
    private Image healthSlider;
    private Image expSlider;

    private TMP_Text healthText; //血量的文字
    private TMP_Text expText; //经验文字

    private void Awake()
    {
        levelText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();

        healthText = transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        expText = transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();
    }

    /// <summary>
    /// 刷新主角的 血量和经验值
    /// </summary>
    public void UpdatePalyerHealthAndExp(int lv, int currentHealth, int maxHealth, int currentExp, int baseExp)
    {
        UpdateHealth(currentHealth, maxHealth);

        levelText.text = "Lv: " + lv.ToString("00");
        UpdateExp(currentExp, baseExp);
    }

    /// <summary>
    /// 刷新血量
    /// </summary>
    void UpdateHealth(int currentHealth, int maxHealth)
    {
        float s = (float) currentHealth / maxHealth;
        float sliderPercent = (float) GameManager.Instance.playerStats.CurrentHealth /
                              GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = s;
        healthText.text = currentHealth + "/" + maxHealth;
    }

    /// <summary>
    /// 刷新经验值
    /// </summary>
    void UpdateExp(int currentExp, int baseExp)
    {
       // Debug.Log("刷新主角经验值");
       // Debug.Log("主角的当前经验为 " + currentExp);

        float s = (float) currentExp / baseExp;
        float sliderPercent = (float) GameManager.Instance.playerStats.currentCharacterSOData.currentExp /
                              GameManager.Instance.playerStats.currentCharacterSOData.baseExp;
        expSlider.fillAmount = s;
        expText.text = currentExp + "/" + baseExp;
    }
}