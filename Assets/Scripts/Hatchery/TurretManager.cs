using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance { get; private set; }

    [Header("Current Placement State")]
    public bool isPlacingTurret = false;
    public GameObject turretToPlacePrefab;
    public int turretCost;

    // 이 터렛 프리팹 리스트에 터렛들을 넣어줘야 합니다.
    public GameObject[] turretPrefabs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 터렛 설치 모드를 시작하고, 설치할 터렛 정보를 저장
    /// </summary>
    public void SelectTurret(int turretIndex, int cost)
    {
        if (turretIndex >= 0 && turretIndex < turretPrefabs.Length)
        {
            turretToPlacePrefab = turretPrefabs[turretIndex];

            // TODO: 터렛 인덱스에 맞는 비용을 설정하는 로직 추가 필요
            turretCost = cost;

            isPlacingTurret = true;
            Debug.Log($"Turret placement mode started for {turretToPlacePrefab.name}.");
        }
        else
        {
            Debug.LogError($"Invalid turret index: {turretIndex}");
        }
    }

    /// <summary>
    /// 터렛 설치를 완료하거나 취소하고 상태를 초기화
    /// </summary>
    public void EndTurretPlacement()
    {
        isPlacingTurret = false;
        turretToPlacePrefab = null;
        turretCost = 0;
        Debug.Log("Turret placement mode ended.");
    }

}