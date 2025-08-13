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
        { 65, 2.0f },
        {150, 3.0f },

        //Unit Shop 2
        { 70, 1.5f },
        { 130, 2.5f },
        { 330, 3.5f },

        //unit shop 3
        { 100, 1.5f },
        { 185, 2.5f },
        { 500, 3.5f },
    };

    // 비용(cost)을 키로, 유닛 인덱스(unitIndex)를 값으로 하는 딕셔너리
    private Dictionary<int, int> _unitCostsAndIndices = new Dictionary<int, int>();
    //터렛 비용과 인덱스를 매핑하는 딕셔너리 추가
    private Dictionary<int, int> _turretCostsAndIndices = new Dictionary<int, int>();

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
        _turretCostsAndIndices.Add(250, 0);
        _turretCostsAndIndices.Add(400, 1);
        _turretCostsAndIndices.Add(600, 2);

        // Turret Shop 2의 터렛 추가
        _turretCostsAndIndices.Add(650, 3);
        _turretCostsAndIndices.Add(800, 4);
        _turretCostsAndIndices.Add(1000, 5);

        // Turret Shop 2의 터렛 추가
        _turretCostsAndIndices.Add(900, 6);
        _turretCostsAndIndices.Add(1300, 7);
        _turretCostsAndIndices.Add(1500, 8);
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
    }

    public void OnUnitButton(int cost)
    {
        if (cooldownManager != null && !cooldownManager.IsOnCooldown())
        {
            // 골드가 충분한지 확인
            if (GoldManager.instance != null && GoldManager.instance.SpendGold(cost))
            {
                // 비용에 해당하는 쿨타임과 유닛 인덱스를 가져옴
                if (_cooldownTimes.TryGetValue(cost, out float cooldownTime) &&
                    _unitCostsAndIndices.TryGetValue(cost, out int unitIndex))
                {
                    // 쿨타임 시작 시 유닛 인덱스를 함께 전달
                    cooldownManager.StartCooldown(cooldownTime, unitIndex);
                    Debug.Log($"유닛 구매! {cost} 골드 소모. 유닛 인덱스: {unitIndex}");
                }
                else
                {
                    Debug.LogWarning($"비용 {cost}에 대한 쿨타임 또는 유닛 인덱스 설정이 없습니다.");
                }
            }
            else
            {
                Debug.Log("골드 부족!");
            }
        }
        else
        {
            Debug.Log("쿨다운 중이거나 쿨다운 매니저 미연결");
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
}

