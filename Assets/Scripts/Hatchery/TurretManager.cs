using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 건설할 터렛의 종류를 선택하고 관리하는 중앙 관리자 클래스
/// </summary>
public class TurretManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static TurretManager Instance { get; private set; }

    [Header("Available Turrets")]
    [Tooltip("건설 가능한 터렛 프리팹 목록. 숫자 키 1, 2, 3... 에 매핑됩니다.")]
    public List<GameObject> turretPrefabs;

    // 현재 선택된 터렛 프리팹
    private GameObject selectedTurretPrefab;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("TurretManager 인스턴스가 이미 존재하여 새로 생성된 인스턴스를 파괴합니다.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 게임 시작 시 기본으로 첫 번째 터렛을 선택합니다.
        /*
        if (turretPrefabs != null && turretPrefabs.Count > 0)
        {
            SelectTurret(0);
        }
        */
    }

    /// <summary>
    /// 건설할 터렛을 선택합니다.
    /// </summary>
    /// <param name="turretIndex">turretPrefabs 목록의 인덱스</param>
    public void SelectTurret(int turretIndex)
    {
        if (turretPrefabs == null || turretIndex < 0 || turretIndex >= turretPrefabs.Count)
        {
            Debug.LogError($"유효하지 않은 터렛 인덱스: {turretIndex}", this);
            selectedTurretPrefab = null;
            return;
        }

        selectedTurretPrefab = turretPrefabs[turretIndex];
        Debug.Log($"<color=yellow>'{selectedTurretPrefab.name}' 터렛 선택됨.</color>");
    }

    /// <summary>
    /// 현재 선택된 터렛 프리팹을 반환합니다.
    /// </summary>
    public GameObject GetSelectedTurretPrefab()
    {
        return selectedTurretPrefab;
    }

    // 테스트용: 숫자 키로 터렛 선택
    void Update()
    {
        // 숫자 키 1, 2, 3으로 터렛을 선택합니다.
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectTurret(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectTurret(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectTurret(2);
        }
    }
}
