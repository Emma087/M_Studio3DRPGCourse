using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

//主角玩家的控制脚本
public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    CharacterStats characterStats; //主角的属性读取脚本

    /// <summary>
    /// 主角身上 Agent 组件的 stopDistance 数值
    /// </summary>
    float stopDistanceAgent;

    /// <summary>
    /// 主角的血条相关
    /// </summary>
    public PlayerHealthUI healthUI;

    /// <summary>
    /// 给主角设定一个攻击目标物体
    /// </summary>
    GameObject attackTargetEnemy;

    /// <summary>
    /// 上次的攻击时间
    /// </summary>
    float lastAttackTime;

    /// <summary>
    /// 主角是否死亡
    /// </summary>
    bool isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistanceAgent = agent.stoppingDistance;
    }

    void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.Log("保存数据为空");
        }

        SaveManager.Instance.LoadPlayerData();

        // TODO 这个函数最好能在 Enable 中调用，因为主角切换场景时候，就调用最好，
        // 但是放在 enable中现在会报错，因为 characterStats 的 Data 赋值慢了一拍
        UpdateHealthAndExp();
    }

    private void OnEnable()
    {
        //GetComponent<Collider>().enabled = false;
        if (MouseManager.Instance == null)
        {
            Debug.Log("鼠标为空");
        }

        //将移动方法添加到鼠标点击事件的订阅中
        MouseManager.Instance.OnMouseClicked += MoveToTarget;

        //将朝着敌人移动的函数添加到事件订阅
        MouseManager.Instance.OnEnemyClicked += EventAttackEnemy;

        GameManager.Instance.RigisterPlayer(characterStats);

        healthUI = FindObjectOfType<PlayerHealthUI>().GetComponent<PlayerHealthUI>();
    }

    private void OnDisable()
    {
        if (MouseManager.Instance != null)
        {
            MouseManager.Instance.OnMouseClicked -= MoveToTarget;
            MouseManager.Instance.OnEnemyClicked -= EventAttackEnemy;
        }
    }

    /// <summary>
    /// 刷新血条信息和 EXP信息的 UI
    /// </summary>
    public void UpdateHealthAndExp()
    {
        // Debug.Log("刷新主角的 UI");
        healthUI.UpdatePalyerHealthAndExp(characterStats.currentCharacterSOData.currentLevel,
            characterStats.currentCharacterSOData.currentHealth, characterStats.currentCharacterSOData.maxHealth,
            characterStats.currentCharacterSOData.currentExp, characterStats.currentCharacterSOData.baseExp);
    }

    /// <summary>
    /// 订阅函数：朝着地图上点击的目标移动，停止朝敌人移动的协程函数
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return; //如果玩家挂了，就不可以移动等操作了
        agent.stoppingDistance = stopDistanceAgent;
        agent.isStopped = false;
        agent.destination = target;
    }

    /// <summary>
    /// 订阅函数：朝着敌人目标物体移动，启动协程
    /// </summary>
    /// <param name="targetEnemy"></param>
    private void EventAttackEnemy(GameObject targetEnemy)
    {
        if (isDead) return; //如果玩家挂了，就不可以移动等操作了
        if (targetEnemy != null)
        {
            attackTargetEnemy = targetEnemy;
            //是否发生了暴击
            characterStats.isCritical = Random.value < characterStats.attackSOData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    /// <summary>
    /// 订阅函数朝着敌人移动的 协程函数，让主角朝敌人物体移动
    /// 判定是否走到了敌人面前，走到面前以后停止 Agent寻路，然后每间隔 0.5秒播放攻击动画一次
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        //这是为了主角进入攻击状态时候，加大停止的范围，不然一直在寻找敌人（敌人 agent 半径很大的那种情况）
        agent.stoppingDistance = characterStats.attackSOData.attackRange;

        attackTargetEnemy?.transform.LookAt(attackTargetEnemy.transform); //脸朝向目标
        //读取主角的攻击范围，按照确定到达了主角的攻击范围而停下
        if (attackTargetEnemy == null) yield break;
        while (Vector3.Distance(attackTargetEnemy.transform.position, transform.position)
               > characterStats.attackSOData.attackRange)
        {
            agent.destination = attackTargetEnemy.transform.position;
            yield return null;
        }

        agent.isStopped = true;
        //攻击敌人
        if (lastAttackTime <= 0)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("Critical", characterStats.isCritical);
            lastAttackTime = characterStats.attackSOData.coolDown;
        }
    }

    /// <summary>
    /// Animation Event 动画事件，发生击打动画，调用攻击计算血量函数
    /// 这个函数看起来貌似没有调用，实际上有哦，动画事件，在主角的攻击动画中 Animation窗口找
    /// </summary>
    void Hit()
    {
        // if (attackTargetEnemy.CompareTag("Attackable") &&
        // attackTargetEnemy.GetComponent<Rock>().rockState = Rock.RockState.Nothing )
        if (attackTargetEnemy.CompareTag("Attackable"))
        {
            if (attackTargetEnemy.GetComponent<Rock>())
            {
                attackTargetEnemy.GetComponent<Rock>().rockState = Rock.RockState.HitEnemt;
                attackTargetEnemy.GetComponent<Rock>().GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTargetEnemy.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            // 打的是敌人
            var targetStats = attackTargetEnemy.GetComponent<CharacterStats>();
            transform.LookAt(targetStats.transform);
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    /// <summary>
    /// 切换动画
    /// </summary>
    public void SwichAnimation()
    {
        //这里是让主角实时更新动画状态，通过赋值给 Blend 动画融合树的 Float
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetBool("Death", isDead);
    }


    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0; //判断主角是否挂了
        if (isDead)
            GameManager.Instance.NotifyObservers();

        SwichAnimation();
        lastAttackTime -= Time.deltaTime;

        if (characterStats.haveWeapon)
        {
            //更新角色动画
        }
    }
}