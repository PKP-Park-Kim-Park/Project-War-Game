using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private UnitType _unitType;
    public UnitType unitType { get { return _unitType; } }

    [SerializeField] 
    private float _moveSpeed;
    public float moveSpeed { get { return _moveSpeed; } }

    [SerializeField] 
    private int _maxHealth;
    public int maxHealth { get { return _maxHealth; } }

    [SerializeField] 
    private int _attackDamage;
    public int attackDamage { get { return _attackDamage; } }

    [SerializeField]
    private float _attackRange;
    public float attackRange { get { return _attackRange; } }
}
