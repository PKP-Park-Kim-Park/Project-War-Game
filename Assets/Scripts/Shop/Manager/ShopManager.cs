using UnityEngine;
using System.Collections.Generic;

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
        { 15, 1.0f },
        { 25, 2.0f },
        { 100, 3.0f },

        //Unit Shop 2
        { 50, 1.5f },
        { 85, 2.5f },
        { 300, 3.5f },
    };

    // 비용(cost)을 키로, 유닛 인덱스(unitIndex)를 값으로 하는 딕셔너리
    private Dictionary<int, int> _unitCostsAndIndices = new Dictionary<int, int>();
    //터렛 비용과 인덱스를 매핑하는 딕셔너리 추가
    private Dictionary<int, int> _turretCostsAndIndices = new Dictionary<int, int>();

    void Awake()
    {
        // Awake()에서 비용과 유닛 인덱스를 매핑
        // 이 부분은 유닛 프리팹 리스트 순서에 맞게 직접 설정해야 합니다.
        _unitCostsAndIndices.Add(15, 0);   // 비용 15 골드는 unitPrefabs[0]에 해당
        _unitCostsAndIndices.Add(25, 1);   // 비용 25 골드는 unitPrefabs[1]에 해당
        _unitCostsAndIndices.Add(100, 2);  // 비용 100 골드는 unitPrefabs[2]에 해당

        //// Unit Shop 2의 유닛들도 추가
        //_unitCostsAndIndices.Add(50, 3);
        //_unitCostsAndIndices.Add(85, 4);
        //_unitCostsAndIndices.Add(300, 5);

        // 터렛 비용과 인덱스 매핑 (예시)
        _turretCostsAndIndices.Add(150, 0); // 50골드 터렛은 turretPrefabs[0]
        _turretCostsAndIndices.Add(300, 1); // 150골드 터렛은 turretPrefabs[1]
        _turretCostsAndIndices.Add(500, 2);
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
            Debug.Log("골드 부족으로 증축 불가.");
        }
    }
}

//public void OnSellingButton(int cost)
//{
//    Debug.Log("판매 완료");
//}
