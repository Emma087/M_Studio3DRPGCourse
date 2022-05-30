using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

//������ҵĿ��ƽű�
public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    CharacterStats characterStats; //���ǵ����Զ�ȡ�ű�

    /// <summary>
    /// �������� Agent ����� stopDistance ��ֵ
    /// </summary>
    float stopDistanceAgent;

    /// <summary>
    /// ���ǵ�Ѫ�����
    /// </summary>
    public PlayerHealthUI healthUI;

    /// <summary>
    /// �������趨һ������Ŀ������
    /// </summary>
    GameObject attackTargetEnemy;

    /// <summary>
    /// �ϴεĹ���ʱ��
    /// </summary>
    float lastAttackTime;

    /// <summary>
    /// �����Ƿ�����
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
            Debug.Log("��������Ϊ��");
        }

        SaveManager.Instance.LoadPlayerData();

        // TODO �������������� Enable �е��ã���Ϊ�����л�����ʱ�򣬾͵�����ã�
        // ���Ƿ��� enable�����ڻᱨ����Ϊ characterStats �� Data ��ֵ����һ��
        UpdateHealthAndExp();
    }

    private void OnEnable()
    {
        //GetComponent<Collider>().enabled = false;
        if (MouseManager.Instance == null)
        {
            Debug.Log("���Ϊ��");
        }

        //���ƶ�������ӵ�������¼��Ķ�����
        MouseManager.Instance.OnMouseClicked += MoveToTarget;

        //�����ŵ����ƶ��ĺ�����ӵ��¼�����
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
    /// ˢ��Ѫ����Ϣ�� EXP��Ϣ�� UI
    /// </summary>
    public void UpdateHealthAndExp()
    {
        // Debug.Log("ˢ�����ǵ� UI");
        healthUI.UpdatePalyerHealthAndExp(characterStats.currentCharacterSOData.currentLevel,
            characterStats.currentCharacterSOData.currentHealth, characterStats.currentCharacterSOData.maxHealth,
            characterStats.currentCharacterSOData.currentExp, characterStats.currentCharacterSOData.baseExp);
    }

    /// <summary>
    /// ���ĺ��������ŵ�ͼ�ϵ����Ŀ���ƶ���ֹͣ�������ƶ���Э�̺���
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return; //�����ҹ��ˣ��Ͳ������ƶ��Ȳ�����
        agent.stoppingDistance = stopDistanceAgent;
        agent.isStopped = false;
        agent.destination = target;
    }

    /// <summary>
    /// ���ĺ��������ŵ���Ŀ�������ƶ�������Э��
    /// </summary>
    /// <param name="targetEnemy"></param>
    private void EventAttackEnemy(GameObject targetEnemy)
    {
        if (isDead) return; //�����ҹ��ˣ��Ͳ������ƶ��Ȳ�����
        if (targetEnemy != null)
        {
            attackTargetEnemy = targetEnemy;
            //�Ƿ����˱���
            characterStats.isCritical = Random.value < characterStats.attackSOData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    /// <summary>
    /// ���ĺ������ŵ����ƶ��� Э�̺����������ǳ����������ƶ�
    /// �ж��Ƿ��ߵ��˵�����ǰ���ߵ���ǰ�Ժ�ֹͣ AgentѰ·��Ȼ��ÿ��� 0.5�벥�Ź�������һ��
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        //����Ϊ�����ǽ��빥��״̬ʱ�򣬼Ӵ�ֹͣ�ķ�Χ����Ȼһֱ��Ѱ�ҵ��ˣ����� agent �뾶�ܴ�����������
        agent.stoppingDistance = characterStats.attackSOData.attackRange;

        attackTargetEnemy?.transform.LookAt(attackTargetEnemy.transform); //������Ŀ��
        //��ȡ���ǵĹ�����Χ������ȷ�����������ǵĹ�����Χ��ͣ��
        if (attackTargetEnemy == null) yield break;
        while (Vector3.Distance(attackTargetEnemy.transform.position, transform.position)
               > characterStats.attackSOData.attackRange)
        {
            agent.destination = attackTargetEnemy.transform.position;
            yield return null;
        }

        agent.isStopped = true;
        //��������
        if (lastAttackTime <= 0)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("Critical", characterStats.isCritical);
            lastAttackTime = characterStats.attackSOData.coolDown;
        }
    }

    /// <summary>
    /// Animation Event �����¼����������򶯻������ù�������Ѫ������
    /// �������������ò��û�е��ã�ʵ������Ŷ�������¼��������ǵĹ��������� Animation������
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
            // ����ǵ���
            var targetStats = attackTargetEnemy.GetComponent<CharacterStats>();
            transform.LookAt(targetStats.transform);
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    /// <summary>
    /// �л�����
    /// </summary>
    public void SwichAnimation()
    {
        //������������ʵʱ���¶���״̬��ͨ����ֵ�� Blend �����ں����� Float
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetBool("Death", isDead);
    }


    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0; //�ж������Ƿ����
        if (isDead)
            GameManager.Instance.NotifyObservers();

        SwichAnimation();
        lastAttackTime -= Time.deltaTime;

        if (characterStats.haveWeapon)
        {
            //���½�ɫ����
        }
    }
}