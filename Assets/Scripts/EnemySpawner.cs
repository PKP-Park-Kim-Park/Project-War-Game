using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    public string age;
    public int exp;
    public List<int> unitSpawnCoolTime;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Ingredient> _ingredients;
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private float _upgradeAgeTime = 90f;
    [SerializeField] private float _spawnCoolTimeMin = 1f;
    [SerializeField] private float _spawnCoolTimeMax = 3f;
    [SerializeField] private Hatchery _hatchery;
    private float _spawnCoolTime = 0f;
    private float _time = 0f;
    private float _ageTime = 0f;
    private float _spawnUnitInt = 0f;

    private void Awake()
    {
        _spawnCoolTime = Random.Range(_spawnCoolTimeMin, _spawnCoolTimeMax);
    }

    private void Update()
    {
        CalculateUpgradeAgeTime();
        CalculateSpawnCoolTime();
    }

    private void CalculateUpgradeAgeTime()
    {
        _ageTime += Time.deltaTime;

        if (_ageTime >= _upgradeAgeTime)
        {
            _hatchery.UpgradeAge();
            _ageTime = 0;
        }
    }

    private void CalculateSpawnCoolTime()
    {
        _time += Time.deltaTime;

        if (_time < _spawnCoolTime)
        {
            return;
        }

        CalculateSpawnUnit();
        _spawnCoolTime = Random.Range(1f, 3f);
        _time = 0;
    }

    private void CalculateSpawnUnit()
    {
        _spawnUnitInt = Random.Range(1f, 100f);

        if (0f < _spawnUnitInt && _spawnUnitInt <= _ingredients[0].unitSpawnCoolTime[(int)UnitType.Normal])
        {
            _spawnPoint.SpawnUnit((int)UnitType.Normal);
        }
        else if (_spawnUnitInt <=
            _ingredients[0].unitSpawnCoolTime[(int)UnitType.Normal] +
            _ingredients[0].unitSpawnCoolTime[(int)UnitType.Long])
        {
            _spawnPoint.SpawnUnit((int)UnitType.Long);
        }
        else if (_spawnUnitInt <=
            _ingredients[0].unitSpawnCoolTime[(int)UnitType.Normal] +
            _ingredients[0].unitSpawnCoolTime[(int)UnitType.Long] +
            _ingredients[0].unitSpawnCoolTime[(int)UnitType.Tank])
        {
            _spawnPoint.SpawnUnit((int)UnitType.Tank);
        }
    }
}
