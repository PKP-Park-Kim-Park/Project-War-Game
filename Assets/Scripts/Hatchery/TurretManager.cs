using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance { get; private set; }

    [Header("Current Placement State")]
    public bool isPlacingTurret = false;

    [Header("Current Selling State")]
    public bool isSellingTurret = false;

    public GameObject turretToPlacePrefab;
    public int turretCost;

    // 이 터렛 프리팹 리스트에 터렛들을 넣어줘야 합니다.
    public GameObject[] turretPrefabs;

    private List<TurretSlot> allTurretSlots = new List<TurretSlot>();

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

        // TurretSpotBuilder에서 발생하는 이벤트를 구독함.
        TurretSpotBuilder.OnTurretSpotBuilt += RegisterTurretSlot;
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독을 해제하여 메모리 누수를 방지함.
        if (Instance == this)
        {
            TurretSpotBuilder.OnTurretSpotBuilt -= RegisterTurretSlot;
        }
    }

    private void Start()
    {
        // 씬에 있는 모든 TurretSlot을 찾아서 리스트에 추가합니다.
        // 이 코드는 씬에 미리 배치된 슬롯들을 등록하기 위해 여전히 유용함.
        allTurretSlots = FindObjectsOfType<TurretSlot>().ToList();
        UpdateAllSlotsVisuals();
    }
    /// <summary>
    /// 터렛 설치 모드를 시작하고, 설치할 터렛 정보를 저장
    /// </summary>
    public void SelectTurret(int turretIndex, int cost)
    {

        if (turretIndex >= 0 && turretIndex < turretPrefabs.Length)
        {
            // 터렛 설치 모드를 시작하면, 판매 모드는 비활성화
            isSellingTurret = false;


            turretToPlacePrefab = turretPrefabs[turretIndex];
            turretCost = cost;
            isPlacingTurret = true;
            UpdateAllSlotsVisuals();
            Debug.Log($"터렛 설치모드(isPlacingTurret = true) ");

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
        UpdateAllSlotsVisuals();
        Debug.Log("Turret placement mode ended.");
    }

    /// <summary>
    /// 터렛 판매 모드 시작/종료
    /// </summary>
    public void ToggleSellMode()
    {
        // 판매 모드를 전환할 때 설치 모드는 항상 비활성화
        EndTurretPlacement();

        isSellingTurret = !isSellingTurret;
        UpdateAllSlotsVisuals();

        if (isSellingTurret)
        {
            Debug.Log("Turret sell mode started.");
        }
        else
        {
            Debug.Log("Turret sell mode ended.");
        }
    }
    public void UpdateAllSlotsVisuals()
    {
        foreach (var slot in allTurretSlots)
        {
            slot.UpdateVisual();
        }
    }

    /// <summary>
    /// 새로운 터렛 슬롯을 리스트에 등록합니다.
    /// </summary>
    /// <param name="slot">새로 추가할 터렛 슬롯</param>
    public void RegisterTurretSlot(TurretSlot slot)
    {        
        if (slot != null && !allTurretSlots.Contains(slot))
        {
            allTurretSlots.Add(slot);
        }
    }
}