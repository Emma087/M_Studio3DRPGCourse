using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class SaveManager : Singleton<SaveManager>
{
    private string sceneNameKey = "";
    public string SceneName => PlayerPrefs.GetString(sceneNameKey);


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 保存玩家的数据
    /// </summary>
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.currentCharacterSOData,
            GameManager.Instance.playerStats.currentCharacterSOData.name);
    }

    /// <summary>
    /// 保存场景相关的内容
    /// </summary>
    public void SaveSceneInfo()
    {
        Debug.Log("当前保存的场景名字为 " + SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString(sceneNameKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载玩家的数据
    /// </summary>
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.currentCharacterSOData,
            GameManager.Instance.playerStats.currentCharacterSOData.name);
    }

    /// <summary>
    /// 保存具体的玩家数据相关
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="jsonKeyName"></param>
    public void Save(Object saveData, string jsonKeyName)
    {
        //这里接收到的是一个 string类型值
        var jsonData = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(jsonKeyName, jsonData);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载具体的玩家数据相关
    /// </summary>
    /// <param name="loadData"></param>
    /// <param name="jsonKeyName"></param> 
    public void Load(Object loadData, string jsonKeyName)
    {
        if (PlayerPrefs.HasKey(jsonKeyName))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(jsonKeyName), loadData);
        }
    }

    private void Update()
    {
        //按下 ESC，就返回主界面场景
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }

        // //按下 S，就保存玩家的数据
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     SavePlayerData();
        // }
    }
}