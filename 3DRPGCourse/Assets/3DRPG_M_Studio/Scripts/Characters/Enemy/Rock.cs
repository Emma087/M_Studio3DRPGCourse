using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockState
    {
        HitPlayer, //伤害主角
        HitEnemt, //伤害敌人
        HitNothing //落在地上谁也不伤害
    }

    public RockState rockState; //石头当前的状态

    private Rigidbody rigidbody;
    [Header("Basic Setting")] public float force;
    public int damage;

    private Vector3 direction; //扔石头的朝向
    public GameObject target;

    public GameObject breakEffect;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        //防止石头一生成，就变成了 RockState.HitNothing，因为一生成的石头没有速度
        rigidbody.velocity = Vector3.one;

        rockState = RockState.HitPlayer;
        FlyToTarget();
        // Destroy(gameObject,2f);
    }

    private void FixedUpdate()
    {
        //检测石头是否落地了，马上要静止了
        if (rigidbody.velocity.sqrMagnitude < 1f)
        {
            rockState = RockState.HitNothing;
        }
    }

    /// <summary>
    /// 石头自己朝目标飞，这个目标在石头从敌人手里生成的时候，被赋值
    /// </summary>
    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;

        //加的这个 vector3.up 是为了让石头在空中稍微飞一下，不要生成了就掉地上了
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockState)
        {
            case RockState.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage
                        (damage, other.gameObject.GetComponent<CharacterStats>(), null);
                    rockState = RockState.HitNothing;
                }

                break;

            case RockState.HitEnemt:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats, null);
                    GameObject go = Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    Destroy(go, 1.5f);
                }

                break;

            case RockState.HitNothing:
                break;
        }
    }
}