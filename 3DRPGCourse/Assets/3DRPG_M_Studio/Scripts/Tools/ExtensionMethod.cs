using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//扩展脚本：扇形区域计算
public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    //是否面朝着目标（目标是否在眼前的扇形区域，以中心为起点，左右各 60°夹角，Cos60，Dot 值为 0.5 ）
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();
        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        return dot >= dotThreshold;
    }
}