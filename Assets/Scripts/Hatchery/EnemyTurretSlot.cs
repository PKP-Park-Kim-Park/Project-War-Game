using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 터렛을 장착할 수 있는 슬롯의 동작을 관리합니다.
/// </summary>
public class EnemyTurretSlot : MonoBehaviour
{
    [Header("Turret Prefabs")]
    [Tooltip("이 슬롯에 건설 가능한 적 터렛 프리팹 목록")]
    public List<GameObject> enemyTurretPrefabs;

    [Header("State (Runtime)")]
    [Tooltip("터렛이 장착되었는지 여부")]
    [SerializeField]
    private bool isOccupied = false;
    public bool IsOccupied => isOccupied;

    [Tooltip("현재 장착된 터렛의 인스턴스")]
    [SerializeField]
    private GameObject mountedTurret;
    public GameObject MountedTurret => mountedTurret;

    /// <summary>
    /// AI 컨트롤러 등 외부에서 호출하여 터렛을 건설하게 할 수 있는 public 메서드
    /// </summary>
    /// <param name="specificTurretPrefab">건설할 특정 터렛 프리팹. null이면 목록에서 무작위로 선택합니다.</param>
    public void BuildTurretByAI(GameObject specificTurretPrefab = null)
    {
        if (specificTurretPrefab != null)
        {
            MountTurret(specificTurretPrefab);
        }
        else
        {
            BuildRandomTurret();
        }
    }

    /// <summary>
    /// enemyTurretPrefabs 목록에서 무작위로 터렛을 선택하여 건설
    /// </summary>
    private void BuildRandomTurret()
    {
        if (enemyTurretPrefabs == null || enemyTurretPrefabs.Count == 0)
        {
            Debug.LogError("적 터렛 프리팹 목록이 비어있습니다. EnemyTurretSlot에 프리팹을 할당하세요.", this.gameObject);
            return;
        }

        // 목록에서 무작위 터렛 프리팹 선택
        int randomIndex = Random.Range(0, enemyTurretPrefabs.Count);
        GameObject turretToBuild = enemyTurretPrefabs[randomIndex];

        if (turretToBuild != null)
        {
            MountTurret(turretToBuild);
        }
        else
        {
            Debug.LogError($"인덱스 {randomIndex}의 적 터렛 프리팹이 null", this.gameObject);
        }
    }

    /// <summary>
    /// 지정된 터렛 프리팹을 이 터렛 슬롯에 장착
    /// </summary>
    public bool MountTurret(GameObject turretPrefab)
    {
        if (isOccupied)
        {
            Debug.LogWarning($"EnemyTurretSlot '{this.gameObject.name}'에 이미 터렛이 장착되어 있습니다.", this.gameObject);
            return false;
        }

        if (turretPrefab == null)
        {
            Debug.LogError("장착하려는 터렛 프리팹 NULL.", this.gameObject);
            return false;
        }

        // TurretSlot의 위치에 터렛을 생성
        mountedTurret = Instantiate(turretPrefab, this.transform.position, this.transform.rotation);

        // 생성된 터렛 계층 구조 정리
        if (this.transform.parent != null)
        {
            mountedTurret.transform.SetParent(this.transform.parent);
        }

        isOccupied = true;
        Debug.Log($"적 터렛 '{turretPrefab.name}'이(가) '{this.gameObject.name}'에 장착되었습니다.", this.gameObject);

        return true;
    }

    /// <summary>
    /// 이 슬롯에 장착된 터렛을 파괴합니다. AI에 의해 호출됩니다.
    /// </summary>
    public void DestroyTurret()
    {
        if (!isOccupied || mountedTurret == null)
        {
            // 이미 비어있거나 터렛이 없는 경우(예: 플레이어에 의해 파괴됨), 상태만 정리하고 종료합니다.
            isOccupied = false;
            mountedTurret = null;
            return;
        }

        Debug.Log($"'{gameObject.name}' 슬롯의 터렛 '{mountedTurret.name}'을(를) 파괴합니다.", this);
        Destroy(mountedTurret);
        mountedTurret = null;
        isOccupied = false;
    }

    // 씬 뷰에서 터렛 슬롯 영역
    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? new Color(1f, 0.5f, 0f) : Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}

// AI 연동 확장