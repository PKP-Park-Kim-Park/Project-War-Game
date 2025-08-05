using UnityEngine;
using UnityEngine.UI;

public class Cash : MonoBehaviour
{
    private int gold = 10000; // 기본값 10000
    public Text goldText;  // UI Text 오브젝트 연결용

    private void Start()
    {
        UpdateGoldText();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    // 골드쓰는건 상점에서만 처리하기 때문에 private 써도 될 것 같음
    private void SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldText();
        }
    }

    private void UpdateGoldText()
    {
        goldText.text = gold.ToString() + " Gold";
    }

    // 하나의 함수로 여러 버튼 처리
    public void OnUnitButton(int cost)
    {
        SpendGold(cost);
    }
}
