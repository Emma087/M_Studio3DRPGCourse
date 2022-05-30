using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///角色的攻击数值信息 ScriptableObject类型文件
[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Header("攻击的距离范围")] public float attackRange; //攻击的距离范围
    [Header("远距离的攻击距离，可能是指技能")] public float skillRange; //远距离的攻击距离，可能是指技能
    [Header("CD时间")] public float coolDown; //CD时间
    [Header("最小攻击数值")] public int minDamage; //最小攻击数值
    [Header("最大攻击数值")] public int maxDamage; //最大攻击数值

    [Header("暴击加成倍数")] public float criticalMultiplier; //暴击加成倍数
    [Header("暴击率")] public float criticalChance; //暴击率


    /// <summary>
    /// 替换攻击数值文件（如果捡起一把武器就要使用武器的数值，替换掉空手的角色的自身数值）
    /// </summary>
    /// <param name="weapon"></param>
    public void ApplyWeaponData(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalMultiplier = weapon.criticalMultiplier;
        criticalChance = weapon.criticalChance;
    }
}