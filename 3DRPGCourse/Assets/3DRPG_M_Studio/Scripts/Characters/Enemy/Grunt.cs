using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Grunt : EnemyController
{
    [Header("Skill 魔兽人技能击飞力")] public float kickForce = 15; //击飞物体的力

    /// <summary>
    /// 击飞面前目标
    /// 动画事件调用
    /// </summary>
    public void GruntKickOff() 
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
        
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}