using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

///敌人的控制，公共脚本，包含敌人状态的枚举声明
public enum EnemyState //敌人当前的状态
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}
//Enemy 的几种枚举类型怪物 GUARD警戒，PATROL巡逻，CHASE追赶，DEAD死亡

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    [Header("Basic Setting 视野范围半径，是否站桩类型敌人")]
    public float sightRadius; //敌人的视线范围（能看到主角的范围半径）

    public bool isGuard; //是站桩敌人吗
    private Quaternion guardRotation; //站桩时候本来的旋转角度（脸朝向）

    [Header("Patrol State 巡逻范围大小，巡逻路线随机点位置，怪物的出生/站桩点 ")]
    public float patrolRange; //敌人的巡逻范围

    Vector3 wayPoint; //巡逻路线随机点位置
    Vector3 guardPosition; //怪物的站桩点，也是出生点

    private EnemyState enemyState;
    NavMeshAgent agent;
    Animator animator;
    private Collider collider;
    protected CharacterStats characterStats; //敌人属性读取脚本
    public float lookAtTime; //敌人巡逻时候，停下来小憩的时间
    float lastaAttackTime; //敌人上次的攻击时间，用来缓 CD

    float speed; //敌人的移动速度
    protected GameObject attackTarget; //给敌人设定一个攻击目标物体
    float remainLookAtTime; //停下来的时间计时器
    private bool playerIsDead; //Player 是不是死了，死了就获胜了

    //动画状态切换的 条件布尔变量，Trigger类型的可不在这写
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        characterStats = GetComponent<CharacterStats>();
        remainLookAtTime = lookAtTime;
        guardPosition = transform.position;
        guardRotation = transform.rotation;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyState.GUARD;
        }
        else
        {
            enemyState = EnemyState.PATROL;
            GetNewWayPoint();
        }

        GameManager.Instance.AddObserver(this);
    }

    private void OnEnable() //切换场景时候启用这个
    {
        //GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return; //这句防止 GameManager 还没有生成导致的报错
        GameManager.Instance.RemoveObserver(this);

        if (GetComponent<LootSpawner>() && isDead)
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }
    }

    /// <summary>
    /// 更改敌人的当前状态
    /// </summary>
    void SwitchStates()
    {
        if (isDead)
            enemyState = EnemyState.DEAD;
        //如果发现 player，切换到 CHASE 追击状态
        else if (FoundPlayer())
            enemyState = EnemyState.CHASE;

        switch (enemyState) //根据敌人的状态，做数值调整
        {
            case EnemyState.GUARD: //警戒
                //isWalk = false;
                isChase = false;
                if (transform.position != guardPosition)
                {
                    //Debug.Log("他位置不对");
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPosition;
                    if (Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance + 0.5f)
                    {
                        //if 语句中的 +0.5f 判断是因为这里有一个bug，关于脚本一运行，敌人就会下陷一块的问题没有解决
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.1f);
                    }
                }

                break;
            case EnemyState.PATROL: //巡逻
                isChase = false;
                agent.speed = speed * 0.5f; //巡逻速度减半
                //判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyState.CHASE: //追击
                //在攻击的范围攻击，播放动画
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer()) //这模块写的是，脱战
                {
                    //主角超出范围了，就回到上一个状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                        enemyState = EnemyState.GUARD;
                    else
                        enemyState = EnemyState.PATROL;
                }
                else //这模块是，追主角
                {
                    //发现了主角，更改动画状态为追击主角，agent 目的地更改为攻击目标
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                //主角是否在（普攻或技能攻击）范围内
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastaAttackTime < 0) //CD时间过完了，说明可以放技能
                    {
                        lastaAttackTime = characterStats.attackSOData.coolDown; //将CD时间重置
                        //暴击的判断
                        characterStats.isCritical = Random.value < characterStats.attackSOData.criticalChance;
                        //进行攻击
                        Attack();
                    }
                }

                break;
            case EnemyState.DEAD:
                collider.enabled = false;
                //agent.enabled = false;  //直接关闭 agent 会导致动画行为调用找不到 agent 报错
                agent.radius = 0; //原本就是为了让敌人死了之后不要再占烘焙地图的地方，所以把 agent.radius 变为 0就可以了
                Destroy(gameObject, 2f);
                break;
        }
    }

    /// <summary>
    /// 敌人的攻击，播放攻击动画，而伤害计算由动画调用事件计算
    /// </summary>
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            animator.SetTrigger("Attack"); //近身动画
        }

        if (TargetInSkillRange())
        {
            animator.SetTrigger("SkillAttack"); //技能攻击
        }
    }

    /// <summary>
    /// 目标是否在普攻的范围内
    /// </summary>
    /// <returns></returns>
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position,
                transform.position) <= characterStats.attackSOData.attackRange;
        return false;
    }

    /// <summary>
    /// 目标是否在技能的远距离攻击范围内
    /// </summary>
    /// <returns></returns>
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position,
                transform.position) <= characterStats.attackSOData.skillRange;
        return false;
    }

    /// <summary>
    /// 是否发现了主角
    /// </summary>
    /// <returns></returns>
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var targetCollider in colliders)
        {
            if (targetCollider.CompareTag("Player"))
            {
                attackTarget = targetCollider.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    /// <summary>
    /// Animation Event 动画事件，发生击打动画，调用攻击计算血量函数
    /// 这个函数看起来貌似没有调用，实际上有哦，动画事件，在敌人的攻击动画中 Animation窗口找
    /// </summary>
    void EnemyHit()
    {
        //判空，因为主角受玩家控制，敌人攻击时候，主角可能已经躲开了，所以必须判空，不然容易报错
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
            //Debug.Log("打了主角，主角的当前血量为"+targetStats.characterData.currentHealth);
        }
    }


    /// <summary>
    /// 把敌人的视野范围画出来，属于辅助线
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    /// <summary>
    /// 根据敌人当前是哪一种状态，切换敌人的动画
    /// </summary>
    void SwitchAnimation()
    {
        animator.SetBool("Walk", isWalk);
        animator.SetBool("Chase", isChase);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Critical", characterStats.isCritical);
        animator.SetBool("Death", isDead);
    }

    /// <summary>
    /// 随机生成一个巡逻路径的点
    /// </summary>
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //Y轴是不变的，敌人维持原有的 Y轴高度就可以
        Vector3 randomPoint = new Vector3(guardPosition.x + randomX,
            transform.position.y, guardPosition.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    /// <summary>
    /// 游戏结束通知，玩家死了
    /// </summary>
    public void EndNotify()
    {
        //获胜，停止所有移动，停止 Agent
        playerIsDead = true;
        animator.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;
        if (!playerIsDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastaAttackTime -= Time.deltaTime;
        }
    }
}