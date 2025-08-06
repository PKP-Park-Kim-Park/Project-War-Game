using UnityEngine;
using UnityEngine.UI;

public class Cash : MonoBehaviour
{
    private int gold = 10000; // 기본값 10000
    public Text goldText;  // UI Text 오브젝트 연결용

    private int exp = 0;
    public Text expText;  // UI Text 오브젝트 연결용

    public int expupgrade = 4000;
    public Button upgradeButton; // 업그레이드 버튼 연결용

    public GameObject mainCanvas;      // 메인 캔버스
    public GameObject upgradeCanvas;   // 업그레이드 후 활성화할 캔버스

    //경험치 테스트용
    private float expTimer = 0f;
    public float expIncreaseInterval = 1f; // 경험치 증가 간격 (초)
    public int expIncreaseAmount = 0;   // 증가할 경험치 양

    // 새로운 변수 추가: CooldownManager 참조
    public CooldownManager cooldownManager;

    private void Start()
    {
        UpdateGoldText();
        UpdateExpText();
        // 초반에는 업그레이드 상태 체크만 하고 바로 업그레이드 처리 함수는 호출하지 않도록 수정
        UpdateUpgradeButtonState();
    }

    //경험치 테스트용
    private void Update()
    {
        // 초당 경험치 1000씩 자동 증가 테스트용
        expTimer += Time.deltaTime;
        if (expTimer >= expIncreaseInterval)
        {
            expTimer = 0f;
            AddExp(expIncreaseAmount);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

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

    public void AddExp(int amount)
    {
        exp += amount;
        UpdateExpText();
        UpdateUpgradeButtonState();
    }

    private void UpdateExpText()
    {
        expText.text = exp.ToString() + " EXP";
    }

    public void OnUnitButton(int cost)
    {
        // CooldownManager가 연결되어 있고, 쿨타임 중이 아니며, 골드가 충분할 때만 실행
        if (cooldownManager != null && !cooldownManager.IsOnCooldown())
        {
            if (HasEnoughGold(cost))
            {
                SpendGold(cost); // 돈 차감
                // TODO: 여기에 유닛 생성 로직과 함께 쿨타임 시작 로직을 추가해야 합니다.
                // 예시: cooldownManager.StartCooldown(5f); // 5초 쿨타임 시작
                Debug.Log($"유닛 구매! {cost} 골드 소모.");
            }
            else
            {
                Debug.Log("골드가 부족합니다. 유닛을 구매할 수 없습니다.");
            }
        }
        else if (cooldownManager != null && cooldownManager.IsOnCooldown())
        {
            Debug.Log("아직 쿨타임 중입니다. 유닛을 구매할 수 없습니다.");
        }
        else
        {
            Debug.Log("CooldownManager가 연결되어 있지 않습니다. 유니티 에디터에서 연결해주세요.");
        }
    }


    public void OnTurretButton(int cost)
    {
        SpendGold(cost);
    }

    public bool HasEnoughGold(int amount)
    {
        return gold >= amount;
    }

    private void UpdateUpgradeButtonState()
    {
        // 경험치 조건 만족 시 업그레이드 버튼 활성화
        upgradeButton.gameObject.SetActive(true);
    }

    public void OnUpgradeButtonClicked()
    {
        if (exp >= expupgrade)
        {
            mainCanvas.SetActive(false);
            upgradeCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("경험치가 부족합니다. 업그레이드를 할 수 없습니다.");
        }
    }
}

