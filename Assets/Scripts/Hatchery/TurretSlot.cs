using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// 터렛을 장착할 수 있는 슬롯 동작 관리
/// </summary>
public class TurretSlot : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("마우스 호버 시 표시될 시각적 효과 오브젝트 => 나중에 드래그한 터렛이 위에 올라가면 나오게 수정할 필요 있음.")]
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

    private void OnMouseDown()
    {
        // 포탑 설치 모드가 아닐 경우, 아무것도 하지 않음.
        if (TurretManager.Instance == null || !TurretManager.Instance.isPlacingTurret)
        {
            return;
        }

        // TurretManager에서 설치할 터렛 정보를 가져옴
        GameObject turretToBuild = TurretManager.Instance.turretToPlacePrefab;
        int turretCost = TurretManager.Instance.turretCost;

        if (turretToBuild != null)
        {
            // 골드가 충분한지 확인하고 소모
            if (GoldManager.instance != null && GoldManager.instance.SpendGold(turretCost))
            {
                if (MountTurret(turretToBuild))
                {
                    Debug.Log("터렛 설치 완료");
                }
                TurretManager.Instance.EndTurretPlacement(); // 설치 완료 후 모드 종료
            }
            else
            {
                Debug.Log("골드 부족으로 터렛 설치 불가");
                TurretManager.Instance.EndTurretPlacement(); // 모드 종료
            }
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
        Debug.Log($"터렛 '{turretPrefab.name}'이(가) '{this.gameObject.name}'에 장착됨..", this.gameObject);

        return true;
    }

    /// <summary>
    /// 슬롯에 장착된 터렛을 제거(판매)
    /// </summary>
    private bool RemoveTurret()
    {
        if (!isOccupied)
        {
            Debug.LogWarning($"TurretSlot '{this.gameObject.name}'에 터렛이 존재하지 않음..", this.gameObject);
            return false;
        }

        if (mountedTurret != null)
        {
            Destroy(mountedTurret);
            mountedTurret = null;
            isOccupied = false;

            // TODO: 터렛 판매 비용의 일부를 반환하는 로직 추가

            Debug.Log($"'{this.gameObject.name}'의 터렛이 제거되었습니다.");
        }

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
