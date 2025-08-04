using UnityEngine;

public class ButtonListener : MonoBehaviour
{
    public void ActivateCanvas(GameObject canvasToActivate)
    {
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(true); // 캔버스 활성화
            Debug.Log($"{canvasToActivate.name} 캔버스를 활성화했습니다.");
        }
        else
        {
            Debug.LogWarning("ActivateCanvas 함수에 할당된 캔버스가 없습니다!");
        }
    }

    public void DeactivateCanvas(GameObject canvasToDeactivate)
    {
        if (canvasToDeactivate != null)
        {
            canvasToDeactivate.SetActive(false); // 캔버스 비활성화
            Debug.Log($"{canvasToDeactivate.name} 캔버스를 비활성화했습니다.");
        }
        else
        {
            Debug.LogWarning("DeactivateCanvas 함수에 할당된 캔버스가 없습니다!");
        }
    }
}