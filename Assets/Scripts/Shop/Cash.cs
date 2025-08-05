using UnityEngine;
using UnityEngine.UI;

public class Cash : MonoBehaviour
{
    private int gold = 10000; // 기본값 10000
    public Text goldText;  // UI Text 오브젝트 연결용

    void Start()
    {
        UpdateGoldText();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    public void SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldText();
        }
    }

    void UpdateGoldText()
    {
        goldText.text = gold.ToString() + " Gold";
    }

    // 유닛 버튼별로 호출될 함수 3개
    public void OnUnitButton1()  // 15골드 차감
    {
        SpendGold(15);
    }

    public void OnUnitButton2()  // 30골드 차감
    {
        SpendGold(30);
    }

    public void OnUnitButton3()  // 100골드 차감
    {
        SpendGold(100);
    }
}
