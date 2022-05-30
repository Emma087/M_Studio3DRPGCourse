using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats; //玩家的信息数据

    private CinemachineFreeLook freeLook;

    //List 中，包含所有订阅了 IEndGameObserver这个接口的观察者
    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    
    /// <summary>
    /// 登记玩家信息，由玩家脚本中自己调用注册
    /// </summary>
    /// <param name="player"></param>
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
        freeLook = FindObjectOfType<CinemachineFreeLook>();
        if (freeLook != null)
        {
            freeLook.Follow = player.transform;
            freeLook.LookAt = player.transform;
        }
    }

    /// <summary>
    /// 添加观察者到 List列表中去
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    /// <summary>
    /// 移除 List中指定的观察者
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    /// <summary>
    /// 通知 List中的每一个观察者
    /// </summary>
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    /// <summary>
    /// 返回当前场景内的传送门入口
    /// </summary>
    /// <returns></returns>
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.SelfPosNameTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform;
        }

        return null;
    }
}