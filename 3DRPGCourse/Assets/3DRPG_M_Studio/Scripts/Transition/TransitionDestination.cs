using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 传送门相关，关于目的地的枚举类型，目的地的标签
public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER,
        A,
        B,
        C
    }

    /// <summary>
    /// 本传送门的标签名字
    /// </summary>
    public DestinationTag SelfPosNameTag;
}