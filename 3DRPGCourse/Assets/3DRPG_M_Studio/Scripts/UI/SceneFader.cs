using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [Header("进入一个场景所需要的时间")] public float fadeInDuration;
    [Header("离开一个场景所需要的时间")] public float fadeOutDuration;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        //这里不要删除，不是说这个物体类似 GameManager 那种用法
        //而是说，这里一直由类似 GameManager 这种长存的物体在使用
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 没用上这个函数
    /// </summary>
    /// <returns></returns>
    // public IEnumerator FadeOutIn()
    // {
    //     yield return FadeOut(fadeOutDuration);
    //     yield return FadeIn(fadeInDuration);
    // }

    /// <summary>
    /// 离开一个场景，白色渐浓的效果
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator FadeOut()
    {
        while (_canvasGroup.alpha < 1)
        {
            _canvasGroup.alpha += Time.deltaTime / fadeOutDuration;
            yield return null;
        }
    }

    /// <summary>
    /// 进入一个场景，白色渐淡的效果
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator FadeIn()
    {
        while (_canvasGroup.alpha != 0)
        {
            _canvasGroup.alpha -= Time.deltaTime / fadeInDuration;
            yield return null;
        }

        Destroy(gameObject);
    }
}