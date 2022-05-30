using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    /// <summary>
    /// 当前的 DialogueData_SO类型文件
    /// </summary>
    public DialogueData_SO currentSOData;

    /// <summary>
    /// 是否可以进行对话行为
    /// </summary>
    bool canTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentSOData != null)
        {
            canTalk = true;
        }
    }

    private void Update()
    {
        if (canTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialogue();
        }
    }

    /// <summary>
    /// 打开对话窗口
    /// </summary>
    void OpenDialogue()
    {
        DialogueUI.Instance.UpdateDialogueData(currentSOData);
        DialogueUI.Instance.UpdateMainDialogue(currentSOData.DialoguePiecesList[0]);
    }
}