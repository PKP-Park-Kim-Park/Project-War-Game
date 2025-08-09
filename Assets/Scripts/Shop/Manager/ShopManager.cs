using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GoldManager goldManager;
    public CooldownManager cooldownManager;

    // 증축 비용을 담는 배열
    private int[] additionCosts = { 1000, 3000, 5000, 8000 };
    // 현재 증축 횟수
    private int currentAdditions = 0;
    private int maxAdditions = 4;

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

    public void OnUnitButton(int cost)
    {
        if (cooldownManager != null && !cooldownManager.IsOnCooldown())
        {
            if (goldManager != null && goldManager.SpendGold(cost))
            {
                if (_cooldownTimes.TryGetValue(cost, out float cooldownTime))
                {
                    cooldownManager.StartCooldown(cooldownTime);
                    Debug.Log($"유닛 구매! {cost} 골드 소모.");
                }
                else
                {
                    Debug.LogWarning($"비용 {cost}에 대한 쿨타임 설정이 없습니다.");
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
        if (goldManager != null && goldManager.SpendGold(cost))
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
        if (goldManager != null && goldManager.SpendGold(cost))
        {
            Debug.Log("증축 완료! 비용: " + cost + "골드");

            //// 상훈's 터렛 증축 코드 필요
            //if (turretManager != null)
            //{
            //    turretManager.BuildTurretSpot();
            //}
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

