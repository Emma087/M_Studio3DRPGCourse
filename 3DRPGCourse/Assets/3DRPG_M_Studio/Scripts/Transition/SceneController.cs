using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

///场景管理器，管理是哪一个场景相关信息
public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    /// <summary>
    /// 玩家的预制体物体，需要拖拽赋值
    /// </summary>
    [Header("玩家的预制体物体，拖拽赋值")] public GameObject playerPrefab;

    /// <summary>
    /// 渐入渐出的 UI效果预制体
    /// </summary>
    [Header("渐入渐出的 UI Mask 板预制体，拖拽赋值")] public SceneFader sceneFaderPrefab;

    private GameObject player; //场景中的 Player哦
    private NavMeshAgent playerAgent;

    /// <summary>
    /// 是否可以进行观察者方法的启用
    /// </summary>
    bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    /// <summary>
    /// 传送到目的位置
    /// </summary>
    /// <param name="transitionPoint">另一个传送点</param>
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            //同一场景的传递函数
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,
                    transitionPoint.GoToDestinationTag));
                break;

            //异场景的传递
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName,
                    transitionPoint.GoToDestinationTag));
                break;
        }
    }

    /// <summary>
    /// 启动传送到指定位置的协程函数
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="GoToDestination"></param>
    /// <returns></returns>
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag GoToDestination)
    {
        //保存玩家的数据
        SaveManager.Instance.SavePlayerData();
        //保存背包三个面板相关数据
        InventoryManager.Instance.SaveData();

        //异场景传送
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, transform.position, transform.rotation);
            player.transform.SetPositionAndRotation(GetDestination(GoToDestination).transform.position,
                GetDestination(GoToDestination).transform.rotation);
            //读取玩家的数据
            SaveManager.Instance.LoadPlayerData();
            InventoryManager.Instance.LoadData();
        }
        else //同场景传送
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(GoToDestination).transform.position,
                GetDestination(GoToDestination).transform.rotation);
            playerAgent.enabled = true;
        }

        //保存最后一次切换场景以后，所在的场景
        SaveManager.Instance.SaveSceneInfo();
    }

    /// <summary>
    /// 获取目的地信息
    /// </summary>
    /// <param name="destinationTag">场景传送点的枚举类型，用以区分是哪一个传送点</param>
    /// <returns></returns>
    TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].SelfPosNameTag == destinationTag)
                return entrances[i];
        }

        return null;
    }

    /// <summary>
    /// 回到开始界面的场景
    /// </summary>
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    /// <summary>
    /// 继续游戏时候，加载刚才挂起的场景，只有继续游戏按钮才调用这个
    /// </summary>
    public void TransitionToLoadGame()
    {
        Debug.Log("需要加载的场景名字为 " + SaveManager.Instance.SceneName);
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    /// <summary>
    /// 加载游戏场景一号场景，只有新游戏开始的时候才调用这个
    /// </summary>
    /// <param name="scene"></param>
    public void TransitionToFirstLevel(string scene) //加载场景，启动协程
    {
        StartCoroutine(LoadLevel(scene));
    }

    /// <summary>
    /// 加载指定的场景的协程，还会顺便加载主角
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != "")  //场景的名判空
        {
            yield return StartCoroutine(fade.FadeOut());
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,
                GameManager.Instance.GetEntrance().rotation);
            //保存玩家的数据
            SaveManager.Instance.SavePlayerData();
            //保存背包三个面板相关数据
            InventoryManager.Instance.SaveData();
            yield return StartCoroutine(fade.FadeIn());
        }
    }

    /// <summary>
    /// 回到主场景，加载开始界面场景
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadMain()
    {
        //保存玩家的数据
        SaveManager.Instance.SavePlayerData();
        //保存背包三个面板相关数据
        InventoryManager.Instance.SaveData();
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut());
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn());
    }

    /// <summary>
    /// 主角死了以后，返回开始界面
    /// </summary>
    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}