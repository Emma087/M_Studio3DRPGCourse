using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Golem : EnemyController
{
    [Header("Skill 石像人技能击飞力")] public float kickForce = 30; //击飞物体的力

    public GameObject rockPrefab; //石头武器的 Prefab
    public Transform rockBornPosition;

    /// <summary>
    /// 捡石头的敌人的攻击函数
    /// Animation Event 击飞面前目标的动画事件
    /// </summary>
    public void GolemKickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;

            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    /// <summary>
    /// 手里生成石头物体，石头的目标为主角
    /// Animation Event 扔石头动画事件
    /// </summary>
    void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, rockBornPosition.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}