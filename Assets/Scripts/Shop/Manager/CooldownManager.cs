using UnityEngine;
using UnityEngine.UI;

public class CooldownManager : MonoBehaviour
{
    public Slider cooltimeSlider;
    public SpawnPoint spawnPoint; // SpawnPoint 스크립트 참조

    private int unitIndexToSpawn; // 스폰할 유닛의 인덱스 저장
    private float currentCooldownTime = 0f;
    private float maxCooldownTime = 0f; // 버튼 클릭 시 설정될 쿨타임 시간
    private bool isCooldown = false;

    private ShopManager shopManager;

    void Start()
    {
        if (cooltimeSlider != null)
        {
            cooltimeSlider.value = 0;
        }
        shopManager = FindObjectOfType<ShopManager>();
        if (shopManager == null)
            Debug.LogError("ShopManager 인스턴스를 찾을 수 없습니다.");
    }

    private void Update()
    {
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime;
            cooltimeSlider.value = currentCooldownTime / maxCooldownTime;

            if (currentCooldownTime >= maxCooldownTime)
            {
                isCooldown = false;
                currentCooldownTime = 0;
                cooltimeSlider.value = 0;

                // 쿨타임이 끝나면 유닛을 스폰합니다.
                if (spawnPoint != null)
                {
                    spawnPoint.SpawnUnit(unitIndexToSpawn);
                }

                Debug.Log("쿨타임 종료! 다시 사용 가능합니다.");

                // **쿨타임이 끝났으니 ShopManager에게 다음 유닛을 처리하도록 알립니다.**
                if (shopManager != null)
                    shopManager.OnCooldownFinished();// 대기큐 다음 아이템 처리 호출
            }
        }
    }

    public bool IsOnCooldown()
    {
        return isCooldown;
    }

    public void StartCooldown(float buttonCooldownTime, int unitIndex)
    {
        // 쿨타임 시작 로직은 동일
        Debug.Log($"쿨타임 시작: {buttonCooldownTime}초");
        isCooldown = true;
        maxCooldownTime = buttonCooldownTime;
        currentCooldownTime = 0;
        cooltimeSlider.value = 0;

        unitIndexToSpawn = unitIndex;
    }
}