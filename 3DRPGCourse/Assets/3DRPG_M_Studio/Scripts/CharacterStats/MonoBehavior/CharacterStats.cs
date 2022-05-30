using System;
using UnityEngine;
using Random = UnityEngine.Random;

///角色的统计数据文件读取脚本
public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack; //事件用于刷新血量值（一个当前血量,一个满血值）

    public CharactersData_SO templateCharacterSOData; //原始的角色信息模板表格
    public CharactersData_SO currentCharacterSOData; //每个角色自己的，临时角色模板表格
    public AttackData_SO attackSOData; //角色攻击相关的信息模板表格

    private AttackData_SO BaseAttackDataSo; //角色基础攻击数据（没有武器的时候是什么样）
    private RuntimeAnimatorController baseAnimator; //运行游戏时候主角身上正在执行的那个动画控制器

    [HideInInspector] public bool isCritical; //是否发生暴击了

    /// <summary>
    /// 角色武器装备位置
    /// </summary>
    [Header("武器装备位置")] public Transform weaponSlot;

    /// <summary>
    /// 是否装备武器
    /// </summary>
    public bool haveWeapon = false;

    private void Awake()
    {
        if (templateCharacterSOData != null) //如果数据的模板不为空
        {
            //给每个角色，复制生成专属角色自己的 角色信息数据
            currentCharacterSOData = Instantiate(templateCharacterSOData);
        }

        BaseAttackDataSo = Instantiate(attackSOData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region 这里是主角的基础数值信息

    public int MaxHealth
    {
        get
        {
            if (currentCharacterSOData != null) return currentCharacterSOData.maxHealth;
            else return 0;
        }
        set { currentCharacterSOData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get
        {
            if (currentCharacterSOData != null) return currentCharacterSOData.currentHealth;
            else return 0;
        }
        set { currentCharacterSOData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get
        {
            if (currentCharacterSOData != null) return currentCharacterSOData.baseDefence;
            else return 0;
        }
        set { currentCharacterSOData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get
        {
            if (currentCharacterSOData != null) return currentCharacterSOData.currentDefence;
            else return 0;
        }
        set { currentCharacterSOData.currentDefence = value; }
    }

    #endregion

    #region Character Combat 角色的战斗相关数据计算

    /// <summary>
    /// 产生攻击：受到攻击伤害数值计算
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defener"></param>
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        //这里使用的 Mathf.Max 是为了限制，防御者防御过高，不掉血反而加血的问题
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        //血量不能为 0

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //如果攻击者产生暴击了，才播放伤害动画，不能每次挨揍都有动画
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            attacker.currentCharacterSOData.UpdateExp(currentCharacterSOData.killPoint);
        }

        //刷新主角血量
        GameManager.Instance.playerStats.GetComponent<PlayerController>().UpdateHealthAndExp();
    }

    /// <summary>
    /// 重载，直接产生伤害的情况用这个
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="defener"></param>
    public void TakeDamage(int damage, CharacterStats defener, Action UpdateUI)
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.currentCharacterSOData.UpdateExp(currentCharacterSOData.killPoint);
        }

        UpdateUI?.Invoke();
    }

    /// <summary>
    /// 本次伤害输出 数值的计算
    /// </summary>
    /// <returns></returns>
    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackSOData.minDamage, attackSOData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackSOData.criticalMultiplier;
        }

        return (int) coreDamage;
    }

    #endregion

    #region 关于武器，装备，卸下

    /// <summary>
    /// 切换武器
    /// </summary>
    public void ChangeWeapon(ItmeData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    /// <summary>
    /// 装备武器
    /// </summary>
    /// <param name="weapon"></param>
    public void EquipWeapon(ItmeData_SO weapon)
    {
        haveWeapon = true;
        if (weapon.weaponPrefab != null)
        {
            Debug.Log("生成一把剑");
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }

        attackSOData.ApplyWeaponData(weapon.weaponAttackSOData);
        InventoryManager.Instance.UpdateStatsText(MaxHealth, attackSOData.minDamage, attackSOData.maxDamage);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
    }

    /// <summary>
    /// 卸下武器
    /// </summary>
    public void UnEquipWeapon()
    {
        haveWeapon = false;
        if (weaponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }

        //还原攻击力数据
        attackSOData.ApplyWeaponData(BaseAttackDataSo);
        InventoryManager.Instance.UpdateStatsText(MaxHealth, attackSOData.minDamage, attackSOData.maxDamage);
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }

    #endregion

    #region 更新玩家的数据（吃了消耗品以后）

    /// <summary>
    /// 吃完了恢复血药以后，恢复血量信息
    /// </summary>
    /// <param name="addNumber"></param>
    public void ApplyHealth(int addNumber)
    {
        if (CurrentHealth + addNumber <= MaxHealth)
            CurrentHealth += addNumber;
        else
            CurrentHealth = MaxHealth;
    }

    #endregion
}