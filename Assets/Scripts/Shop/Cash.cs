using UnityEngine;
using UnityEngine.UI;

public class Cash : MonoBehaviour
{
    private int gold = 10000; // �⺻�� 10000
    public Text goldText;  // UI Text ������Ʈ �����

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

    // ���� ��ư���� ȣ��� �Լ� 3��
    public void OnUnitButton1()  // 15��� ����
    {
        SpendGold(15);
    }

    public void OnUnitButton2()  // 30��� ����
    {
        SpendGold(30);
    }

    public void OnUnitButton3()  // 100��� ����
    {
        SpendGold(100);
    }
}
