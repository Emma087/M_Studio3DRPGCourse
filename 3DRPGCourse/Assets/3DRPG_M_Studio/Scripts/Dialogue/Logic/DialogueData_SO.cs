using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话相关文件
/// </summary>
[CreateAssetMenu(fileName = "New Dailogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    /// <summary>
    /// 一整套对话的 List
    /// </summary>
    public List<DialoguePiece> DialoguePiecesList = new List<DialoguePiece>();

    /// <summary>
    /// 用以装带有对话索引的字典，键是 ID号
    /// </summary>
    public Dictionary<string, DialoguePiece> DialoguePiecesIndexDic = new Dictionary<string, DialoguePiece>();

#if UNITY_EDITOR
    /// <summary>
    /// 每当 Inspector窗口有更新，本函数就会执行
    /// </summary>
    void OnValidate()
    {
        DialoguePiecesIndexDic.Clear();
        foreach (var dialoguePiece in DialoguePiecesList)
        {
            if (!DialoguePiecesIndexDic.ContainsKey(dialoguePiece.ID))
                DialoguePiecesIndexDic.Add(dialoguePiece.ID, dialoguePiece);
        }
    }
#endif
    
}