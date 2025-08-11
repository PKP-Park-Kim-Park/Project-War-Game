using UnityEngine;

public class TurretData : MonoBehaviour
{
    // 이 터렛의 구매 비용
    public int purchaseCost;

    // 이 터렛을 판매했을 때 얻는 금액 (구매 비용의 50%로 가정)
    public int GetSellCost()
    {
        return purchaseCost / 2;
    }
}