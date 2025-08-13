using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject upgradeCanvas;
    public Button upgradeButton;

    [Header("Hatchery Upgrade")]
    private Hatchery playerHatchery;
    public int expRequired = 4000;

    private void Start()
    {
        UpdateUpgradeButtonState();

        // 플레이어의 해처리를 찾아서 연결합니다.
        GameObject playerHatcheryObject = GameObject.FindGameObjectWithTag("PlayerHatchery");
        if (playerHatcheryObject != null)
        {
            playerHatchery = playerHatcheryObject.GetComponent<Hatchery>();
        }

        if (playerHatchery == null)
        {
            Debug.LogError("UpgradeManager: PlayerHatchery를 찾을 수 없습니다. 'PlayerHatchery' 태그가 올바르게 설정되었는지 확인하세요.", this);
        }
    }

    public void UpdateUpgradeButtonState()
    {
        if (ExpManager.instance != null)
            upgradeButton.gameObject.SetActive(true);
    }

    public void OnUpgradeButtonClicked()
    {
        if (ExpManager.instance != null && ExpManager.instance.HasEnoughExp(expRequired))
        {
            mainCanvas.SetActive(false);
            upgradeCanvas.SetActive(true);
            playerHatchery.UpgradeAge();
            Debug.Log($"시대 업그레이드 완료!");

            // 최종 시대에 도달하면 버튼을 비활성화합니다.
            if (playerHatchery.CurrAge == Age.Advanced)
            {
                upgradeButton.interactable = false;
            }
        }
        else
        {
            Debug.Log("경험치 부족.");
        }
    }
}
