using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话一套内容
/// </summary>
[Serializable]
public class DialoguePiece
{
    /// <summary>
    /// 本段对话的 ID号
    /// </summary>
    public string ID;

    /// <summary>
    /// 本段对话的图片
    /// </summary>
    public Sprite image;

    /// <summary>
    /// 本段对话的内容
    /// </summary>
    [TextArea] public string text;

    /// <summary>
    /// 有关于任务的信息
    /// </summary>
    public QuestData_SO QuestDataSo;

    /// <summary>
    /// 
    /// </summary>
    public List<DialogueOption> OptionsList = new List<DialogueOption>();
}