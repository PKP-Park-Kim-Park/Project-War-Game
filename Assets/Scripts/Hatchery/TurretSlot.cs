using UnityEngine;

/// <summary>
/// 터렛을 장착할 수 있는 슬롯 동작 관리
/// </summary>
public class TurretSlot : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("마우스 호버 시 표시될 시각적 효과 오브젝트 => 나중에 드래그한 터렛이 위에 올라가면 나오게 수정할 필요 있음.")]
    public GameObject hoverVisual;

    [Header("Turret To Build")]
    [Tooltip("테스트용: 클릭 시 터렛 건설")]
    public GameObject turretToMountPrefab;

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
        // 게임 시작 시 호버 효과를 비활성화합니다.
        if (hoverVisual != null)
        {
            hoverVisual.SetActive(false);
        }
    }

    // 테스트용 마우스 클릭으로 터렛 건설
    private void OnMouseDown()
    {
        Debug.Log($"TurretSlot Clicked '{this.gameObject.name}'");
        if (turretToMountPrefab != null)
        {
            MountTurret(turretToMountPrefab);
        }
        else
        {
            Debug.LogError("장착할 터렛 프리팹이 TurretSlot에 할당되지 않음..", this.gameObject);
        }
    }

    /// <summary>
    /// 지정된 터렛 프리팹을 이 터렛 슬롯에 장착
    /// </summary>

    public bool MountTurret(GameObject turretPrefab)
    {
        if (isOccupied)
        {
            Debug.LogWarning($"TurretSlot '{this.gameObject.name}'에 이미 터렛이 장착되어 있음..", this.gameObject);
            return false;
        }

        if (turretPrefab == null)
        {
            Debug.LogError("터렛 프리팹이 NULL...", this.gameObject);
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
        Debug.Log($"터렛 '{turretPrefab.name}'이(가) '{this.gameObject.name}'에 장착됨..", this.gameObject);

        return true;
    }

    // 씬 뷰에서 터렛 슬롯 영역을 시각적으로 표시합니다.
    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.cyan;

        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }

    /// <summary>
    /// 호버 효과
    /// </summary>
    private void OnMouseEnter()
    {
        // 슬롯이 비었을 때만 호버 효과 표시
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
