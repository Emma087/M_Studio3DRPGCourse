using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    /// <summary>
    /// 回复的文字
    /// </summary>
    public Text optionText;

    /// <summary>
    /// 回复本身按钮
    /// </summary>
    public Button thisBtn;

    /// <summary>
    /// 当前这套对话
    /// </summary>
    private DialoguePiece curretPiece;

    /// <summary>
    /// 下一套对话的 ID
    /// </summary>
    private string nextPieceID;

    private void Awake()
    {
        thisBtn = GetComponent<Button>();
        thisBtn.onClick.AddListener(OnOptionClicked);
    }

    /// <summary>
    /// 更新回复模块的内容
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="option"></param>
    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        curretPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
    }

    /// <summary>
    /// 点击回复对话
    /// </summary>
    public void OnOptionClicked()
    {
        //如果本套对话结束，就关闭对话 UI
        if (nextPieceID == "")
        {
            Debug.Log("关闭对话按钮框");
            DialogueUI.Instance.dialoguePanel.SetActive(false);
        }
        else
        {
            Debug.Log("更新下一套对话");
            DialogueUI.Instance.UpdateMainDialogue(
                DialogueUI.Instance.currentSOData.DialoguePiecesIndexDic[nextPieceID]);
        }
    }
}