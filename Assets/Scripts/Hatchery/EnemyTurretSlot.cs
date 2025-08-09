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

    [Header("Visuals")]
    [Tooltip("마우스 호버 시 표시될 시각적 효과 오브젝트")]
    public GameObject hoverVisual;

    [Header("State (Runtime)")]
    [Tooltip("터렛이 장착되었는지 여부")]
    [SerializeField]
    private bool isOccupied = false;
    public bool IsOccupied => isOccupied;

    [Tooltip("현재 장착된 터렛의 인스턴스")]
    [SerializeField]
    private GameObject mountedTurret;
    public GameObject MountedTurret => mountedTurret;

    void Start()
    {
        // 게임 시작 시 호버 효과를 비활성화
        if (hoverVisual != null)
        {
            hoverVisual.SetActive(false);
        }
    }

    // 테스트용 마우스 클릭으로 터렛 건설
    private void OnMouseDown()
    {
        Debug.Log($"EnemyTurretSlot Clicked '{this.gameObject.name}'");
        BuildRandomTurret();
    }

    /// <summary>
    /// AI 컨트롤러 등 외부에서 호출하여 터렛을 건설하게 할 수 있는 public 메서드입니다.
    /// </summary>
    public void BuildTurretByAI()
    {
        BuildRandomTurret();
    }

    /// <summary>
    /// enemyTurretPrefabs 목록에서 무작위로 터렛을 선택하여 건설합니다.
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
            Debug.LogError($"인덱스 {randomIndex}의 적 터렛 프리팹이 null입니다.", this.gameObject);
        }
    }

    /// <summary>
    /// 지정된 터렛 프리팹을 이 터렛 슬롯에 장착합니다.
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
            Debug.LogError("장착하려는 터렛 프리팹이 NULL입니다.", this.gameObject);
            return false;
        }

        // TurretSlot의 위치에 터렛을 생성합니다.
        mountedTurret = Instantiate(turretPrefab, this.transform.position, this.transform.rotation);

        // 생성된 터렛을 TurretSpot(부모)의 자식으로 만들어 계층 구조를 정리합니다.
        if (this.transform.parent != null)
        {
            mountedTurret.transform.SetParent(this.transform.parent);
        }

        // 터렛이 장착되었으므로 호버 효과를 비활성화합니다.
        if (hoverVisual != null)
        {
            hoverVisual.SetActive(false);
        }

        isOccupied = true;
        Debug.Log($"적 터렛 '{turretPrefab.name}'이(가) '{this.gameObject.name}'에 장착되었습니다.", this.gameObject);

        return true;
    }

    // 씬 뷰에서 터렛 슬롯 영역을 시각적으로 표시합니다.
    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? new Color(1f, 0.5f, 0f) : Color.magenta; // 적 슬롯은 주황/마젠타로 구분
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }

    private void OnMouseEnter()
    {
        if (!isOccupied && hoverVisual != null)
        {
            hoverVisual.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (hoverVisual != null)
        {
            hoverVisual.SetActive(false);
        }
    }
}

// AI 연동 확장