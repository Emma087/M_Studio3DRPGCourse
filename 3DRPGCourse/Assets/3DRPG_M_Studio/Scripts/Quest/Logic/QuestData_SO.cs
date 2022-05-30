using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有关于任务模块的 ScriptableObject类型文件
/// </summary>
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    /// <summary>
    /// 任务的名字
    /// </summary>
    public string questName;

    /// <summary>
    /// 任务的描述信息
    /// </summary>
    [TextArea] public string description;

    /// <summary>
    /// 任务的状态：任务已经开始，任务已经完成了（还没有交任务领奖励），任务彻底完成（奖励已经领完了）
    /// </summary>
    public bool isStarted, isComplete, isFinished;

    /// <summary>
    /// 任务列表
    /// </summary>
    public List<QuestRequire> questRequires = new List<QuestRequire>();
}

/// <summary>
/// 任务的目标类
/// </summary>
[Serializable]
public class QuestRequire
{
    /// <summary>
    /// 任务的名字
    /// </summary>
    public string name;

    /// <summary>
    /// 任务所需要的数量
    /// </summary>
    public int requireAmount;

    /// <summary>
    /// 当前完成了几个数量
    /// </summary>
    public int currentAmount;
}