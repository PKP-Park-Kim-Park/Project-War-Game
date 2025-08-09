using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public int expRequired = 4000;
    public GameObject mainCanvas;
    public GameObject upgradeCanvas;
    public Button upgradeButton;

    public ExpManager expManager;

    private void Start()
    {
        UpdateUpgradeButtonState();
    }

    public void UpdateUpgradeButtonState()
    {
        if (expManager != null)
            upgradeButton.gameObject.SetActive(true);
    }

    public void OnUpgradeButtonClicked()
    {
        if (expManager != null && expManager.HasEnoughExp(expRequired))
        {
            mainCanvas.SetActive(false);
            upgradeCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("경험치가 부족합니다.");
        }
    }
}
