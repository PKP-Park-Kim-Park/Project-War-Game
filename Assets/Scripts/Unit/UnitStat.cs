using UnityEngine;

public class UnitStat
{
    public float MoveSpeed { get; private set; }
    public int MaxHealth { get; private set; }
    public int AttackDamage { get; private set; }
    public float AttackRange { get; private set; }
    public UnitType UnitType { get; private set; }
    public string AttackTargetTag { get; private set; }
    public string StopTargetTag { get; private set; }

    public void Initialize(UnitData unitData, string unitTag)
    {
        MoveSpeed = unitData.moveSpeed;
        MaxHealth = unitData.maxHealth;
        AttackDamage = unitData.attackDamage;
        AttackRange = unitData.attackRange;
        UnitType = unitData.unitType;

        if (unitTag == "Player")
        {
            AttackTargetTag = "Enemy";
            StopTargetTag = "Player";
        }
        else if (unitTag == "Enemy")
        {
            AttackTargetTag = "Player";
            StopTargetTag = "Enemy";
        }
        else
        {
            Debug.LogWarning("유닛 태그가 잘못 지정되어 있음.");
        }
    }
}
