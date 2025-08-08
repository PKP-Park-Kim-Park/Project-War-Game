using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private Dictionary<int, float> _cooldownTimes;

    private void Start()
    {
        UpdateGoldText();
        UpdateExpText();
        // 초반에는 업그레이드 상태 체크만 하고 바로 업그레이드 처리 함수는 호출하지 않도록 수정
        UpdateUpgradeButtonState();

        // 같은 비용 설정 불가 
        // 이유 : 비용대신 유닛이름 써서 나눠보려 했지만 OnUnitButton함수에 매개변수 2개 들어가서 쉽지 않아짐
        // 나중에 시간날때 고쳐봄
        _cooldownTimes = new Dictionary<int, float>()
        {
            // UnitShop1
            { 15, 1.0f },   //보병
            { 25, 2.0f },   //궁병
            { 100, 3.0f },  //탱크

            // UnitShop2

        };
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
        goldText.text = "Gold : " + gold.ToString();
    }

    public void AddExp(int amount)
    {
        exp += amount;
        UpdateExpText();
        UpdateUpgradeButtonState();
    }

    private void UpdateExpText()
    {
        expText.text = "EXP : " + exp.ToString();
    }

 
    public void OnUnitButton(int cost)
    {
        if (cooldownManager != null && !cooldownManager.IsOnCooldown())
        {
            if (HasEnoughGold(cost))
            {
                SpendGold(cost);

                // 딕셔너리에서 비용에 맞는 쿨타임 시간을 가져옴
                if (_cooldownTimes.TryGetValue(cost, out float cooldownTime))
                {
                    cooldownManager.StartCooldown(cooldownTime);
                    Debug.Log($"유닛 구매! {cost} 골드 소모.");
                }
                else
                {
                    Debug.LogWarning($"비용 {cost}에 대한 쿨타임 설정이 없습니다.");
                }
            }
            else
            {
                Debug.Log("골드가 부족합니다.");
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