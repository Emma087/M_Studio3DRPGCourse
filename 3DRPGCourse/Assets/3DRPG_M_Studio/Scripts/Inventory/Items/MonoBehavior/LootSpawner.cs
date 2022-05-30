using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootSpawner : MonoBehaviour
{
    /// <summary>
    /// 可能掉落的物品 列表数组
    /// </summary>
    public LootItem[] lootItems;

    /// <summary>
    /// 掉落物品
    /// </summary>
    public void SpawnLoot()
    {
        float f = Random.value;
        for (int i = 0; i < lootItems.Length; i++)
        {
            if (f <= lootItems[i].weight)
            {
                //生成物品
                GameObject o = Instantiate(lootItems[i].item);
                o.transform.position = transform.position + Vector3.up * 2;
                break;  //掉落了一个，就不掉落其他的了
            }
        }
    }
}

[Serializable]
public class LootItem
{
    /// <summary>
    /// 有几率掉落的物品
    /// </summary>
    public GameObject item;

    /// <summary>
    /// 掉落物品的权重
    /// </summary>
    [Range(0, 1)] public float weight;
}