using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GoldManager goldManager;
    public CooldownManager cooldownManager;

    private Dictionary<int, float> _cooldownTimes = new Dictionary<int, float>()
    {
        { 15, 1.0f },
        { 25, 2.0f },
        { 100, 3.0f }
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
    }
}
