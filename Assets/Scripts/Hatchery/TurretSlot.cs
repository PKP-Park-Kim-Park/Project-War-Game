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
        // TurretManager가 없으면 아무것도 하지 않음.
        if (TurretManager.Instance == null)
        {
            return;
        }

        // --- 판매 모드일 때의 동작 ---
        if (TurretManager.Instance.isSellingTurret)
        {
            // 슬롯에 터렛이 있다면 제거(판매)
            if (isOccupied)
            {
                RemoveTurret();
                // 판매 후 판매 모드 종료 (선택 사항)
                TurretManager.Instance.ToggleSellMode();
            }
            return; // 판매 모드에서는 이 코드가 실행된 후 함수를 종료
        }

        // --- 설치 모드일 때의 동작 ---
        if (TurretManager.Instance.isPlacingTurret)
        {
            // 슬롯이 비어있을 경우에만 설치 진행
            if (!isOccupied)
            {
                GameObject turretToBuild = TurretManager.Instance.turretToPlacePrefab;
                int turretCost = TurretManager.Instance.turretCost;

                if (turretToBuild != null)
                {
                    if (GoldManager.instance != null && GoldManager.instance.SpendGold(turretCost))
                    {
                        if (MountTurret(turretToBuild))
                        {
                            Debug.Log("터렛 설치 완료");
                        }
                        TurretManager.Instance.EndTurretPlacement();
                    }
                    else
                    {
                        Debug.Log("골드 부족으로 터렛 설치 불가");
                        TurretManager.Instance.EndTurretPlacement();
                    }
                }
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
            // 터렛의 판매 금액을 가져옵니다.
            int sellValue = 0;
            TurretData turretData = mountedTurret.GetComponent<TurretData>();

            if (turretData != null)
            {
                sellValue = turretData.GetSellCost();
            }

            // 터렛을 파괴합니다.
            Destroy(mountedTurret);
            mountedTurret = null;
            isOccupied = false;

            // GoldManager를 통해 골드를 추가합니다.
            if (GoldManager.instance != null)
            {
                GoldManager.instance.AddGold(sellValue);
                Debug.Log($"터렛 판매 완료! {sellValue} 골드를 획득했습니다.");
            }
            else
            {
                Debug.LogError("GoldManager 인스턴스를 찾을 수 없습니다.");
            }

            Debug.Log($"'{this.gameObject.name}'의 터렛이 제거되었습니다.");
            return true;
        }

        return false;
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
        if (hoverVisual == null) return;

        // 비어있는 슬롯에 터렛을 설치할 때
        if (!isOccupied && TurretManager.Instance != null && TurretManager.Instance.isPlacingTurret)
        {
            hoverVisual.SetActive(true);
        }
        // 터렛이 설치된 슬롯을 판매할 때
        else if (isOccupied && TurretManager.Instance != null && TurretManager.Instance.isSellingTurret)
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
