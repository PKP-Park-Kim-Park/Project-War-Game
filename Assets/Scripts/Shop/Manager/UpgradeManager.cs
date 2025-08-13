using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
    // 기존 캔버스를 시대별 캔버스 배열로 대체
    public GameObject[] ageCanvases;

    public Button upgradeButton;

    [Header("Hatchery Upgrade")]
    private Hatchery playerHatchery;
    public int[] expRequirements = { 4000, 10000 };

    private void Start()
    {
        UpdateUpgradeButtonState();

        GameObject playerHatcheryObject = GameObject.FindGameObjectWithTag("PlayerHatchery");
        if (playerHatcheryObject != null)
        {
            playerHatchery = playerHatcheryObject.GetComponent<Hatchery>();
        }

        if (playerHatchery == null)
        {
            Debug.LogError("UpgradeManager: PlayerHatchery를 찾을 수 없습니다. 'PlayerHatchery' 태그가 올바르게 설정되었는지 확인하세요.", this);
        }

        // 게임 시작 시 첫 번째 시대 캔버스만 활성화
        ActivateAgeCanvas(0);
    }

    public void UpdateUpgradeButtonState()
    {
        if (ExpManager.instance != null)
            upgradeButton.gameObject.SetActive(true);
    }

    // 시대에 맞는 캔버스를 활성화하고 다른 캔버스는 비활성화하는 메서드
    private void ActivateAgeCanvas(int ageIndex)
    {
        if (ageCanvases != null && ageCanvases.Length > ageIndex)
        {
            for (int i = 0; i < ageCanvases.Length; i++)
            {
                ageCanvases[i].SetActive(i == ageIndex);
            }
        }
        else
        {
            Debug.LogError("ageCanvases 배열이 설정되지 않았거나 인덱스가 유효하지 않습니다.");
        }
    }

    public void OnUpgradeButtonClicked()
    {
        int currentAgeIndex = (int)playerHatchery.CurrAge;

        if (currentAgeIndex < expRequirements.Length)
        {
            int requiredExp = expRequirements[currentAgeIndex];

            if (ExpManager.instance != null && ExpManager.instance.HasEnoughExp(requiredExp))
            {
                // 해처리 시대를 업그레이드
                playerHatchery.UpgradeAge();

                // 업그레이드 후 새로운 시대를 활성화
                int nextAgeIndex = currentAgeIndex + 1;
                ActivateAgeCanvas(nextAgeIndex);

                Debug.Log($"시대 업그레이드 완료! 다음 시대: {nextAgeIndex + 1}");

                if (nextAgeIndex + 1 >= ageCanvases.Length)
                {
                    upgradeButton.interactable = false;
                    Debug.Log("최종 시대에 도달하여 더 이상 업그레이드할 수 없습니다.");
                }
            }
            else
            {
                Debug.Log("경험치 부족.");
                MessageManager.Instance.ShowTemporaryText($"need to <color=blue>{requiredExp}</color> exp");
            }
        }
        else
        {
            Debug.Log("더 이상 업그레이드할 시대가 없습니다.");
            MessageManager.Instance.ShowTemporaryText("<color=red>full upgrade</color>");
            upgradeButton.interactable = false;
        }
    }
}