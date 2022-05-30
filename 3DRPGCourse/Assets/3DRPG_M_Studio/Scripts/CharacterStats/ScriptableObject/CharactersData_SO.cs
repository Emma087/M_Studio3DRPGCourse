using UnityEngine;

///角色的基础属性信息 ScriptableObject类型文件
[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data", order = 1000)]
public class CharactersData_SO : ScriptableObject
{
    [Header("Stats Info 角色基础属性数值")] public int maxHealth; //基础血量
    public int currentHealth;
    public int baseDefence; //基础防御
    public int currentDefence;


    [Header("KillPoint 死了以后提供多少多少经验值")] public int killPoint; //敌人死了以后加多少经验值

    [Header("Level")] public int currentLevel; //当前等级
    public int maxLevel; //满级多少
    public int baseExp; //这次升级所需要的基础经验值
    public int currentExp; //当前的经验值
    public float levelBuff;

    public float LevelMultiplier //经验值增量
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point) //刷新经验值
    {
        currentExp += point;
        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// 升级了，刷新级别
    /// </summary>
    private void LevelUp()
    {
        //升级以后需要改变的数值
        //限制升级，不让等级超过最高的等级限制
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int) (baseExp * LevelMultiplier);
        maxHealth = (int) (maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        Debug.Log("Level Up " + currentLevel + "Max Health " + maxHealth);
    }
}