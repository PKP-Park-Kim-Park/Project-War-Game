using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GoldManager goldManager;
    public CooldownManager cooldownManager;

    // ���� ����� ��� �迭
    private int[] additionCosts = { 1000, 3000, 5000, 8000 };
    // ���� ���� Ƚ��
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

        //isInstallingTurret = true;  // ��ġ ��� ����
    }

    public void OnAdditionButton()
    {
        if (currentAdditions >= maxAdditions)
        {
            Debug.Log("�� �̻� ������ �� �����ϴ�. �ִ� ���� Ƚ���� �����߽��ϴ�.");
            return;
        }

        // ���� ���� �ܰ迡 �´� ����� ������
        int cost = additionCosts[currentAdditions];

        // ��尡 ������� Ȯ��
        if (goldManager != null && goldManager.SpendGold(cost))
        {
            Debug.Log("���� �Ϸ�! ���: " + cost + "���");

            //// ����'s �ͷ� ���� �ڵ� �ʿ�
            //if (turretManager != null)
            //{
            //    turretManager.BuildTurretSpot();
            //}
            currentAdditions++;

        }
        else
        {
            Debug.Log("��� �������� ���� �Ұ�.");
        }
    }
}

    //public void OnSellingButton(int cost)
    //{
    //    Debug.Log("�Ǹ� �Ϸ�");
    //}

