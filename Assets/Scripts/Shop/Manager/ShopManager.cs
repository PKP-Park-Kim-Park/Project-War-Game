using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public CooldownManager cooldownManager;
    [Tooltip("Turret Manager 넣으쇼..")]
    public TurretSpotBuilder turretSpotBuilder;
    // 터렛 매니저 참조 변수 추가
    public TurretManager turretManager;

    // 증축 비용을 담는 배열
    private int[] additionCosts = { 1000, 3000, 5000 };
    // 현재 증축 횟수
    private int currentAdditions = 0;
    private int maxAdditions = 3;

    private Dictionary<int, float> _cooldownTimes = new Dictionary<int, float>()
    {
        //Unit Shop 1
        { 25, 1.0f },
        { 65, 1.3f },
        {150, 2.0f },

        //Unit Shop 2
        { 70, 1.2f },
        { 130, 1.8f },
        { 330, 2.5f },

        //unit shop 3
        { 100, 1.5f },
        { 185, 2.2f },
        { 500, 3.0f },
    };

    // 비용(cost)을 키로, 유닛 인덱스(unitIndex)를 값으로 하는 딕셔너리
    private Dictionary<int, int> _unitCostsAndIndices = new Dictionary<int, int>();
    //터렛 비용과 인덱스를 매핑하는 딕셔너리 추가
    private Dictionary<int, int> _turretCostsAndIndices = new Dictionary<int, int>();

    // 모든 유닛 합산 대기큐
    private Queue<(int unitIndex, int cost)> unitSpawnQueue = new Queue<(int, int)>();
    private const int maxQueueSize = 4;

    // UI 관련 변수
    public Transform queueUIParent;              // QueuePanel 연결
    public GameObject queueBoxPrefab;            // 흰색 박스 프리팹 연결
    private List<GameObject> activeQueueBoxes = new List<GameObject>();  // 현재 띄워진 박스 리스트

    void Awake()
    {
        // Unit Shop 1의 유닛 추가
        _unitCostsAndIndices.Add(25, 0);
        _unitCostsAndIndices.Add(65, 1);
        _unitCostsAndIndices.Add(150, 2);

        // Unit Shop 2의 유닛 추가
        _unitCostsAndIndices.Add(70, 3);
        _unitCostsAndIndices.Add(130, 4);
        _unitCostsAndIndices.Add(330, 5);

        // Unit Shop 3의 유닛 추가
        _unitCostsAndIndices.Add(100, 6);
        _unitCostsAndIndices.Add(185, 7);
        _unitCostsAndIndices.Add(500, 8);

        // Turret Shop 1의 터렛 추가
        _turretCostsAndIndices.Add(300, 0);
        _turretCostsAndIndices.Add(500, 1);
        _turretCostsAndIndices.Add(900, 2);

        // Turret Shop 2의 터렛 추가
        _turretCostsAndIndices.Add(750, 3);
        _turretCostsAndIndices.Add(1100, 4);
        _turretCostsAndIndices.Add(1700, 5);

        // Turret Shop 2의 터렛 추가
        _turretCostsAndIndices.Add(1500, 6);
        _turretCostsAndIndices.Add(2200, 7);
        _turretCostsAndIndices.Add(3000, 8);
    }

    void Start()
    {
        if (turretManager == null)
        {
            turretManager = TurretManager.Instance;
            if (turretManager == null)
            {
                Debug.LogError("TurretManager 인스턴스를 찾을 수 없습니다. 씬에 TurretManager를 배치했는지 확인하세요.");
            }
        }
        UpdateQueueUI();
    }

    public void OnUnitButton(int cost)
    {
        if (!_unitCostsAndIndices.TryGetValue(cost, out int unitIndex))
            return;

        // 대기큐 공간 먼저 확인 (골드 차감 전에)
        if (unitSpawnQueue.Count >= maxQueueSize)
        {
            Debug.Log("대기큐가 가득 찼습니다. 골드 차감 없음.");
            // 필요시 UI에 대기큐 꽉 찼다는 메시지 표시
            return;
        }

        // 골드가 충분한지 확인하고 차감
        if (GoldManager.instance == null || !GoldManager.instance.SpendGold(cost))
        {
            Debug.Log("골드 부족으로 유닛 구매 불가");
            return;
        }

        // 쿨타임 없는 경우 즉시 소환
        if (cooldownManager != null && !cooldownManager.IsOnCooldown())
        {
            if (_cooldownTimes.TryGetValue(cost, out float cooldownTime))
            {
                cooldownManager.StartCooldown(cooldownTime, unitIndex);
                Debug.Log($"유닛 즉시 소환: 인덱스 {unitIndex}");
            }
        }
        else
        {
            // 쿨타임 중이면 대기큐에 추가
            unitSpawnQueue.Enqueue((unitIndex, cost));
            Debug.Log($"대기큐에 추가: 인덱스 {unitIndex}");
            UpdateQueueUI();
        }
    }



    public void OnTurretButton(int cost)
    {
        if (_turretCostsAndIndices.TryGetValue(cost, out int turretIndex))
        {
            // TurretManager에 포탑 설치 모드 시작을 알림
            if (TurretManager.Instance != null)
            {
                // 포탑 설치 모드 시작. 골드는 아직 소모하지 않음.
                TurretManager.Instance.SelectTurret(turretIndex, cost); 
                Debug.Log("Turret설치 모드 시작");
            }
            else
            {
                Debug.LogError("TurretManager 인스턴스를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"비용 {cost}에 해당하는 터렛이 없습니다.");
        }
    }

    public void OnSellingButton(int cost)
    {

        // 터렛 매니저가 있는지 확인
        if (turretManager != null)
        {
            // 터렛 매니저의 ToggleSellMode 함수를 호출하여 판매 모드 전환
            turretManager.ToggleSellMode();
            Debug.LogWarning($"터렛 판매 모드 토글!");
        }
        else
        {
            Debug.LogError("TurretManager 인스턴스를 찾을 수 없습니다.");
        }
    }


    public void OnAdditionButton()
    {
        if (currentAdditions >= maxAdditions)
        {
            Debug.Log("더 이상 증축할 수 없습니다. 최대 증축 횟수에 도달했습니다.");
            MessageManager.Instance.ShowTemporaryText("<color=red>full addition</color>");
            return;
        }

        // 현재 증축 단계에 맞는 비용을 가져옴
        int cost = additionCosts[currentAdditions];

        // 골드가 충분한지 확인
        if (GoldManager.instance != null && GoldManager.instance.SpendGold(cost))
        {
            Debug.Log("증축 완료! 비용: " + cost + "골드");

            // TurretSpotBuilder의 터렛 스팟 건설 메서드 호출
            if (turretSpotBuilder != null)
            {
                turretSpotBuilder.BuildTurretSpot();
            }
            else
            {
                Debug.LogError("TurretSpotBuilder가 ShopManager에 연결되지 않았습니다.");
            }
            currentAdditions++;

        }
        else
        {
            Debug.Log(cost + "골드 있어야 증축 가능");
            MessageManager.Instance.ShowTemporaryText($"need to <color=#FFD700>{cost}</color> gold");

        }
    }
    public void OnCooldownFinished()
    {
        if (unitSpawnQueue.Count > 0)
        {
            var (unitIndex, cost) = unitSpawnQueue.Dequeue();
            if (_cooldownTimes.TryGetValue(cost, out float cooldownTime))
            {
                cooldownManager.StartCooldown(cooldownTime, unitIndex);
                UpdateQueueUI();
                Debug.Log($"대기큐에서 유닛 소환 시작: 인덱스 {unitIndex}");
            }
        }
    }

    public void UpdateQueueUI()
    {
        // 기존 박스 모두 제거
        foreach (var box in activeQueueBoxes)
            Destroy(box);
        activeQueueBoxes.Clear();

        // 채워진 박스: 실제 대기큐에 들어있는 유닛 개수만큼
        int filledCount = unitSpawnQueue.Count;
        for (int i = 0; i < filledCount; i++)
        {
            GameObject box = Instantiate(queueBoxPrefab, queueUIParent);
            // 기본: 채워진 박스(불투명 흰색 등)
            Image img = box.GetComponent<Image>();
            if (img != null)
                img.color = Color.white;
            activeQueueBoxes.Add(box);
        }

        // 빈 박스: (최대 - 현재 대기큐 개수)만큼 반복
        for (int i = filledCount; i < maxQueueSize; i++)
        {
            GameObject box = Instantiate(queueBoxPrefab, queueUIParent);
            // 빈 칸 연출: 투명도 낮추거나 회색 등
            Image img = box.GetComponent<Image>();
            if (img != null)
                img.color = new Color(1, 1, 1, 0.5f); // 연한 흰색
            activeQueueBoxes.Add(box);
        }
    }


}

