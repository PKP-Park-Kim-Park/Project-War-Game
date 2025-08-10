using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public CooldownManager cooldownManager;
    [Tooltip("Turret Manager 넣으쇼..")]
    public TurretSpotBuilder turretSpotBuilder;

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
        if (GoldManager.instance != null && GoldManager.instance.SpendGold(cost))
        {
            Debug.Log("터렛 설치 완료");
            // 터렛 설치 로직 추가
        }
        else
        {
            Debug.Log("골드 부족으로 터렛 설치 불가");
        }

        //isInstallingTurret = true;  // 설치 모드 진입
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
