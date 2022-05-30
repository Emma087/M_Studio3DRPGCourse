using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//传送门代码，触碰到传送门以后调用的代码
public class TransitionPoint : MonoBehaviour
{
    /// <summary>
    /// 场景切换的枚举，同场景/异场景
    /// </summary>
    public enum TransitionType
    {
        SameScene, //同场景传送
        DifferentScene //不同场景传送
    }

    /// <summary>
    /// 与 TransitionType枚举有关，与 GoToDestinationTag所在的场景同步（目的地传送门的场景名字）
    /// </summary>
    [Header("Transition Info 目的地传送门所在的场景名字")]
    public string sceneName;

    /// <summary>
    /// 关于传送点场景的类型
    /// </summary>
    public TransitionType transitionType;

    /// <summary>
    /// 可以传送到的指定目的地的标签 tag，本项目中，每一个传送门只能去往一个地方，是这样设计的
    /// </summary>
    public TransitionDestination.DestinationTag GoToDestinationTag;

    /// <summary>
    /// 现在是否可以进行传送（只有到达传送点区域才可以传送）
    /// </summary>
    bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            // TODO 进行传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}