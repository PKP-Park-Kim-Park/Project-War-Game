using UnityEngine;

public class TurretData : MonoBehaviour
{
    // �� �ͷ��� ���� ���
    public int purchaseCost;

    // �� �ͷ��� �Ǹ����� �� ��� �ݾ� (���� ����� 50%�� ����)
    public int GetSellCost()
    {
        return purchaseCost / 2;
    }
}