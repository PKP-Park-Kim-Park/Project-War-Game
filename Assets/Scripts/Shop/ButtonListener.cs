using UnityEngine;

public class ButtonListener : MonoBehaviour
{
    public void ActivateCanvas(GameObject canvasToActivate)
    {
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(true); // ĵ���� Ȱ��ȭ
            Debug.Log($"{canvasToActivate.name} ĵ������ Ȱ��ȭ�߽��ϴ�.");
        }
        else
        {
            Debug.LogWarning("ActivateCanvas �Լ��� �Ҵ�� ĵ������ �����ϴ�!");
        }
    }

    public void DeactivateCanvas(GameObject canvasToDeactivate)
    {
        if (canvasToDeactivate != null)
        {
            canvasToDeactivate.SetActive(false); // ĵ���� ��Ȱ��ȭ
            Debug.Log($"{canvasToDeactivate.name} ĵ������ ��Ȱ��ȭ�߽��ϴ�.");
        }
        else
        {
            Debug.LogWarning("DeactivateCanvas �Լ��� �Ҵ�� ĵ������ �����ϴ�!");
        }
    }
}