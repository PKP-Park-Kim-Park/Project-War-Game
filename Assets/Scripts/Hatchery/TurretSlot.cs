using UnityEngine;

/// <summary>
/// 터렛을 장착할 수 있는 슬롯 동작 관리
/// </summary>
public class TurretSlot : MonoBehaviour
{
    [Header("Turret To Build")]
    [Tooltip("테스트용: 클릭 시 터렛 건설")]
    public GameObject turretToMountPrefab;

    [Header("State (Runtime)")]
    [Tooltip("터렛이 장착되었는지 여부를 나타냅니다. (런타임에 자동으로 변경됩니다)")]
    [SerializeField]
    private bool isOccupied = false;
    public bool IsOccupied => isOccupied;

    [Tooltip("현재 장착된 터렛의 인스턴스입니다. (런타임에 자동으로 할당됩니다)")]
    [SerializeField]
    private GameObject mountedTurret;
    public GameObject MountedTurret => mountedTurret;

    // 테스트를 위해 마우스 클릭으로 터렛을 장착합니다.
    // 이 기능을 사용하려면 TurretSlot 게임 오브젝트에 Collider가 부착되어 있어야 합니다.
    private void OnMouseDown()
    {
        Debug.Log($"TurretSlot '{this.gameObject.name}'이(가) 클릭되었습니다.");
        if (turretToMountPrefab != null)
        {
            MountTurret(turretToMountPrefab);
        }
        else
        {
            Debug.LogError("장착할 터렛 프리팹이 TurretSlot에 할당되지 않았습니다.", this.gameObject);
        }
    }

    /// <summary>
    /// 지정된 터렛 프리팹을 이 터렛 슬롯에 장착합니다.
    /// </summary>
    /// <param name="turretPrefab">장착할 터렛의 프리팹입니다.</param>
    /// <returns>장착에 성공하면 true, 이미 점유되어 있으면 false를 반환합니다.</returns>
    public bool MountTurret(GameObject turretPrefab)
    {
        if (isOccupied)
        {
            Debug.LogWarning($"TurretSlot '{this.gameObject.name}'에는 이미 터렛이 장착되어 있습니다.", this.gameObject);
            return false;
        }

        if (turretPrefab == null)
        {
            Debug.LogError("장착하려는 터렛 프리팹이 null일 수 없습니다.", this.gameObject);
            return false;
        }

        // TurretSlot의 위치에 터렛을 생성합니다.
        mountedTurret = Instantiate(turretPrefab, this.transform.position, this.transform.rotation);

        // 생성된 터렛을 TurretSpot(부모)의 자식으로 만들어 계층 구조를 정리합니다.
        if (this.transform.parent != null)
        {
            mountedTurret.transform.SetParent(this.transform.parent);
        }

        isOccupied = true;
        Debug.Log($"터렛 '{turretPrefab.name}'이(가) '{this.gameObject.name}'에 장착되었습니다.", this.gameObject);

        return true;
    }

    // 씬 뷰에서 터렛 슬롯 영역을 시각적으로 표시합니다.
    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
