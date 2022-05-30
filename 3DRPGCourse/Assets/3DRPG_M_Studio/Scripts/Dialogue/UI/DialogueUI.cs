using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("对话窗口 UI的基础的功能")] public Image icon;
    public Text mainText;
    public Button nextButton;


    /// <summary>
    /// 对话的 UI面板
    /// </summary>
    public GameObject dialoguePanel;

    /// <summary>
    /// 回复文字 UI面板
    /// </summary>
    [Header("关于回复 Option相关功能")] public RectTransform optionPanel;

    /// <summary>
    /// 对话框物体
    /// </summary>
    public OptionUI optionPrefab;

    /// <summary>
    /// 当前的 DialogueData_SO数据
    /// </summary>
    [Header("数据")] public DialogueData_SO currentSOData;

    /// <summary>
    /// 当前对话的索引
    /// </summary>
    private int currentIndex;

    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);
    }

    /// <summary>
    /// 按钮事件：下一条对话显示
    /// </summary>
    void ContinueDialogue()
    {
        //如果对话当前索引，小于对话数据等待播放的 List中总数，那么才可以播放对话
        if (currentIndex < currentSOData.DialoguePiecesList.Count)
            UpdateMainDialogue(currentSOData.DialoguePiecesList[currentIndex]);
        else
            dialoguePanel.SetActive(false); //否则关闭对话界面
    }

    /// <summary>
    /// 更新对话相关数据
    /// </summary>
    /// <param name="data"></param>
    public void UpdateDialogueData(DialogueData_SO data)
    {
        Debug.Log("更新对话的数据");
        currentSOData = data;
        currentIndex = 0;
    }

    /// <summary>
    /// 更新主对话
    /// </summary>
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);
        currentIndex++;
        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;
        }

        mainText.text = "";
        mainText.DOText(piece.text, 1f);
        // mainText.text = piece.text;

        //如果玩家的回复没有了，并且下一套 NPC的对话还有
        if (piece.OptionsList.Count == 0 && currentSOData.DialoguePiecesList.Count > 0)
        {
            //显示下一套对话，并且 Index增加
            nextButton.gameObject.SetActive(true);
            nextButton.interactable = true;
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            nextButton.interactable = false;
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        //创建 Option回复
        CreateOptions(piece);
    }

    /// <summary>
    /// 生成回复内容
    /// </summary>
    /// <param name="piece"></param>
    void CreateOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.OptionsList.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(piece, piece.OptionsList[i]);
        }
    }
}