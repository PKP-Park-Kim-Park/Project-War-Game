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
                    Debug.Log($"���� ����! {cost} ��� �Ҹ�.");
                }
                else
                {
                    Debug.LogWarning($"��� {cost}�� ���� ��Ÿ�� ������ �����ϴ�.");
                }
            }
            else
            {
                Debug.Log("��� ����!");
            }
        }
        else
        {
            Debug.Log("��ٿ� ���̰ų� ��ٿ� �Ŵ��� �̿���");
        }
    }

    public void OnTurretButton(int cost)
    {
        if (goldManager != null && goldManager.SpendGold(cost))
        {
            Debug.Log("�ͷ� ��ġ �Ϸ�");
            // �ͷ� ��ġ ���� �߰�
        }
        else
        {
            Debug.Log("��� �������� �ͷ� ��ġ �Ұ�");
        }
    }
}
