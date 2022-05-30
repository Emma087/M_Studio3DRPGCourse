using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话回复相关（也就是玩家会回复什么内容）
/// </summary>
[Serializable]
public class DialogueOption
{
    /// <summary>
    /// 对话回复内容
    /// </summary>
    public string text;

    /// <summary>
    /// 回复对话要转到的下一个对话目标 ID号
    /// </summary>
    public string targetID;

    /// <summary>
    /// 是否接受任务
    /// </summary>
    public bool takeQuest;
}