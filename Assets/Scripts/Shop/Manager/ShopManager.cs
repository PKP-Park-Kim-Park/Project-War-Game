using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public CooldownManager cooldownManager;
    public TurretSpotBuilder turretSpotBuilder;
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

    private Dictionary<int, int> _unitCostsAndIndices = new Dictionary<int, int>();
    private Dictionary<int, int> _turretCostsAndIndices = new Dictionary<int, int>();

    // 모든 유닛 합산 대기큐
    private Queue<(int unitIndex, int cost)> unitSpawnQueue = new Queue<(int, int)>();
    private const int maxQueueSize = 5;

    public Transform queueUIParent; // QueuePanel 연결
    public GameObject queueBoxPrefab; // 흰색 박스 프리팹 연결
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

        // Turret Shop 3의 터렛 추가
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

        if (unitSpawnQueue.Count >= maxQueueSize)
        {
            Debug.Log("대기큐가 가득 찼습니다. 골드 차감 없음.");
            return;
        }

        if (GoldManager.instance == null || !GoldManager.instance.SpendGold(cost))
        {
            Debug.Log("골드 부족으로 유닛 구매 불가");
            return;
        }

        unitSpawnQueue.Enqueue((unitIndex, cost));
        Debug.Log($"유닛 구매 완료. 대기큐에 추가: 인덱스 {unitIndex}");

        UpdateQueueUI();
        TryStartProduction();
    }

    public void OnCooldownFinished()
    {
        unitSpawnQueue.Dequeue();
        UpdateQueueUI();
        TryStartProduction();
    }

    // 큐에 유닛이 있고 쿨타임이 비어있을 때만 생산을 시작하는 핵심 함수
    private void TryStartProduction()
    {
        if (cooldownManager != null && !cooldownManager.IsOnCooldown() && unitSpawnQueue.Count > 0)
        {
            var (unitIndex, cost) = unitSpawnQueue.Peek(); // 큐에서 꺼내지 않고 확인만 함
            if (_cooldownTimes.TryGetValue(cost, out float cooldownTime))
            {
                cooldownManager.StartCooldown(cooldownTime, unitIndex);
                Debug.Log($"대기큐에 있는 유닛 소환 시작: 인덱스 {unitIndex}");
            }
        }
    }

    public void OnTurretButton(int cost)
    {
        if (_turretCostsAndIndices.TryGetValue(cost, out int turretIndex))
        {
            if (TurretManager.Instance != null)
            {
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
        if (turretManager != null)
        {
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

        int cost = additionCosts[currentAdditions];

        if (GoldManager.instance != null && GoldManager.instance.SpendGold(cost))
        {
            Debug.Log("증축 완료! 비용: " + cost + "골드");

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

    public void UpdateQueueUI()
    {
        foreach (var box in activeQueueBoxes)
            Destroy(box);
        activeQueueBoxes.Clear();

        int filledCount = unitSpawnQueue.Count;
        for (int i = 0; i < filledCount; i++)
        {
            GameObject box = Instantiate(queueBoxPrefab, queueUIParent);
            Image img = box.GetComponent<Image>();
            if (img != null)
                img.color = Color.white;
            activeQueueBoxes.Add(box);
        }

        for (int i = filledCount; i < maxQueueSize; i++)
        {
            GameObject box = Instantiate(queueBoxPrefab, queueUIParent);
            Image img = box.GetComponent<Image>();
            if (img != null)
                img.color = new Color(1, 1, 1, 0.5f);
            activeQueueBoxes.Add(box);
        }
    }
}