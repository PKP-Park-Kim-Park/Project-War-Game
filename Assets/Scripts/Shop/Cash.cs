using UnityEngine;
using UnityEngine.UI;

public class Cash : MonoBehaviour
{
    private int gold = 10000; // �⺻�� 10000
    public Text goldText;  // UI Text ������Ʈ �����

    private void Start()
    {
        UpdateGoldText();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    // ��徲�°� ���������� ó���ϱ� ������ private �ᵵ �� �� ����
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

    // �ϳ��� �Լ��� ���� ��ư ó��
    public void OnUnitButton(int cost)
    {
        SpendGold(cost);
    }
}
