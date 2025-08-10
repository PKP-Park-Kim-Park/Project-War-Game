using UnityEngine;
using System.Collections.Generic;

public class UnitStat
{
    public float MoveSpeed { get; private set; }
    public int MaxHealth { get; private set; }
    public int AttackDamage { get; private set; }
    public float AttackRange { get; private set; }
    public UnitType UnitType { get; private set; }
    public List<string> AttackTargetTags { get; private set; }
    public string StopTargetTag { get; private set; }
    public int Gold { get; private set; }
    public int Exp { get; private set; }


    public void Initialize(UnitData unitData, string unitTag)
    {
        MoveSpeed = unitData.moveSpeed;
        MaxHealth = unitData.maxHealth;
        AttackDamage = unitData.attackDamage;
        AttackRange = unitData.attackRange;
        UnitType = unitData.unitType;
        Gold = unitData.gold;
        Exp = unitData.exp;

        if (unitTag == "Player")
        {
            AttackTargetTags = new List<string> { "Enemy", "EnemyHatchery" };
            StopTargetTag = "Player";
        }
        else if (unitTag == "Enemy")
        {
            AttackTargetTags = new List<string> { "Player", "PlayerHatchery" };
            StopTargetTag = "Enemy";
        }
        else
        {
            Debug.LogWarning("유닛 태그가 잘못 지정되어 있음.");
        }
    }
}
