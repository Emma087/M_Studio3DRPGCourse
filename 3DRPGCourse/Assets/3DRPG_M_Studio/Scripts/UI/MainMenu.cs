using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button btnNewGame;
    Button btnContinue;
    Button btnQuit;

    /// <summary>
    /// TimeLine下面的功能
    /// </summary>
    PlayableDirector _director;

    private void Awake()
    {
        btnNewGame = transform.GetChild(1).GetComponent<Button>();
        btnContinue = transform.GetChild(2).GetComponent<Button>();
        btnQuit = transform.GetChild(3).GetComponent<Button>();
    }

    private void Start()
    {
        btnNewGame.onClick.AddListener(PlayTimeLine);
        btnQuit.onClick.AddListener(QuitGame);
        btnContinue.onClick.AddListener(ContinueGame);

        _director = FindObjectOfType<PlayableDirector>();
        _director.stopped += NewGameStart;
    }

    /// <summary>
    /// 播放拜访好的 TimeLine
    /// </summary>
    void PlayTimeLine()
    {
        _director.Play();
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    /// <param name="obj"></param>
    void NewGameStart(PlayableDirector obj) //这个参数用不上，但是  _director.stopped 需要一个这样的参数
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel("GameScene"); //加载首次进入游戏的场景，Game 一号场景
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    void ContinueGame()
    {
        if (SaveManager.Instance.SceneName == "")
        {
            PlayTimeLine();
            return;
        }

        //转换场景，读取进度
        SceneController.Instance.TransitionToLoadGame();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}